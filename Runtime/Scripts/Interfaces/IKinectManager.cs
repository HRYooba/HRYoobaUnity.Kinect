using HRYooba.Kinect.Core;

namespace HRYooba.Kinect
{
    public interface IKinectManager
    {
        UserData[] GetAllUserData();
        AreaData[] GetAllAreaData();
        KinectData[] GetAllKinectData();
        UserData GetUserData(int areaId);
        ProvideTextures[] GetAllKinectTextures();
        ProvideTextures GetKinectTextures(string kinectId = null);
    }
}