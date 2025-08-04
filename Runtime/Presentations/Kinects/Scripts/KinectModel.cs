using System;
using UnityEngine;
using R3;

namespace HRYooba.Kinect.Presentations.Kinects
{
    public class KinectModel : IDisposable
    {
        public string Id { get; }
        public int BodyTrackingSensorOrientation { get; }
        public int PrimaryUserAreaId { get; }

        private readonly ReactiveProperty<Vector3> _position = new();
        public ReadOnlyReactiveProperty<Vector3> Position => _position;
        public void SetPosition(Vector3 value) => _position.Value = value;

        private readonly ReactiveProperty<Vector3> _eulerAngles = new();
        public ReadOnlyReactiveProperty<Vector3> EulerAngles => _eulerAngles;
        public void SetEluerAngles(Vector3 value) => _eulerAngles.Value = value;

        private readonly ReactiveProperty<float> _minDepthDistance = new();
        public ReadOnlyReactiveProperty<float> MinDepthDistance => _minDepthDistance;
        public void SetMinDepthDistance(float value) => _minDepthDistance.Value = value;

        private readonly ReactiveProperty<float> _maxDepthDistance = new();
        public ReadOnlyReactiveProperty<float> MaxDepthDistance => _maxDepthDistance;
        public void SetMaxDepthDistance(float value) => _maxDepthDistance.Value = value;

        private readonly ReactiveProperty<bool> _isActivePointCloud = new();
        public ReadOnlyReactiveProperty<bool> IsActivePointCloud => _isActivePointCloud;
        public void SetIsActivePointCloud(bool value) => _isActivePointCloud.Value = value;

        public KinectModel(
            string id,
            Vector3 position,
            Vector3 eulerAngles,
            float minDepthDistance,
            float maxDepthDistance,
            int bodyTrackingSensorOrientation,
            int primaryUserAreaId)
        {
            Id = id;
            _position.Value = position;
            _eulerAngles.Value = eulerAngles;
            _minDepthDistance.Value = minDepthDistance;
            _maxDepthDistance.Value = maxDepthDistance;
            BodyTrackingSensorOrientation = bodyTrackingSensorOrientation;
            PrimaryUserAreaId = primaryUserAreaId;
        }

        public void Dispose()
        {
            _position.Dispose();
            _eulerAngles.Dispose();
            _minDepthDistance.Dispose();
            _maxDepthDistance.Dispose();
            _isActivePointCloud.Dispose();
        }
    }
}