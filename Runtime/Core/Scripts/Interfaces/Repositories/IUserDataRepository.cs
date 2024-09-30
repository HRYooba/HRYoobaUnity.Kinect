namespace HRYooba.Kinect.Core.Repositories
{
    public interface IUserDataRepository
    {
        void Add(UserData userData);
        void Update(UserData userData);
        void Remove(ulong id);
        void Clear();
        UserData Get(ulong id);
        UserData[] GetAll();
    }
}