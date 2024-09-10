using System;
using UnityEngine;
using R3;

namespace HRYooba.Kinect.Presentations.Areas
{
    public class AreaModel : IDisposable
    {
        public int Id { get; }

        private readonly ReactiveProperty<Vector3> _position = new();
        public ReadOnlyReactiveProperty<Vector3> Position => _position;
        public void SetPosition(Vector3 value) => _position.Value = value;

        private readonly ReactiveProperty<float> _radius = new();
        public ReadOnlyReactiveProperty<float> Radius => _radius;
        public void SetRadius(float value) => _radius.Value = value;

        public AreaModel(int id, Vector3 position, float radius)
        {
            Id = id;
            _position.Value = position;
            _radius.Value = radius;
        }

        public void Dispose()
        {
            _position.Dispose();
            _radius.Dispose();
        }
    }
}