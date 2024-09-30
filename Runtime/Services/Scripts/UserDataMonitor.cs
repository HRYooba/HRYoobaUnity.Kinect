using System;
using System.Linq;
using R3;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Services
{
    public class UserDataMonitor : IUserDataNotifier, IDisposable
    {
        private readonly IUserDataRepository _repository;
        private readonly UserData.IdComparer _userDataIdComparer = new();
        private readonly CompositeDisposable _disposables = new();
        private UserData[] _userDataBuffer = Array.Empty<UserData>();

        private readonly Subject<UserData> _onUserDataAddedSubject = new();
        public Observable<UserData> OnUserDataAddedObservable => _onUserDataAddedSubject;

        private readonly Subject<UserData> _onUserDataUpdatedSubject = new();
        public Observable<UserData> OnUserDataUpdatedObservable => _onUserDataUpdatedSubject;

        private readonly Subject<UserData> _onUserDataRemovedSubject = new();
        public Observable<UserData> OnUserDataRemovedObservable => _onUserDataRemovedSubject;

        public UserDataMonitor(IUserDataRepository repository)
        {
            _repository = repository;

            Observable.EveryUpdate().Subscribe(_ => Update()).AddTo(_disposables);
        }

        public void Dispose()
        {
            if (_disposables.IsDisposed) return;

            _userDataBuffer = Array.Empty<UserData>();
            _disposables.Dispose();
            _onUserDataAddedSubject.Dispose();
            _onUserDataUpdatedSubject.Dispose();
            _onUserDataRemovedSubject.Dispose();
        }

        private void Update()
        {
            var currentUserData = _repository.GetAll().ToArray();
            var newUserData = currentUserData.Except(_userDataBuffer, _userDataIdComparer).ToArray();
            var removedUserData = _userDataBuffer.Except(currentUserData, _userDataIdComparer).ToArray();

            foreach (var userData in newUserData)
            {
                _onUserDataAddedSubject.OnNext(userData);
            }

            foreach (var userData in removedUserData)
            {
                _onUserDataRemovedSubject.OnNext(userData);
            }

            foreach (var userData in currentUserData)
            {
                _onUserDataUpdatedSubject.OnNext(userData);
            }

            _userDataBuffer = currentUserData;
        }
    }
}