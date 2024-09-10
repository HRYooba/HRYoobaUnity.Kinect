using R3;

namespace HRYooba.Kinect.Core.Services
{
    public interface IUserDataNotifier : IUserDataAddNotifier, IUserDataUpdateNotifier, IUserDataRemoveNotifier
    {
        
    }

    public interface IUserDataAddNotifier
    {
        public Observable<UserData> OnUserDataAddedObservable { get; }
    }

    public interface IUserDataUpdateNotifier
    {
        public Observable<UserData> OnUserDataUpdatedObservable { get; }
    }

    public interface IUserDataRemoveNotifier
    {
        public Observable<UserData> OnUserDataRemovedObservable { get; }
    }
}