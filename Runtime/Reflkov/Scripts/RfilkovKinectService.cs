using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using R3;
using com.rfilkov.kinect;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Rfilkov
{
    public class RfilkovKinectService : IKinectService, IDisposable
    {
        // rfilkov assets
        private readonly KinectManager _kinectManager;
        private readonly KinectInterop.SensorData _sensorData;
        private readonly DepthSensorBase _depthSensor;

        // repository
        private readonly IAreaDataRepository _areaDataRepository;
        private readonly IKinectDataRepository _kinectDataRepository;
        private readonly IUserDataRepository _userDataRepository;

        // private fields
        private readonly UserData.IdComparer _userDataIdComparer = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly BodyByBodyIndexTexture _bodyByBodyIndexTexture;
        private readonly RenderTexture _pointCloudTexture;
        private UserData _primaryUserData;
        private ulong _lastBodyImageTime;

        // public properties
        public Texture ColorTexture => _sensorData?.colorImageTexture;
        public Texture DepthTexture => _sensorData?.depthImageTexture;
        public Texture DepthMappedColorTexture => _sensorData?.depthCamColorImageTexture;
        public Texture BodyIndexTexture => _sensorData?.bodyImageTexture;
        public Texture PrimaryUserTexture => _bodyByBodyIndexTexture?.Texture;
        public Texture PointCloudTexture => _depthSensor?.pointCloudVertexTexture;
        public UserData PrimaryUserData => _primaryUserData;
        public bool IsValid => _sensorData != null;

        public RfilkovKinectService(
            string kinectId,
            IAreaDataRepository areaDataRepository,
            IKinectDataRepository kinectDataRepository,
            IUserDataRepository userDataRepository)
        {
            _kinectManager = KinectManager.Instance;
            if (!TryGetSensorData(kinectId, out var sensorData))
            {
                OutputSensorListLog();
                Debug.LogError($"[RfilkovKinectService] SensorData not found. SensorId: {kinectId}");
                return;
            }
            _sensorData = sensorData;
            _depthSensor = (DepthSensorBase)_sensorData.sensorInterface;

            _areaDataRepository = areaDataRepository;
            _kinectDataRepository = kinectDataRepository;
            _userDataRepository = userDataRepository;

            _bodyByBodyIndexTexture = new BodyByBodyIndexTexture(
                _sensorData.depthImageWidth,
                _sensorData.depthImageHeight,
                _depthSensor.minDepthDistance,
                _depthSensor.maxDepthDistance
            );

            _pointCloudTexture = new RenderTexture(
                _sensorData.depthImageWidth,
                _sensorData.depthImageHeight,
                0,
                RenderTextureFormat.ARGBFloat
            );

            var kinectData = _kinectDataRepository.Get(kinectId);

            // rfilkov.kinectの設定
            var kinectSensor = (Kinect4AzureInterface)_depthSensor;
            kinectSensor.StopBodyTracking(_sensorData);
            kinectSensor.bodyTrackingSensorOrientation = kinectData.BodyTrackingSensorOrientation switch
            {
                BodyTrackingSensorOrientationType.Default => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_DEFAULT,
                BodyTrackingSensorOrientationType.Clockwise90 => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_CLOCKWISE90,
                BodyTrackingSensorOrientationType.CounterClockwise90 => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_COUNTERCLOCKWISE90,
                BodyTrackingSensorOrientationType.Flip180 => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_FLIP180,
                _ => Microsoft.Azure.Kinect.Sensor.k4abt_sensor_orientation_t.K4ABT_SENSOR_ORIENTATION_DEFAULT
            };
            kinectSensor.InitBodyTracking(kinectSensor.frameSourceFlags, _sensorData, kinectSensor.coordMapperCalib, true);
            _depthSensor.EnableDepthCameraColorFrame(_sensorData, true); // depthCamColorImageTexture, pointCloudTextureを有効にするために必要
            _depthSensor.pointCloudVertexTexture = _pointCloudTexture;

            // Update
            Observable.EveryUpdate(UnityFrameProvider.PreUpdate).Subscribe(_ => PreUpdate()).AddTo(_disposables);
            Observable.EveryUpdate(UnityFrameProvider.Update).Subscribe(_ => Update()).AddTo(_disposables);
            Observable.EveryUpdate(UnityFrameProvider.PreLateUpdate).Subscribe(_ => PreLateUpdate()).AddTo(_disposables);
        }

        public void Dispose()
        {
            if (_disposables.IsDisposed) return;

            _disposables.Dispose();
            _bodyByBodyIndexTexture?.Dispose();

            _pointCloudTexture?.Release();
            UnityEngine.Object.Destroy(_pointCloudTexture);
        }

        private bool TryGetSensorData(string id, out KinectInterop.SensorData sensorData)
        {
            var sensorCount = _kinectManager.GetSensorCount();
            for (int i = 0; i < sensorCount; i++)
            {
                var data = _kinectManager.GetSensorData(i);
                if (data.sensorId == id)
                {
                    sensorData = data;
                    return true;
                }
            }

            sensorData = null;
            return false;
        }

        private void OutputSensorListLog()
        {
            var log = new StringBuilder();
            log.Append("[RfilkovKinectService] Find SensorList: ");

            var sensorCount = _kinectManager.GetSensorCount();
            if (sensorCount == 0) throw new InvalidOperationException("[RfilkovKinectService] Sensor not found.");

            for (int i = 0; i < sensorCount; i++)
            {
                if (i != 0) log.Append(", ");

                var data = _kinectManager.GetSensorData(i);
                log.Append($"Sensor[{i}]:{data.sensorId}");
            }
            Debug.LogWarning(log);
        }

        private void PreUpdate()
        {
            _primaryUserData = null;
            _lastBodyImageTime = _sensorData.lastBodyImageTime;
        }

        private void Update()
        {
            ApplyKinectData();
            UpdateUserDataRepository();
            UpdatePrimaryUserData();
        }

        private void PreLateUpdate()
        {
            UpdateBodyByBodyIndexTexture();
        }

        private void ApplyKinectData()
        {
            var sensorId = _sensorData.sensorId;
            var kinectData = _kinectDataRepository.Get(sensorId);

            var position = kinectData.Position;
            var rotation = Quaternion.Euler(kinectData.EulerAngles);
            var minDepthDistance = kinectData.MinDepthDistance;
            var maxDepthDistance = kinectData.MaxDepthDistance;

            _sensorData.sensorInterface.SetSensorToWorldMatrix(position, rotation, true);
            _depthSensor.minDepthDistance = minDepthDistance;
            _depthSensor.maxDepthDistance = maxDepthDistance;

            _bodyByBodyIndexTexture.SetDepthDistance(minDepthDistance, maxDepthDistance);
        }

        private void UpdateUserDataRepository()
        {
            var userIds = _kinectManager.GetAllUserIds();
            var currentUserData = userIds.Select(id => ConvertToUserData(id, _kinectManager.GetUserBodyData(id))).Where(_ => _.Id != 0).ToArray();
            var lastUserData = _userDataRepository.GetAll();
            var newUserData = currentUserData.Except(lastUserData, _userDataIdComparer).ToArray();
            var removedUserData = lastUserData.Except(currentUserData, _userDataIdComparer).ToArray();

            foreach (var userData in newUserData)
            {
                _userDataRepository.Add(userData);
            }

            foreach (var userData in removedUserData)
            {
                _userDataRepository.Remove(userData.Id);
            }

            foreach (var userData in currentUserData)
            {
                _userDataRepository.Update(userData);
            }
        }

        private void UpdatePrimaryUserData()
        {
            var getUserData = GetPrimaryUserData();
            if (!Equals(getUserData, default(UserData)))
            {
                _primaryUserData = getUserData;
            }
        }

        private void UpdateBodyByBodyIndexTexture()
        {
            if (_lastBodyImageTime != _sensorData.lastBodyImageTime) // BodyImageが更新されたら
            {
                var userId = _primaryUserData?.Id ?? 0;
                var bodyIndex = _kinectManager.GetBodyIndexByUserId(userId);
                _bodyByBodyIndexTexture.Update(bodyIndex, _sensorData.bodyIndexBuffer);
            }
        }

        private UserData GetPrimaryUserData()
        {
            var allUserData = _userDataRepository.GetAll();
            var allAreaData = _areaDataRepository.GetAll();

            // エリア内にいるユーザーを取得
            var areaInUserData = new List<UserData>();
            foreach (var userData in allUserData)
            {
                var userPos = new Vector2(userData.Position.x, userData.Position.z);

                foreach (var areaData in allAreaData)
                {
                    var areaPos = new Vector2(areaData.Position.x, areaData.Position.z);
                    if (Vector2.Distance(userPos, areaPos) < areaData.Radius)
                    {
                        areaInUserData.Add(userData);
                        break;
                    }
                }
            }

            // エリア内にいるユーザーの中で最もセンサーに近いユーザーを取得
            var kinectData = _kinectDataRepository.Get(_sensorData.sensorId);
            var kinectPos = new Vector2(kinectData.Position.x, kinectData.Position.z);
            var primaryUser = areaInUserData
                .OrderBy(userData => Vector2.Distance(new Vector2(userData.Position.x, userData.Position.z), kinectPos))
                .FirstOrDefault();

            return primaryUser;
        }

        private UserData ConvertToUserData(ulong userId, KinectInterop.BodyData bodyData)
        {
            var id = userId;
            var sensorTransform = _sensorData.sensorInterface.GetSensorTransform();

            var position = _kinectManager.GetUserKinectPosition(userId, true);
            position.x *= -1;
            position = sensorTransform.TransformPoint(position);

            var rotation = _kinectManager.GetUserOrientation(userId, false);
            rotation = new Quaternion(rotation.x, rotation.y, -rotation.z, -rotation.w);
            rotation = sensorTransform.rotation * rotation * Quaternion.Euler(0, 180, 0);
            rotation = Quaternion.Euler(0, 180, 0) * rotation;

            var joints = bodyData.joint.Select(data => ConvertToJointData(userId, data)).ToArray();

            if (bodyData.liTrackingID != userId)
            {
                id = 0;
            }

            return new UserData(id, position, rotation, joints);
        }

        private JointData ConvertToJointData(ulong userId, KinectInterop.JointData jointData)
        {
            var sensorTransform = _sensorData.sensorInterface.GetSensorTransform();

            var isTracked = _kinectManager.IsJointTracked(userId, jointData.jointType);

            var joint = jointData.jointType;
            var position = _kinectManager.GetJointKinectPosition(userId, joint, true);
            position.x *= -1;
            position = sensorTransform.TransformPoint(position);

            var rotation = _kinectManager.GetJointOrientation(userId, joint, false);
            var mirrorRotation = rotation;

            rotation = new Quaternion(rotation.x, rotation.y, -rotation.z, -rotation.w);
            rotation = sensorTransform.rotation * rotation * Quaternion.Euler(0, 180, 0);
            rotation = Quaternion.Euler(0, 180, 0) * rotation;

            mirrorRotation = Quaternion.Euler(0, 180, 0) * mirrorRotation;

            return new JointData((JointData.JointType)joint, isTracked, position, rotation, mirrorRotation);
        }

        private class BodyByBodyIndexTexture : IDisposable
        {
            private RenderTexture _texture;
            public Texture Texture => _texture;

            private Material _material;
            private float _minDepthDistance;
            private float _maxDepthDistance;

            public BodyByBodyIndexTexture(int width, int height, float minDepthDistance, float maxDepthDistance)
            {
                _minDepthDistance = minDepthDistance;
                _maxDepthDistance = maxDepthDistance;

                _texture = new RenderTexture(width, height, 0, RenderTextureFormat.Default);

                var shader = Shader.Find("HRYooba/Kinect/UserBodyImageByBodyIndexShader");
                _material = new Material(shader);
            }

            public void Dispose()
            {
                if (_texture != null)
                {
                    UnityEngine.Object.Destroy(_texture);
                    _texture = null;
                }

                if (_material != null)
                {
                    UnityEngine.Object.Destroy(_material);
                    _material = null;
                }
            }

            public void SetDepthDistance(float minDepthDistance, float maxDepthDistance)
            {
                _minDepthDistance = minDepthDistance;
                _maxDepthDistance = maxDepthDistance;
            }

            public void Update(int bodyIndex, ComputeBuffer bodyIndexBuffer)
            {
                _material.SetBuffer("_BodyIndexMap", bodyIndexBuffer);
                _material.SetInt("_BodyIndex", bodyIndex);
                _material.SetInt("_TexResX", _texture.width);
                _material.SetInt("_TexResY", _texture.height);
                _material.SetInt("_MinDepth", (int)(_minDepthDistance * 1000f));
                _material.SetInt("_MaxDepth", (int)(_maxDepthDistance * 1000f));

                Graphics.Blit(null, _texture, _material);
            }
        }
    }
}