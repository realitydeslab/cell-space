using UnityEngine;
using Unity.Sentis;
using Bibcam.Decoder;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.CompilerServices;
using System;
using static Unity.Mathematics.math;
using Format = UnityEngine.XR.ARSubsystems.XRCpuImage.Format;
using UnityEngine.Rendering;
using System.Drawing;
using UnityEngine.Experimental.Rendering;
using System.Linq;

namespace RealityDesignLab.MoveNet
{
    public sealed class MoveNetMultipose3D : MonoBehaviour {
        [SerializeField] private BibcamMetadataDecoder _decoder = null;
        [SerializeField] private BibcamTextureDemuxer _demux = null;

        [SerializeField] private Camera _camera;
        [SerializeField] private MoveNetMultiposeVisualizer _visualizer = null;
        [SerializeField] private MoveNetMultipose3DVisualizer _3dvisualizer = null;

        [SerializeField] private ModelAsset _modelAsset;
        [SerializeField] private float _minScore;
        [SerializeField] RenderTexture _image;
        [SerializeField] RenderTexture _depth;
        [SerializeField] Texture2D _depthBuffer = null;
        [SerializeField] Transform _site = null;

        int _imageWidth;
        int _imageHeight;
        int _depthWidth;
        int _depthHeight;

        public const int MODEL_IMAGE_SIZE = 256; 

        private OneEuroFilter[] _filter;
        [SerializeField]  private GameObject[] _headspace;

        [SerializeField]  private GameObject _keypointPrefab;
        private Model _runtimeModel;
        private Worker _worker;
        // private IBackend _ops;
        private TextureTransform _textureTransform;
        private CommandBuffer _commandBuffer;
        private RenderTexture _intermediateTexture;
        private RenderTargetIdentifier _intermediatedestinationId;

        private Tensor<float> _inputTensor;


        [SerializeField][Range(0, 5)] private float cutoffFrequency = 0.5f;
        [SerializeField][Range(0, 2)] private float beta = 3f;
        [SerializeField][Range(0, 2)] private float derivativeCutoff = 1f;


        void Start() {
            _runtimeModel = ModelLoader.Load(_modelAsset);
            _worker = new Worker(_runtimeModel, BackendType.GPUCompute);
            _ops = WorkerFactory.CreateBackend(BackendType.GPUCompute, new TensorCachingAllocator());
            _filter = new OneEuroFilter[10];
            _headspace = new GameObject[10];
            for (int i = 0; i < 10; i++) {
                _filter[i] = new OneEuroFilter(cutoffFrequency, beta, derivativeCutoff);
                _headspace[i] = Instantiate(_keypointPrefab, new Vector3(0, -10, 0), Quaternion.identity);

            }
            _textureTransform = new TextureTransform().SetDimensions(MODEL_IMAGE_SIZE, MODEL_IMAGE_SIZE, 3).SetTensorLayout(TensorLayout.NHWC);

            _commandBuffer = new CommandBuffer();
            _commandBuffer.name = "CameraToTensor";

            _intermediateTexture = new RenderTexture(MODEL_IMAGE_SIZE, MODEL_IMAGE_SIZE, 0, RenderTextureFormat.ARGB32);
            _intermediateTexture.enableRandomWrite = true;
            _intermediateTexture.Create();
            _intermediatedestinationId = new RenderTargetIdentifier(intermediateTexture);

            _inputTensor = TensorFloat.Zeros(new TensorShape(1, MODEL_IMAGE_SIZE, MODEL_IMAGE_SIZE, 3), backend);

        }

        void Update () {

            if (_demux.ColorTexture == null || _demux.DepthTexture == null)
                return;

            _imageWidth = _demux.ColorTexture.width;
            _imageHeight = _demux.ColorTexture.height;
            _depthWidth = _demux.DepthTexture.width;
            _depthHeight = _demux.DepthTexture.height;

            _image = _demux.ColorTexture;
            _depth = _demux.DepthTexture;

            // if (_depthBuffer == null)
            // {
            //     _depthBuffer = new Texture2D(_depth.width, _depth.height, TextureFormat.ARGB32, 0, true);
            // }
            

            commandBuffer.Clear();
            commandBuffer.Blit(_image, _intermediatedestinationId);
            commandBuffer.ToTensor(_intermediatedestinationId, inputTensor, _textureTransforme);
            Graphics.ExecuteCommandBuffer(commandBuffer);

            // RenderTexture tempRT = RenderTexture.GetTemporary(_depth.width, _depth.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            // Graphics.Blit(_depth, tempRT);

            // RenderTexture lastActive = RenderTexture.active;
            // RenderTexture.active = tempRT;
            // _depthBuffer.ReadPixels(new Rect(0, 0, _depth.width, _depth.height), 0, 0);
            // _depthBuffer.Apply();
            // RenderTexture.active = lastActive;
            // tempRT.Release();

            // var inputTensor = TextureConverter.ToTensor(_image, _textureTransform);
            // var tensorScaled = _ops.Mul(inputTensor, 255f);
            Inference(tensorScaled);
            inputTensor.Dispose();
            tensorScaled.Dispose();
        }

        void OnDestroy()
        {
            _worker.Dispose();
        }

        // Pose3d Unproject(Pose pose) {
        //     var xScale = (float) imageWidth / depthWidth;
        //     var yScale = (float) imageHeight / depthHeight;
        //     var scale = Mathf.Max(xScale, yScale); // Image is always aspect filled in screen
        //     var xRatio = scale * depthWidth / imageWidth;
        //     var yRatio = scale * depthHeight / imageHeight;
        //     // Transform
        //     var keypoints = new Vector4[pose.Count];
        //     for (var i = 0; i < pose.Count; ++i) {
        //         var keypoint = pose[i];
        //         var scaledKeypoint = new Vector2(xRatio * (keypoint.x - 0.5f) + 0.5f, yRatio * (keypoint.y - 0.5f) + 0.5f);
        //         var worldPoint = Unproject(depthMap, scaledKeypoint);
        //         keypoints[i] = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, keypoint.z);
        //     }
        // }

        //private unsafe T Sample<T>(int x, int y) where T : unmanaged
        //{
        //    var plane = depthBuffer.GetPlane(0);
        //    var idx = y * plane.rowStride + x * plane.pixelStride;
        //    var data = (byte*)plane.data.GetUnsafeReadOnlyPtr();
        //    var sample = *(T*)&data[idx];
        //    return sample;
        //}

        public float Sample(Vector2 point) {
            var s = float2(_depth.width, _depth.height); // use unoriented size
            var uv = float2(point.x, point.y);
            var rotation = 0f;
            var t = Mathf.Deg2Rad * rotation; 
            var T = mul(float2x2(cos(t), -sin(t), sin(t), cos(t)), float2x2(1f, 0f, 0f, -1f));
            var uv_r = mul(T, uv - 0.5f) + 0.5f;
            var xy = int2(uv_r * s);
            if (xy.x < 0 || xy.x >= _image.width || xy.y < 0 || xy.y >= _image.height)
                return -1;

            Debug.Log(string.Format("{0} {1} {2} {3} {4} {5}", point.x, point.y, xy.x, xy.y, _depthBuffer.width, _depthBuffer.height));

            int x = Mathf.FloorToInt(point.x / _image.width * _depthBuffer.width);
            int z = Mathf.FloorToInt(point.y / _image.height * _depthBuffer.height);

            return _depthBuffer.GetPixel((int) point.x, (int)point.y)[0];

            //switch (_image.format) {
            //    case Format.DepthFloat32:   return Sample<float>(xy.x, xy.y);
            //    case Format.DepthUint16:    return 0.001f * Sample<ushort>(xy.x, xy.y);
            //    default:                    throw new InvalidOperationException($"Cannot sample depth because image has invalid format: {image.format}");
            //}
        }

        // Vector3 Unproject(Vector2 point) { 
        //     var depth = Sample(_depth, point);
        //     var viewport = new Vector3(point.x, point.y, depth);
        //     var world = _camera.ViewportToWorldPoint(viewport);
        //     return world;
        // }

        Pose3D ConstructPose3D(Pose pose)
        {
            // Compute scale factor
            var xScale = (float) _imageWidth / _depthWidth;
            var yScale = (float) _imageHeight / _depthHeight;
            var scale = Mathf.Max(xScale, yScale); // Image is always aspect filled in screen
            var xRatio = scale * _depthWidth / _imageWidth;
            var yRatio = scale * _depthHeight / _imageHeight;
            // Transform
            var keypoints = new Vector4[pose.Count];
            for (var i = 0; i < pose.Count; ++i)
            {
                var keypoint = pose[i];
                var scaledKeypoint = new Vector2(xRatio * (keypoint.x - 0.5f) + 0.5f, yRatio * (keypoint.y - 0.5f) + 0.5f);
                //Debug.Log(string.Format("{} {} {} {}", keypoint.x, keypoint.y, scaledKeypoint.x, scaledKeypoint.y));
                //                var worldPoint = Unproject(scaledKeypoint);
                var di = (int)(keypoint.x * _depthWidth);
                var dj = (int)(keypoint.y * _depthHeight); 
                var d = _depthBuffer.GetPixel(di, dj);

                var depth = 4;// d[0];
                var viewport = new Vector3(keypoint.x, keypoint.y, depth);
                var worldPoint = _camera.ViewportToWorldPoint(viewport);

                Debug.Log(string.Format("kx:{0:F2} ky:{1:F2} sx:{2:F2} sy:{3:F2} wpx:{4:F2} wpy:{5:F2} wpz:{6:F2} cx:{7:F4} cy:{8:F4} cz:{9:F4} depth:{10:F4} d:{11} di:{12} dj:{13}",
                        keypoint.x, keypoint.y, scaledKeypoint.x, scaledKeypoint.y, worldPoint.x, worldPoint.y, worldPoint.z,
                        _camera.transform.position.x, _camera.transform.position.y, _camera.transform.position.z, depth, d, di, dj));


                keypoints[i] = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, keypoint.z);
            }
            return new Pose3D(keypoints);
        }

        //public float Sample (Vector2 point) {
        //    var s = float2(image.width, image.height); // use unoriented size
        //    var uv = float2(point.x, point.y);
        //    var t = Mathf.Deg2Rad * rotation;
        //    var T = mul(float2x2(cos(t), -sin(t), sin(t), cos(t)), float2x2(1f, 0f, 0f, -1f));
        //    var uv_r = mul(T, uv - 0.5f) + 0.5f;
        //    var xy = int2(uv_r * s);
        //    if (xy.x < 0 || xy.x >= image.width || xy.y < 0 || xy.y >= image.height)
        //        return -1;
        //    switch (image.format) {
        //        case Format.DepthFloat32:   return Sample<float>(xy.x, xy.y);
        //        case Format.DepthUint16:    return 0.001f * Sample<ushort>(xy.x, xy.y);
        //        default:                    throw new InvalidOperationException($"Cannot sample depth because image has invalid format: {image.format}");
        //    }
        //}

        public Vector3 Unproject(Vector2 point)
        {
            //var depth = Sample(point);
            var depth = 1;
            var viewport = new Vector3(point.x, point.y, depth);
            var world = _camera.ViewportToWorldPoint(viewport);
            return world;
        }

        //#endregion
//        private unsafe T Sample<T>(Texture2D texture, int x, int y) where T : unmanaged
//        {
//            var plane = image.GetPlane(0);
//            var idx = y * plane.rowStride + x * plane.pixelStride;
//            var data = (byte*)plane.data.GetUnsafeReadOnlyPtr();
//            var sample = *(T*)&data[idx];
//            return sample;
//        }



        private void Inference(Tensor input)
        {
            _worker.Schedule(input);

            var keypoints = _worker.PeekOutput() as TensorFloat;
            
            if (keypoints == null) { return; }

            keypoints.MakeReadable();

            var keypointData = keypoints.ToReadOnlyArray();

            //keypointData = _filter?.Filter(keypointData) ?? keypointData;

//            Create poses
            var result = new List<Pose>();
            var result3D = new List<Pose3D>();

            int bodyIndex = 0;
            List<Vector3> trackedBodies = new List<Vector3>();
            for (int i = 0, ilen = keypoints.shape[1], istride = keypoints.shape[2]; i < ilen; ++i)
            {
                var offset = i * istride;
                var pose = new Pose(keypointData, offset);
                if (pose.score >= _minScore)
                {
                    result.Add(pose);

                    var center = pose.nose; //new Vector3(
                                            //Enumerable.Range(0, 10).Select(k => pose[k].x).Average(),
                                            //Enumerable.Range(0, 10).Select(k => pose[k].y).Average(),
                                            //8);

                    //var center = new Vector3(
                    //                        Enumerable.Range(0, 10).Select(k => pose[k].x).Average(),
                    //                        Enumerable.Range(0, 10).Select(k => pose[k].y).Average(),
                    //                        8);

                    //var center = new Vector3(
                    //                       pose.nose.x, pose.nose.y,
                    //                       8);
                    var world3d = _camera.ViewportToWorldPoint(center);


                    //var pose3d = ConstructPose3D(pose);

                    trackedBodies.Add(world3d);

                    float[] keypointXYZ = new float[] { world3d.x, world3d.y, world3d.z};

                    
                    //keypointXYZ = _filter[bodyIndex]?.Filter(keypointXYZ) ?? keypointXYZ;


                    bodyIndex++;
                    //result3D.Add(pose3d);
                }
            }
            if (trackedBodies.Count >= 2)
            {
                if (trackedBodies[0].x > trackedBodies[1].x)
                {
                    Vector3 tmp;
                    tmp = trackedBodies[1];
                    trackedBodies[1] = trackedBodies[0];
                    trackedBodies[0] = tmp;
                }
                if (trackedBodies.Count >= 3)
                {
                    if (trackedBodies[0].x > trackedBodies[2].x)
                    {
                        Vector3 tmp;
                        tmp = trackedBodies[2];
                        trackedBodies[0] = tmp;
                    }
                    if (trackedBodies[1].x > trackedBodies[2].x)
                    {
                        Vector3 tmp;
                        tmp = trackedBodies[2];
                        trackedBodies[2] = tmp;
                    }
                }
                for (int i = 0; i< 3; i++)
                {
                    float[] keypointXYZ = new float[] { trackedBodies[i].x, trackedBodies[i].y, trackedBodies[i].z };

                    keypointXYZ = _filter[i]?.Filter(keypointXYZ) ?? keypointXYZ;
                    _headspace[i].transform.position = new Vector3(keypointXYZ[0], keypointXYZ[1], keypointXYZ[2]);
                    _headspace[i].transform.parent = _site;

                }
            }

            keypoints.Dispose();

            _visualizer.Render(result.ToArray());
            _3dvisualizer.Render(result3D.ToArray());
        }
    }
}