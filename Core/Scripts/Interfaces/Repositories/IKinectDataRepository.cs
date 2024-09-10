namespace HRYooba.Kinect.Core.Repositories
{
    public interface IKinectDataRepository
    {
        void Add(KinectData kinectData);
        void Update(KinectData kinectData);
        void Remove(string id);
        void Clear();
        KinectData Get(string id);
        KinectData[] GetAll();
    }
}