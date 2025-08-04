using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using com.rfilkov.kinect;
using R3;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;
using HRYooba.Kinect.Services;
using HRYooba.Kinect.Repositories;
using HRYooba.Kinect.Presentations.Areas;
using HRYooba.Kinect.Presentations.Kinects;
using HRYooba.Kinect.Presentations.Users;

namespace HRYooba.Kinect.Rfilkov
{
    [DefaultExecutionOrder(-1)]
    public class RfilkovKinectManager : MonoBehaviour, IKinectManager
    {
        [SerializeField] private AreaInitializer _areaInitializer;
        [SerializeField] private KinectInitializer _kinectInitializer;
        [SerializeField] private UserInitializer _userInitializer;

        private AreaDataRepository _areaDataRepository;
        private KinectDataRepository _kinectDataRepository;
        private UserDataRepository _userDataRepository;

        private AreaDataJsonManager _areaDataJsonManager;
        private KinectDataJsonManager _kinectDataJsonManager;
        private UserDataMonitor _userDataMonitor;

        private KinectManager _kinectManager;
        private readonly Dictionary<string, RfilkovKinectService> _rfilkovKinectServices = new();
        private readonly UserData.IdComparer _userDataIdComparer = new();

        public bool Initialized { get; private set; }

        #region Unity Events
        private void Awake()
        {
            _areaDataRepository = new();
            _kinectDataRepository = new();
            _userDataRepository = new();

            _areaDataJsonManager = new(_areaDataRepository);
            _kinectDataJsonManager = new(_kinectDataRepository);
            _userDataMonitor = new(_userDataRepository);

            Observable.EveryUpdate(UnityFrameProvider.PreUpdate).Subscribe(_ => PreUpdate()).AddTo(this);
            Observable.EveryUpdate(UnityFrameProvider.PreLateUpdate).Subscribe(_ => PreLateUpdate()).AddTo(this);
        }

        private void Start()
        {
            _kinectManager = KinectManager.Instance;
            OutputSensorListLog();

            var allKinectData = _kinectDataRepository.GetAll();
            foreach (var kinectData in allKinectData)
            {
                var kinectId = kinectData.Id;
                if (!TryGetSensorData(kinectId, out var sensorData))
                {
                    Debug.LogWarning($"[RfilkovKinectManager] SensorData not found. SensorId: {kinectId}");
                    _kinectDataRepository.Remove(kinectId);
                    continue;
                }

                var rfilkovKinectService = new RfilkovKinectService(sensorData, kinectData.BodyTrackingSensorOrientation);
                _rfilkovKinectServices.Add(kinectData.Id, rfilkovKinectService);
            }

            _areaInitializer.Initialize(
                _areaDataRepository,
                _areaDataJsonManager,
                _areaDataJsonManager
            );

            _kinectInitializer.Initialize(
                _kinectDataRepository,
                _kinectDataJsonManager,
                _kinectDataJsonManager,
                _rfilkovKinectServices.ToDictionary(k => k.Key, v => (IKinectService)v.Value)
            );

            _userInitializer.Initialize(
                _userDataMonitor
            );

            Initialized = true;
        }

        private void OnDestory()
        {
            _userDataMonitor.Dispose();

            foreach (var rfilkovKinectService in _rfilkovKinectServices.Values)
            {
                rfilkovKinectService.Dispose();
            }
            _rfilkovKinectServices.Clear();
        }

        private void OnApplicationQuit() => OnDestory();

        private void PreUpdate()
        {
            foreach (var rfilkovKinectService in _rfilkovKinectServices.Values)
            {
                rfilkovKinectService.PreUpdate();
            }
        }

        private void Update()
        {
            UpdateUserDataRepository();

            foreach (var pair in _rfilkovKinectServices)
            {
                var kinectId = pair.Key;
                var rfilkovKinectService = pair.Value;
                var kinectData = _kinectDataRepository.Get(kinectId);
                var primaryUserId = GetUserData(kinectData.PrimaryUserAreaId)?.Id ?? 0;

                rfilkovKinectService.ApplyKinectData(kinectData);
                rfilkovKinectService.SetPrimaryUserId(primaryUserId);
            }
        }

        private void PreLateUpdate()
        {
            foreach (var rfilkovKinectService in _rfilkovKinectServices.Values)
            {
                rfilkovKinectService.PreLateUpdate();
            }
        }
        #endregion

        #region IKinectManager Implementation
        public UserData[] GetAllUserData()
        {
            return _userDataRepository.GetAll().ToArray();
        }

        public AreaData[] GetAllAreaData()
        {
            return _areaDataRepository.GetAll().ToArray();
        }

        public KinectData[] GetAllKinectData()
        {
            return _kinectDataRepository.GetAll().ToArray();
        }

        public UserData GetUserData(int areaId)
        {
            var areaData = _areaDataRepository.Get(areaId);
            if (areaData == null) return null;
            var areaPos = new Vector2(areaData.Position.x, areaData.Position.z);

            var allUserData = _userDataRepository.GetAll();
            var areaInUserData = new List<UserData>();
            foreach (var userData in allUserData)
            {
                var userPos = new Vector2(userData.Position.x, userData.Position.z);
                if (Vector2.Distance(areaPos, userPos) < areaData.Radius)
                {
                    areaInUserData.Add(userData);
                }
            }
            if (areaInUserData.Count == 0) return null;

            // area posから一番近い人を返す
            var closestUserData = areaInUserData
                .OrderBy(user => Vector2.Distance(areaPos, new Vector2(user.Position.x, user.Position.z)))
                .FirstOrDefault();

            return closestUserData;
        }

        public ProvideTextures[] GetAllKinectTextures()
        {
            return _rfilkovKinectServices.Values
                .Select(k => new ProvideTextures(
                    k.ColorTexture,
                    k.DepthTexture,
                    k.DepthMappedColorTexture,
                    k.BodyIndexTexture,
                    k.PrimaryUserTexture,
                    k.PointCloudTexture
                ))
                .ToArray();
        }

        public ProvideTextures GetKinectTextures(string kinectId = null)
        {
            {
                if (kinectId == null)
                {
                    var kinectService = _rfilkovKinectServices.Values.FirstOrDefault();
                    if (kinectService != null)
                    {
                        return new ProvideTextures(
                            kinectService.ColorTexture,
                            kinectService.DepthTexture,
                            kinectService.DepthMappedColorTexture,
                            kinectService.BodyIndexTexture,
                            kinectService.PrimaryUserTexture,
                            kinectService.PointCloudTexture
                        );
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            {
                if (_rfilkovKinectServices.TryGetValue(kinectId, out var kinectService))
                {
                    return new ProvideTextures(
                        kinectService.ColorTexture,
                        kinectService.DepthTexture,
                        kinectService.DepthMappedColorTexture,
                        kinectService.BodyIndexTexture,
                        kinectService.PrimaryUserTexture,
                        kinectService.PointCloudTexture
                    );
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region private methods
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
            log.Append("[RfilkovKinectManager] Find SensorList: ");

            var sensorCount = KinectManager.Instance.GetSensorCount();
            if (sensorCount == 0) return;

            for (int i = 0; i < sensorCount; i++)
            {
                if (i != 0) log.Append(", ");

                var data = KinectManager.Instance.GetSensorData(i);
                log.Append($"Sensor[{i}]:{data.sensorId}");
            }
            Debug.Log(log);
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

        private UserData ConvertToUserData(ulong userId, KinectInterop.BodyData bodyData)
        {
            var id = userId;
            var sensorIndex = _kinectManager.GetUserBodyData(userId).sensorIndex;
            var sensorData = _kinectManager.GetSensorData(sensorIndex);
            var sensorTransform = sensorData.sensorInterface.GetSensorTransform();

            var position = _kinectManager.GetUserKinectPosition(userId, true);
            position.x *= -1;
            position = sensorTransform.TransformPoint(position);

            var rotation = _kinectManager.GetUserOrientation(userId, false);
            rotation = new Quaternion(rotation.x, rotation.y, -rotation.z, -rotation.w);
            rotation = sensorTransform.rotation * rotation * Quaternion.Euler(0, 180, 0);
            rotation = Quaternion.Euler(0, 180, 0) * rotation;

            var joints = bodyData.joint.Select(data => ConvertToJointData(userId, sensorData, data)).ToArray();

            if (bodyData.liTrackingID != userId)
            {
                id = 0;
            }

            return new UserData(id, position, rotation, joints);
        }

        private JointData ConvertToJointData(ulong userId, KinectInterop.SensorData sensorData, KinectInterop.JointData jointData)
        {
            var sensorTransform = sensorData.sensorInterface.GetSensorTransform();

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
        #endregion
    }
}