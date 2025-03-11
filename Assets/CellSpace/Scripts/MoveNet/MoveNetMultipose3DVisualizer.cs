namespace RealityDesignLab.MoveNet {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

    /// <summary>
    /// MoveNet multi-pose visualizer.
    /// This visualizer uses visualizes the pose keypoints overlaid on a UI panel.
    /// </summary>
    public sealed class MoveNetMultipose3DVisualizer : MonoBehaviour {

        #region --Inspector--
        [SerializeField] Transform keypointPrefab;
        [SerializeField] LineRenderer bonePrefab;
        #endregion


        #region --Client API--
        /// <summary>
        /// Render detected poses.
        /// </summary>
        /// <param name="poses">Body poses to render.</param>
        public void Render (params Pose3D[] poses) {
            // Delete current
            foreach (var currentSkeleton in currentSkeletons)
                foreach (var point in currentSkeleton)
                    GameObject.Destroy(point.gameObject);
            currentSkeletons.Clear();

            // Visualize
            foreach (var pose in poses) {

                var currentSkeleton = new List<Transform>();
                // Instantiate keypoints
                for (var i = 0; i < 1; ++i) {
                    
                   var point = Instantiate(keypointPrefab, (Vector3) pose[i], Quaternion.identity, transform);
                   point.gameObject.SetActive(true);
                   currentSkeleton.Add(point);
                }
                currentSkeletons.Add(currentSkeleton);


                //foreach (var positions in new [] {
                //   new [] { pose.leftShoulder, pose.rightShoulder },
                //   new [] { pose.leftShoulder, pose.leftElbow, pose.leftWrist },
                //   new [] { pose.rightShoulder, pose.rightElbow, pose.rightWrist },
                //   new [] { pose.leftShoulder, pose.leftHip },
                //   new [] { pose.rightShoulder, pose.rightHip },
                //   new [] { pose.leftHip, pose.rightHip },
                //   new [] { pose.leftHip, pose.leftKnee, pose.leftAnkle },
                //   new [] { pose.rightHip, pose.rightKnee, pose.rightAnkle }
                //}) {
                //   var bone = Instantiate(bonePrefab, transform.position, Quaternion.identity, transform);
                //   bone.gameObject.SetActive(true);
                //   bone.positionCount = positions.Length;
                //   bone.SetPositions(positions.Select(v => (Vector3)v).ToArray());
                //   currentSkeleton.Add(bone.transform);
                //};
            }
        }
        #endregion


        #region --Operations--
        private readonly List<RectTransform> currentKeypoints = new List<RectTransform>();
        readonly List<List<Transform>> currentSkeletons = new List<List<Transform>>();
        #endregion
    }
}