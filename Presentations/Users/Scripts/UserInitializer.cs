using System.Collections.Generic;
using UnityEngine;
using R3;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;

namespace HRYooba.Kinect.Presentations.Users
{
    public class UserInitializer : MonoBehaviour
    {
        [SerializeField] private UserPresenter _userPresenterPrefab = null;
        [SerializeField] private JointVisibleButton _jointVisibleButton = null;
        [SerializeField] private Transform _worldRoot = null;

        private IUserDataNotifier _userDataNotifier;
        private Dictionary<ulong, UserPresenter> _userPresenterDictionary = new();

        private void OnDestroy()
        {
            foreach (var presenter in _userPresenterDictionary.Values)
            {
                Destroy(presenter.gameObject);
            }

            _userPresenterDictionary.Clear();
        }

        public void Initialize(IUserDataNotifier userDataNotifier)
        {
            _userDataNotifier = userDataNotifier;
            userDataNotifier.OnUserDataAddedObservable.Subscribe(AddUserPresenter).AddTo(this);
            userDataNotifier.OnUserDataRemovedObservable.Subscribe(RemoveUserPresenter).AddTo(this);
        }

        private void AddUserPresenter(UserData userData)
        {
            var presenter = Instantiate(_userPresenterPrefab, transform);
            presenter.Construct(userData, _userDataNotifier, _jointVisibleButton, _worldRoot);

            _userPresenterDictionary.Add(userData.Id, presenter);
        }

        private void RemoveUserPresenter(UserData userData)
        {
            if (_userPresenterDictionary.TryGetValue(userData.Id, out var presenter))
            {
                Destroy(presenter.gameObject);
                _userPresenterDictionary.Remove(userData.Id);
            }
        }
    }
}
