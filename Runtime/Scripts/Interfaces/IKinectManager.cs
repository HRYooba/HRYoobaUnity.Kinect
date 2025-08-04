using HRYooba.Kinect.Core;

namespace HRYooba.Kinect
{
    public interface IKinectManager
    {
        UserData[] GetAllUserData();
        AreaData[] GetAllAreaData();
        KinectData[] GetAllKinectData();
        UserData[] GetAllPrimaryUserData();
        UserData GetPrimaryUserData(string kinectId = null);
        UserData GetUserData(int areaId);
        ProvideTextures[] GetAllKinectTextures();
        ProvideTextures GetKinectTextures(string kinectId = null);
    }
}