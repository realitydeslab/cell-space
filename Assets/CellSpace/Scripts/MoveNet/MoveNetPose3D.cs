/*
*   MoveNet 3D
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace RealityDesignLab.MoveNet {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    /// <summary>
    /// Detected 3D body pose.
    /// The xyz coordinates are the world position of the keypoint.
    /// The w coordinate is the confidence score of the keypoint, in range [0, 1].
    /// </summary>
    public readonly struct Pose3D : IReadOnlyList<Vector4> {

        #region --Client API--
        /// <summary>
        /// Number of keypoints in the pose.
        /// </summary>
        public readonly int Count               => keypoints.Length;

        /// <summary>
        /// Nose position.
        /// </summary>
        public readonly Vector4 nose            => this[0];
        
        /// <summary>
        /// Left eye position.
        /// </summary>'
        public readonly Vector4 leftEye         => this[1];

        /// <summary>
        /// Right eye position.
        /// </summary>
        public readonly Vector4 rightEye        => this[2];

        /// <summary>
        /// Left ear position.
        /// </summary>
        public readonly Vector4 leftEar         => this[3];

        /// <summary>
        /// Right ear position.
        /// </summary>
        public readonly Vector4 rightEar        => this[4];

        /// <summary>
        /// Left shoulder position.
        /// </summary>
        public readonly Vector4 leftShoulder    => this[5];

        /// <summary>
        /// Right shoulder position.
        /// </summary>
        public readonly Vector4 rightShoulder   => this[6];

        /// <summary>
        /// Left elbow position.
        /// </summary>
        public readonly Vector4 leftElbow       => this[7];

        /// <summary>
        /// Right elbow position.
        /// </summary>
        public readonly Vector4 rightElbow      => this[8];

        /// <summary>
        /// Left wrist position.
        /// </summary>
        public readonly Vector4 leftWrist       => this[9];

        /// <summary>
        /// Right wrist position.
        /// </summary>
        public readonly Vector4 rightWrist      => this[10];

        /// <summary>
        /// Left hip position.
        /// </summary>
        public readonly Vector4 leftHip         => this[11];

        /// <summary>
        /// Right hip position.
        /// </summary>
        public readonly Vector4 rightHip        => this[12];

        /// <summary>
        /// Left knee position.
        /// </summary>
        public readonly Vector4 leftKnee        => this[13];

        /// <summary>
        /// Right knee position.
        /// </summary>
        public readonly Vector4 rightKnee       => this[14];

        /// <summary>
        /// Left ankle position.
        /// </summary>
        public readonly Vector4 leftAnkle       => this[15];

        /// <summary>
        /// Right ankle position.
        /// </summary>
        public readonly Vector4 rightAnkle      => this[16];

        /// <summary>
        /// Get a pose anchor by index.
        /// </summary>
        /// <param name="idx">Keypoint index. Must be in range [0, 16].</param>
        public readonly Vector4 this [int idx]  => keypoints[idx];
        #endregion


        #region --Operations--
        private readonly Vector4[] keypoints;

        internal Pose3D(Vector4[] keypoints)
        {
            this.keypoints = keypoints;
        }

        IEnumerator<Vector4> IEnumerable<Vector4>.GetEnumerator () {
            for (var i = 0; i < Count; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<Vector4>).GetEnumerator();
        #endregion
    }
}