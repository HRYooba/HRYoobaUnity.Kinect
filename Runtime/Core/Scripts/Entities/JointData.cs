using System.Collections.Generic;
using UnityEngine;

namespace HRYooba.Kinect.Core
{
    public class JointData
    {
        public JointData(JointType type, bool isTracking, Vector3 position, Quaternion rotation, Quaternion mirrorRotation)
        {
            Type = type;
            MirrorType = GetMirrorJointType(type);
            IsTracking = isTracking;
            Position = position;
            Rotation = rotation;
            MirrorRotation = mirrorRotation;
        }

        public JointType Type { get; }
        public JointType MirrorType { get; }
        public bool IsTracking { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Quaternion MirrorRotation { get; }

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

        public static JointType GetMirrorJointType(JointType type)
        {
            switch (type)
            {
                case JointType.ClavicleLeft: return JointType.ClavicleRight;
                case JointType.ShoulderLeft: return JointType.ShoulderRight;
                case JointType.ElbowLeft: return JointType.ElbowRight;
                case JointType.WristLeft: return JointType.WristRight;
                case JointType.HandLeft: return JointType.HandRight;

                case JointType.ClavicleRight: return JointType.ClavicleLeft;
                case JointType.ShoulderRight: return JointType.ShoulderLeft;
                case JointType.ElbowRight: return JointType.ElbowLeft;
                case JointType.WristRight: return JointType.WristLeft;
                case JointType.HandRight: return JointType.HandLeft;

                case JointType.HipLeft: return JointType.HipRight;
                case JointType.KneeLeft: return JointType.KneeRight;
                case JointType.AnkleLeft: return JointType.AnkleRight;
                case JointType.FootLeft: return JointType.FootRight;

                case JointType.HipRight: return JointType.HipLeft;
                case JointType.KneeRight: return JointType.KneeLeft;
                case JointType.AnkleRight: return JointType.AnkleLeft;
                case JointType.FootRight: return JointType.FootLeft;

                case JointType.EyeLeft: return JointType.EyeRight;
                case JointType.EarLeft: return JointType.EarRight;
                case JointType.EyeRight: return JointType.EyeLeft;
                case JointType.EarRight: return JointType.EarLeft;

                case JointType.HandtipLeft: return JointType.HandtipRight;
                case JointType.ThumbLeft: return JointType.ThumbRight;
                case JointType.HandtipRight: return JointType.HandtipLeft;
                case JointType.ThumbRight: return JointType.ThumbLeft;

                default: return type;
            }
        }

        public static readonly Dictionary<HumanBodyBones, JointType> JointMap = new()
        {
            // 上半身
            {HumanBodyBones.Hips, JointType.Pelvis},
            {HumanBodyBones.Spine, JointType.SpineNaval},
            {HumanBodyBones.Chest, JointType.SpineChest},
            {HumanBodyBones.Neck, JointType.Neck},
            {HumanBodyBones.Head, JointType.Head},

            // 左腕
            {HumanBodyBones.LeftShoulder, JointType.ClavicleLeft},
            {HumanBodyBones.LeftUpperArm, JointType.ShoulderLeft},
            {HumanBodyBones.LeftLowerArm, JointType.ElbowLeft},
            {HumanBodyBones.LeftHand, JointType.WristLeft},

            // 右腕
            {HumanBodyBones.RightShoulder, JointType.ClavicleRight},
            {HumanBodyBones.RightUpperArm, JointType.ShoulderRight},
            {HumanBodyBones.RightLowerArm, JointType.ElbowRight},
            {HumanBodyBones.RightHand, JointType.WristRight},

            // 左脚
            {HumanBodyBones.LeftUpperLeg, JointType.HipLeft},
            {HumanBodyBones.LeftLowerLeg, JointType.KneeLeft},
            {HumanBodyBones.LeftFoot, JointType.AnkleLeft},
            {HumanBodyBones.LeftToes, JointType.FootLeft},

            // 右脚
            {HumanBodyBones.RightUpperLeg, JointType.HipRight},
            {HumanBodyBones.RightLowerLeg, JointType.KneeRight},
            {HumanBodyBones.RightFoot, JointType.AnkleRight},
            {HumanBodyBones.RightToes, JointType.FootRight},
        };
    }
}