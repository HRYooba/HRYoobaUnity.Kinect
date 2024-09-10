using UnityEngine;

namespace HRYooba.Kinect.Core
{
    public class AreaData
    {
        public AreaData(int id, Vector3 position, float radius)
        {
            Id = id;
            Position = position;
            Radius = radius;
        }

        public int Id { get; }
        public Vector3 Position { get; }
        public float Radius { get; }
    }
}