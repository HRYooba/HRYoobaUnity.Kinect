using UnityEngine;

namespace HRYooba.Kinect.Core
{
    public class KinectData
    {
        public KinectData(
            string id, 
            Vector3 position, 
            Vector3 eulerAngles, 
            float minDepthDistance, 
            float maxDepthDistance, 
            BodyTrackingSensorOrientationType bodyTrackingSensorOrientation)
        {
            Id = id;
            Position = position;
            EulerAngles = eulerAngles;
            MinDepthDistance = minDepthDistance;
            MaxDepthDistance = maxDepthDistance;
            BodyTrackingSensorOrientation = bodyTrackingSensorOrientation;
        }

        public string Id { get; }
        public Vector3 Position { get; }
        public Vector3 EulerAngles { get; }
        public float MinDepthDistance { get; }
        public float MaxDepthDistance { get; }
        public BodyTrackingSensorOrientationType BodyTrackingSensorOrientation { get; }
    }
}
