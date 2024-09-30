using UnityEngine;
using R3;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;

namespace HRYooba.Kinect.Presentations.Kinects
{
    public class KinectPresenter : MonoBehaviour
    {
        // model
        private KinectModel _kinectModel;

        // view
        [SerializeField] private KinectObject _kinectObjectPrefab;
        private KinectObject _kinectObject;

        [SerializeField] private PointCloudViewer _pointCloudViewerPrefab;
        private PointCloudViewer _pointCloudViewer;

        [SerializeField] private KinectOperationPanel _kinectOperationPanelPrefab;
        private KinectOperationPanel _kinectOperationPanel;

        private IKinectDataFileSaver _kinectDataFileSaver;
        private IKinectDataUpdater _kinectDataUpdater;
        private IKinectTextureProvider _kinectTextureProvider;
        private IKinectState _kinectState;

        public void Construct(
            KinectData kinectData,
            IKinectDataFileSaver kinectDataFileSaver,
            IKinectDataUpdater kinectDataUpdater,
            IKinectTextureProvider kinectTextureProvider,
            IKinectState kinectState,
            Transform worldRoot,
            Transform operationRoot)
        {
            _kinectDataFileSaver = kinectDataFileSaver;
            _kinectDataUpdater = kinectDataUpdater;
            _kinectTextureProvider = kinectTextureProvider;
            _kinectState = kinectState;
            _kinectModel = new KinectModel(
                kinectData.Id,
                kinectData.Position,
                kinectData.EulerAngles,
                kinectData.MinDepthDistance,
                kinectData.MaxDepthDistance,
                (int)kinectData.BodyTrackingSensorOrientation);
            _kinectObject = Instantiate(_kinectObjectPrefab, worldRoot);
            _pointCloudViewer = Instantiate(_pointCloudViewerPrefab, worldRoot);
            _kinectOperationPanel = Instantiate(_kinectOperationPanelPrefab, operationRoot);
        }

        private void Start()
        {
            SetViewId(_kinectModel.Id);
            SetBodyTrackingSensorOrientation(_kinectModel.BodyTrackingSensorOrientation);
            SetViewTexture();
            BindEvent();
        }

        private void OnDestory()
        {
            _kinectModel.Dispose();

            Destroy(_kinectObject.gameObject);
            _kinectObject = null;

            Destroy(_pointCloudViewer.gameObject);
            _pointCloudViewer = null;

            Destroy(_kinectOperationPanel.gameObject);
            _kinectOperationPanel = null;
        }

        private void BindEvent()
        {
            // model event
            _kinectModel.Position.Subscribe(OnPositionChanged).AddTo(this);
            _kinectModel.IsValid.Subscribe(OnIsValidChanged).AddTo(this);
            _kinectModel.EulerAngles.Subscribe(OnEulerAnglesChanged).AddTo(this);
            _kinectModel.MinDepthDistance.Subscribe(OnMinDepthDistanceChanged).AddTo(this);
            _kinectModel.MaxDepthDistance.Subscribe(OnMaxDepthDistanceChanged).AddTo(this);
            _kinectModel.IsActivePointCloud.Subscribe(OnActivePointCloudChanged).AddTo(this);

            // view event
            _kinectOperationPanel.OnPositionChanged.Subscribe(_kinectModel.SetPosition).AddTo(this);
            _kinectOperationPanel.OnEulerAnglesChanged.Subscribe(_kinectModel.SetEluerAngles).AddTo(this);
            _kinectOperationPanel.OnMinDepthDistanceChanged.Subscribe(_kinectModel.SetMinDepthDistance).AddTo(this);
            _kinectOperationPanel.OnMaxDepthDistanceChanged.Subscribe(_kinectModel.SetMaxDepthDistance).AddTo(this);
            _kinectOperationPanel.OnPointCloudButtonClicked.Subscribe(_ => _kinectModel.SetIsActivePointCloud(!_kinectModel.IsActivePointCloud.CurrentValue)).AddTo(this);
            _kinectOperationPanel.OnSaveButtonClicked.Subscribe(_ => _kinectDataFileSaver.Save()).AddTo(this);

            Observable.EveryValueChanged(_kinectState, state => state.IsValid).Subscribe(_kinectModel.SetIsValid).AddTo(this);
        }

        private void SetViewId(string id)
        {
            _kinectObject.SetId(id);
            _kinectOperationPanel.SetIdText(id);
        }

        private void SetBodyTrackingSensorOrientation(int orientation) 
        {
            _kinectOperationPanel.SetBodyTrackingSensorOrientation(orientation);
        }

        private void OnPositionChanged(Vector3 position)
        {
            _kinectDataUpdater.Update(
                new KinectData(
                    _kinectModel.Id,
                    position,
                    _kinectModel.EulerAngles.CurrentValue,
                    _kinectModel.MinDepthDistance.CurrentValue,
                    _kinectModel.MaxDepthDistance.CurrentValue,
                    (BodyTrackingSensorOrientationType)_kinectModel.BodyTrackingSensorOrientation
                )
            );

            _kinectObject.SetPosition(position);
            _pointCloudViewer.SetPosition(position);
            _kinectOperationPanel.SetPosition(position);
        }

        private void OnIsValidChanged(bool isValid)
        {
            _kinectObject.SetIsValid(isValid);
            _kinectOperationPanel.SetIsValid(isValid);
        }

        private void OnEulerAnglesChanged(Vector3 eulerAngles)
        {
            _kinectDataUpdater.Update(
                new KinectData(
                    _kinectModel.Id,
                    _kinectModel.Position.CurrentValue,
                    eulerAngles,
                    _kinectModel.MinDepthDistance.CurrentValue,
                    _kinectModel.MaxDepthDistance.CurrentValue,
                    (BodyTrackingSensorOrientationType)_kinectModel.BodyTrackingSensorOrientation
                )
            );

            _kinectObject.SetEluerAngles(eulerAngles);
            _pointCloudViewer.SetEluerAngles(eulerAngles);
            _kinectOperationPanel.SetEulerAngles(eulerAngles);
        }

        private void OnMinDepthDistanceChanged(float distance)
        {
            _kinectDataUpdater.Update(
                new KinectData(
                    _kinectModel.Id,
                    _kinectModel.Position.CurrentValue,
                    _kinectModel.EulerAngles.CurrentValue,
                    distance,
                    _kinectModel.MaxDepthDistance.CurrentValue,
                    (BodyTrackingSensorOrientationType)_kinectModel.BodyTrackingSensorOrientation
                )
            );

            _kinectOperationPanel.SetMinDepthDistance(distance);
        }

        private void OnMaxDepthDistanceChanged(float distance)
        {
            _kinectDataUpdater.Update(
                new KinectData(
                    _kinectModel.Id,
                    _kinectModel.Position.CurrentValue,
                    _kinectModel.EulerAngles.CurrentValue,
                    _kinectModel.MinDepthDistance.CurrentValue,
                    distance,
                    (BodyTrackingSensorOrientationType)_kinectModel.BodyTrackingSensorOrientation
                )
            );
            _kinectOperationPanel.SetMaxDepthDistance(distance);
        }

        private void OnActivePointCloudChanged(bool isActive)
        {
            _pointCloudViewer.SetVisible(isActive);
            _kinectOperationPanel.SetActivePointCloudButton(!isActive);
        }

        private void SetViewTexture()
        {
            // PointCloud
            _pointCloudViewer.SetDepthMap(_kinectTextureProvider.DepthTexture);
            _pointCloudViewer.SetVertexMap(_kinectTextureProvider.PointCloudTexture);

            // KinectOperationPanel
            _kinectOperationPanel.SetColorImage(_kinectTextureProvider.ColorTexture);
            _kinectOperationPanel.SetDepthImage(_kinectTextureProvider.DepthTexture);
            _kinectOperationPanel.SetBodyImage(_kinectTextureProvider.BodyIndexTexture);
            _kinectOperationPanel.SetPrimaryUserImage(_kinectTextureProvider.PrimaryUserTexture);
            _kinectOperationPanel.SetPointCloudImage(_kinectTextureProvider.PointCloudTexture);
        }
    }
}