using System.Collections.Generic;
using UnityEngine;

namespace HRYooba.Kinect.Core
{
    public class UserData
    {
        public UserData(ulong id, Vector3 position, Quaternion rotation, JointData[] joints)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Joints = joints;
        }

        public ulong Id { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public JointData[] Joints { get; }

        public class IdComparer : IEqualityComparer<UserData>
        {
            public bool Equals(UserData x, UserData y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(UserData obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}