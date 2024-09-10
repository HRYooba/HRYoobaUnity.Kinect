using UnityEngine;

namespace HRYooba.Kinect.Core
{
    public class JointData
    {
        public JointData(JointType type, bool isTracking, Vector3 position, Quaternion rotation)
        {
            Type = type;
            IsTracking = isTracking;
            Position = position;
            Rotation = rotation;
        }

        public JointType Type { get; }
        public bool IsTracking { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public static int JointCount = 32;
        public enum JointType
        {
            Pelvis = 0,
            SpineNaval = 1,
            SpineChest = 2,
            Neck = 3,
            Head = 4,

            ClavicleLeft = 5,
            ShoulderLeft = 6,
            ElbowLeft = 7,
            WristLeft = 8,
            HandLeft = 9,

            ClavicleRight = 10,
            ShoulderRight = 11,
            ElbowRight = 12,
            WristRight = 13,
            HandRight = 14,

            HipLeft = 15,
            KneeLeft = 16,
            AnkleLeft = 17,
            FootLeft = 18,

            HipRight = 19,
            KneeRight = 20,
            AnkleRight = 21,
            FootRight = 22,

            Nose = 23,
            EyeLeft = 24,
            EarLeft = 25,
            EyeRight = 26,
            EarRight = 27,

            HandtipLeft = 28,
            ThumbLeft = 29,
            HandtipRight = 30,
            ThumbRight = 31
        }
    }
}