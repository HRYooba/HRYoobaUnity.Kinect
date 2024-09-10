using HRYooba.Kinect.Core;

namespace HRYooba.Kinect
{
    public interface IKinectManager
    {
        bool IsValid { get; }
        UserData[] GetAllUserData();
        AreaData[] GetAllAreaData();
        KinectData[] GetAllKinectData();
        UserData[] GetAllPrimaryUserData();
        UserData GetPrimaryUserData(string kinectId = null);
        ProvideTextures[] GetAllKinectTextures();
        ProvideTextures GetKinectTextures(string kinectId = null);
    }
}