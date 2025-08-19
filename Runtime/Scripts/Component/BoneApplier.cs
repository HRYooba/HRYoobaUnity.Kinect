using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HRYooba.Kinect
{
    public class BoneApplier : MonoBehaviour
    {
        private Animator _animator;
        private Dictionary<HumanBodyBones, Quaternion> _initBoneRotations = new();
        private Quaternion _initRotation;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _initRotation = transform.rotation;
            transform.rotation = Quaternion.identity;

            // 初期姿勢を保存
            foreach (var bone in BoneData.BoneMap.Keys)
            {
                _initBoneRotations[bone] = _animator.GetBoneTransform(bone).rotation;
            }

            transform.rotation = _initRotation;
        }

        private void OnEnable()
        {
            foreach (var bone in BoneData.BoneMap.Keys)
            {
                _animator.GetBoneTransform(bone).rotation = _initRotation * _initBoneRotations[bone];
            }
        }

        public void Apply(BoneData[] allBoneData)
        {
            if (allBoneData == null) return;

            foreach (var pair in BoneData.BoneMap)
            {
                var humanBone = pair.Key;
                var type = pair.Value;

                var boneData = allBoneData.FirstOrDefault(x => x.Type == type);
                if (boneData == null) continue;

                var newRotation = boneData.Rotation * _initBoneRotations[humanBone];
                newRotation = transform.rotation * newRotation;

                var nowRotation = _animator.GetBoneTransform(humanBone).rotation;
                _animator.GetBoneTransform(humanBone).rotation = Quaternion.Lerp(nowRotation, newRotation, Time.deltaTime * 10);
            }
        }

        public class BoneData
        {
            public BoneData(BoneType type, bool isTracking, Vector3 position, Quaternion rotation)
            {
                Type = type;
                IsTracking = isTracking;
                Position = position;
                Rotation = rotation;
            }

            public BoneType Type { get; }
            public bool IsTracking { get; }
            public Vector3 Position { get; }
            public Quaternion Rotation { get; }

            public const int BoneCount = 32;

            public enum BoneType
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
                ThumbRight = 31,
            }

            public static readonly Dictionary<HumanBodyBones, BoneType> BoneMap = new()
            {
                // 上半身
                {HumanBodyBones.Hips, BoneType.Pelvis},
                {HumanBodyBones.Spine, BoneType.SpineNaval},
                {HumanBodyBones.Chest, BoneType.SpineChest},
                {HumanBodyBones.Neck, BoneType.Neck},
                {HumanBodyBones.Head, BoneType.Head},

                // 左腕
                {HumanBodyBones.LeftShoulder, BoneType.ClavicleLeft},
                {HumanBodyBones.LeftUpperArm, BoneType.ShoulderLeft},
                {HumanBodyBones.LeftLowerArm, BoneType.ElbowLeft},
                {HumanBodyBones.LeftHand, BoneType.WristLeft},

                // 右腕
                {HumanBodyBones.RightShoulder, BoneType.ClavicleRight},
                {HumanBodyBones.RightUpperArm, BoneType.ShoulderRight},
                {HumanBodyBones.RightLowerArm, BoneType.ElbowRight},
                {HumanBodyBones.RightHand, BoneType.WristRight},

                // 左脚
                {HumanBodyBones.LeftUpperLeg, BoneType.HipLeft},
                {HumanBodyBones.LeftLowerLeg, BoneType.KneeLeft},
                {HumanBodyBones.LeftFoot, BoneType.AnkleLeft},
                {HumanBodyBones.LeftToes, BoneType.FootLeft},

                // 右脚
                {HumanBodyBones.RightUpperLeg, BoneType.HipRight},
                {HumanBodyBones.RightLowerLeg, BoneType.KneeRight},
                {HumanBodyBones.RightFoot, BoneType.AnkleRight},
                {HumanBodyBones.RightToes, BoneType.FootRight},
            };
        }
    }
}