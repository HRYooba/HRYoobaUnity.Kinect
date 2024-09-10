using System;
using Newtonsoft.Json;
using UnityEngine;
using HRYooba.Kinect.Core;

namespace HRYooba.Kinect.Services
{
    [Serializable]
    public class KinectsSetting
    {
        [JsonProperty("settings")]
        public Setting[] Settings { get; set; }

        [Serializable]
        public class Setting
        {
            [JsonProperty("id")]
            public string Id { get; set; } = "";

            [JsonProperty("position")]
            public Vector3 Position { get; set; } = Vector3.zero;

            [JsonProperty("euler_angles")]
            public Vector3 EulerAngles { get; set; } = Vector3.zero;

            [JsonProperty("min_depth_distance")]
            public float MinDepthDistance { get; set; } = 0.5f;

            [JsonProperty("max_depth_distance")]
            public float MaxDepthDistance { get; set; } = 10.0f;

            [JsonProperty("body_tracking_sensor_orientation")]
            public BodyTrackingSensorOrientationType BodyTrackingSensorOrientation { get; set; } = BodyTrackingSensorOrientationType.Default;
        }
    }
}
