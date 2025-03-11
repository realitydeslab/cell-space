using UnityEngine;
using Unity.Sentis;
using Bibcam.Decoder;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace RealityDesignLab.MoveNet
{
    public sealed class RTMOMultipose3D : MonoBehaviour {
        [SerializeField] private BibcamMetadataDecoder _decoder = null;
        [SerializeField] private BibcamTextureDemuxer _demux = null;

        [SerializeField] private Camera _camera;
        [SerializeField] private MoveNetMultiposeVisualizer _visualizer = null;

        [SerializeField] private ModelAsset _modelAsset;
        [SerializeField] private float _minScore;

        int _imageWidth;
        int _imageHeight;
        int _depthWidth;
        int _depthHeight;

        public const int MODEL_IMAGE_SIZE = 640; 

        private OneEuroFilter _filter;
        private Model _runtimeModel;
        private Worker _worker;
        // private IBackend _ops;
        private TextureTransform _textureTransform;

        void Start () {
            _runtimeModel = ModelLoader.Load(_modelAsset);
            _worker = new Worker(_runtimeModel, BackendType.GPUCompute);
            _ops = WorkerFactory.CreateBackend(BackendType.GPUCompute, new TensorCachingAllocator());
            _filter = new OneEuroFilter(0.5f, 3f, 1f);
            _textureTransform = new TextureTransform().SetDimensions(MODEL_IMAGE_SIZE, MODEL_IMAGE_SIZE, 3).SetTensorLayout(TensorLayout.NCHW);
        }

        void Update () {

            if (_demux.ColorTexture == null || _demux.DepthTexture == null)
                return;

            _imageWidth = _demux.ColorTexture.width;
            _imageHeight = _demux.ColorTexture.height;
            _depthWidth = _demux.DepthTexture.width;
            _depthHeight = _demux.DepthTexture.height;
            var inputTensor = TextureConverter.ToTensor(_demux.ColorTexture, _textureTransform);
            var tensorScaled = _ops.Mul(inputTensor, 255f);
            Inference(tensorScaled);
            inputTensor.Dispose();
            tensorScaled.Dispose();
        }

        void OnDestroy()
        {
            _worker.Dispose();
        }

        private void Inference(Tensor input)
        {
            _worker.Execute(input);

            var dets = _worker.PeekOutput("dets") as Tensor<float>;
            var keypoints = _worker.PeekOutput("keypoints") as Tensor<float>;
            Debug.Log(dets.shape);
            Debug.Log(keypoints.shape);

           if (keypoints == null) { return; }

           dets.MakeReadable();
           keypoints.MakeReadable();

           var detData = dets.ToReadOnlyArray();
           var keypointData = keypoints.ToReadOnlyArray();

//           // keypointData = _filter?.Filter(keypointData) ?? keypointData;

// //            Create poses
//            var result = new List<Pose>();
//            for (int i = 0, ilen = keypoints.shape[1], istride = keypoints.shape[2]; i < ilen; ++i)
//            {
//                var offset = i * istride;
//                var pose = new Pose(keypointData, offset);
//                if (pose.score >= _minScore)
//                {
//                    result.Add(pose);
//                }
//            }

//            keypoints.Dispose();

//            _visualizer.Render(result.ToArray());
        }
    }
}