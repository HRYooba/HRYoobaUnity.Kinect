using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        private readonly Dictionary<string, RfilkovKinectService> _rfilkovKinectServices = new();

        public bool IsValid { get; private set; }

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

            IsValid = true;
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
            var kinectService = kinectId == null ? _rfilkovKinectServices.Values.First(k => k.IsValid) : _rfilkovKinectServices[kinectId];

            return kinectService.PrimaryUserData;
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
            var kinectService = kinectId == null ? _rfilkovKinectServices.Values.First(k => k.IsValid) : _rfilkovKinectServices[kinectId];

            return new ProvideTextures(
                kinectService.ColorTexture,
                kinectService.DepthTexture,
                kinectService.DepthMappedColorTexture,
                kinectService.BodyIndexTexture,
                kinectService.PrimaryUserTexture,
                kinectService.PointCloudTexture
            );
        }
    }
}