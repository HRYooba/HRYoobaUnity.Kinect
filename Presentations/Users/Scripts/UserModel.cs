using System;
using UnityEngine;
using R3;

namespace HRYooba.Kinect.Presentations.Users
{
    public class UserModel : IDisposable
    {
        public ulong Id { get; }

        private readonly ReactiveProperty<Vector3> _position = new();
        public ReadOnlyReactiveProperty<Vector3> Position => _position;
        public void SetPosition(Vector3 value) => _position.Value = value;

        private readonly ReactiveProperty<Quaternion> _rotation = new();
        public ReadOnlyReactiveProperty<Quaternion> Rotation => _rotation;
        public void SetRotation(Quaternion value) => _rotation.Value = value;

        private readonly ReactiveProperty<(bool IsTracking, Vector3 Position, Quaternion Rotation)[]> _joints = new();
        public ReadOnlyReactiveProperty<(bool IsTracking, Vector3 Position, Quaternion Rotation)[]> Joints => _joints;
        public void SetJoints((bool IsTracking, Vector3 Position, Quaternion Rotation)[] value) => _joints.Value = value;

        public UserModel(ulong id, Vector3 position, Quaternion rotation, (bool IsTracking, Vector3 Position, Quaternion Rotation)[] joints)
        {
            Id = id;
            _position.Value = position;
            _rotation.Value = rotation;
            _joints.Value = joints;
        }

        public void Dispose()
        {
            _position.Dispose();
            _rotation.Dispose();
            _joints.Dispose();
        }
    }
}