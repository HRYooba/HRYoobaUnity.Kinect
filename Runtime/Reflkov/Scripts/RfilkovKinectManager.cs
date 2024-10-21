using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;
using HRYooba.Kinect.Services;
using HRYooba.Kinect.Repositories;
using HRYooba.Kinect.Presentations.Areas;
using HRYooba.Kinect.Presentations.Kinects;
using HRYooba.Kinect.Presentations.Users;
using com.rfilkov.kinect;

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

        private readonly Dictionary<string, RfilkovKinectService> _rfilkovKinectServices = new();

        public bool Initialized { get; private set; }

        private void Awake()
        {
            _areaDataRepository = new();
            _kinectDataRepository = new();
            _userDataRepository = new();

            _areaDataJsonManager = new(_areaDataRepository);
            _kinectDataJsonManager = new(_kinectDataRepository);
            _userDataMonitor = new(_userDataRepository);
        }

        private void Start()
        {
            var allKinectData = _kinectDataRepository.GetAll();
            foreach (var kinectData in allKinectData)
            {
                var rfilkovKinectService = new RfilkovKinectService(
                    kinectData.Id,
                    _areaDataRepository,
                    _kinectDataRepository,
                    _userDataRepository
                );

                _rfilkovKinectServices.Add(kinectData.Id, rfilkovKinectService);
            }

            if (_rfilkovKinectServices.Count(_ => _.Value.IsValid) == 0)
            {
                OutputSensorListLog();
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

        private void OnApplicationQuit() => OnDestory();

        private void OnDestory()
        {
            _userDataMonitor.Dispose();

            foreach (var rfilkovKinectService in _rfilkovKinectServices.Values)
            {
                rfilkovKinectService.Dispose();
            }
            _rfilkovKinectServices.Clear();
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
            Debug.LogWarning(log);
        }

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

        public UserData[] GetAllPrimaryUserData()
        {
            return _rfilkovKinectServices.Values.Select(k => k.PrimaryUserData).ToArray();
        }

        public UserData GetPrimaryUserData(string kinectId = null)
        {
            {
                if (kinectId == null)
                {
                    var kinectService = _rfilkovKinectServices.Values.FirstOrDefault(_ => _.IsValid);
                    if (kinectService != null)
                    {
                        return kinectService.PrimaryUserData;
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
                    return kinectService.PrimaryUserData;
                }
                else
                {
                    return null;
                }
            }
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
                    var kinectService = _rfilkovKinectServices.Values.FirstOrDefault(_ => _.IsValid);
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
    }
}