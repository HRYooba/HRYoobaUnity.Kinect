using UnityEngine;
using R3;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;

namespace HRYooba.Kinect.Presentations.Areas
{
    public class AreaPresenter : MonoBehaviour
    {
        // model
        private AreaModel _areaModel;

        // view
        [SerializeField] private AreaObject _areaObjectPrefab;
        private AreaObject _areaObject;

        [SerializeField] private AreaOperationPanel _areaOperationPanelPrefab;
        private AreaOperationPanel _areaOperationPanel;

        private IAreaDataFileSaver _areaDataFileSaver;
        private IAreaDataUpdater _areaDataUpdater;

        public void Construct(
            AreaData areaData,
            IAreaDataFileSaver areaDataFileSaver,
            IAreaDataUpdater areaDataUpdater,
            Transform worldRoot,
            Transform operationRoot)
        {
            _areaDataFileSaver = areaDataFileSaver;
            _areaDataUpdater = areaDataUpdater;
            _areaModel = new AreaModel(areaData.Id, areaData.Position, areaData.Radius);
            _areaObject = Instantiate(_areaObjectPrefab, worldRoot);
            _areaOperationPanel = Instantiate(_areaOperationPanelPrefab, operationRoot);
        }

        private void Start()
        {
            SetViewId(_areaModel.Id);
            BindEvent();
        }

        private void OnDestory()
        {
            _areaModel.Dispose();

            Destroy(_areaObject.gameObject);
            _areaObject = null;

            Destroy(_areaOperationPanel.gameObject);
            _areaOperationPanel = null;
        }

        private void SetViewId(int id)
        {
            _areaObject.SetId(id);
            _areaOperationPanel.SetId(id);
        }

        private void BindEvent()
        {
            // model event
            _areaModel.Position.Subscribe(OnPositionChanged).AddTo(this);
            _areaModel.Radius.Subscribe(OnRadiusChanged).AddTo(this);

            // view event
            _areaOperationPanel.OnPositionChanged.Subscribe(_areaModel.SetPosition).AddTo(this);
            _areaOperationPanel.OnRadiusChanged.Subscribe(_areaModel.SetRadius).AddTo(this);
            _areaOperationPanel.OnSaveButtonClicked.Subscribe(_ => _areaDataFileSaver.Save()).AddTo(this);
        }

        private void OnPositionChanged(Vector3 position)
        {
            _areaDataUpdater.Update(
                new AreaData(
                    _areaModel.Id,
                    position,
                    _areaModel.Radius.CurrentValue
                )
            );

            _areaObject.SetPosition(position);
            _areaOperationPanel.SetPosition(position);
        }

        private void OnRadiusChanged(float radius)
        {
            _areaDataUpdater.Update(
                new AreaData(
                    _areaModel.Id,
                    _areaModel.Position.CurrentValue,
                    radius
                )
            );

            _areaObject.SetRadius(radius);
            _areaOperationPanel.SetRadius(radius);
        }
    }
}