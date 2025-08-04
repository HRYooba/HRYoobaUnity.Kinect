using System.Collections.Generic;
using UnityEngine;
using HRYooba.Kinect.Core.Services;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Presentations.Kinects
{
    public class KinectInitializer : MonoBehaviour
    {
        [SerializeField] private KinectPresenter _kinectPresenterPrefab = null;
        [SerializeField] private Transform _worldRoot = null;
        [SerializeField] private Transform _operationRoot = null;

        public void Initialize(
            IKinectDataRepository kinectDataRepository,
            IKinectDataFileSaver kinectDataFileSaver,
            IKinectDataUpdater kinectDataUpdater,
            IReadOnlyDictionary<string, IKinectService> kinectServices)
        {
            var allKinectData = kinectDataRepository.GetAll();

            foreach (var kinectData in allKinectData)
            {
                var presenter = Instantiate(_kinectPresenterPrefab, transform);
                var kinectService = kinectServices[kinectData.Id];
                presenter.Construct(kinectData, kinectDataFileSaver, kinectDataUpdater, kinectService, _worldRoot, _operationRoot);
            }
        }
    }
}