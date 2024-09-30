using System.Linq;
using UnityEngine;
using R3;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;

namespace HRYooba.Kinect.Presentations.Users
{
    public class UserPresenter : MonoBehaviour
    {
        // model
        private UserModel _userModel;

        // view
        [SerializeField] private UserObject _userObjectPrefab;
        private UserObject _userObject;

        private IUserDataUpdateNotifier _userDataUpdateNotifier;
        private IJointViewState _jointViewState;

        public void Construct(
            UserData userData,
            IUserDataUpdateNotifier userDataUpdateNotifier,
            IJointViewState jointViewState,
            Transform worldRoot)
        {
            _userDataUpdateNotifier = userDataUpdateNotifier;
            _jointViewState = jointViewState;
            _userModel = new UserModel(
                userData.Id,
                userData.Position,
                userData.Rotation,
                ConvertJoints(userData.Joints));

            _userObject = Instantiate(_userObjectPrefab, worldRoot);
        }

        private void Start()
        {
            _userObject.SetId(_userModel.Id);
            BindEvent();
        }

        private void OnDestroy()
        {
            _userModel.Dispose();

            Destroy(_userObject.gameObject);
            _userObject = null;
        }

        private void BindEvent()
        {
            _userModel.Position.Subscribe(_userObject.SetPosition).AddTo(this);
            _userModel.Rotation.Subscribe(_userObject.SetRotation).AddTo(this);
            _userModel.Joints.Subscribe(_userObject.SetJoints).AddTo(this);

            _userDataUpdateNotifier.OnUserDataUpdatedObservable
                .Where(userData => userData.Id == _userModel.Id)
                .Subscribe(OnUserDataUpdated)
                .AddTo(this);

            _jointViewState.IsVisible.Subscribe(_userObject.SetVisibleBoxes).AddTo(this);
        }

        private (bool IsTracking, Vector3 Position, Quaternion Rotation)[] ConvertJoints(JointData[] joints)
        {
            return joints.Select(j => (j.IsTracking, j.Position, j.Rotation)).ToArray();
        }

        private void OnUserDataUpdated(UserData userData)
        {
            _userModel.SetPosition(userData.Position);
            _userModel.SetRotation(userData.Rotation);
            _userModel.SetJoints(ConvertJoints(userData.Joints));
        }
    }
}