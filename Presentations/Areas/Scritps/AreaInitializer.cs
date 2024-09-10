using UnityEngine;
using HRYooba.Kinect.Core.Services;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Presentations.Areas
{
    public class AreaInitializer : MonoBehaviour
    {
        [SerializeField] private AreaPresenter _areaPresenterPrefab = null;
        [SerializeField] private Transform _worldRoot = null;
        [SerializeField] private Transform _operationRoot = null;

        public void Initialize(
            IAreaDataRepository areaDataRepository,
            IAreaDataFileSaver areaDataFileSaver,
            IAreaDataUpdater areaDataUpdater)
        {
            var allAreaData = areaDataRepository.GetAll();
            foreach (var areaData in allAreaData)
            {
                var presenter = Instantiate(_areaPresenterPrefab, transform);
                presenter.Construct(areaData, areaDataFileSaver, areaDataUpdater, _worldRoot, _operationRoot);
            }
        }
    }
}