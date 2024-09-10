using System;
using Newtonsoft.Json;
using UnityEngine;

namespace HRYooba.Kinect.Services
{
    [Serializable]
    public class AreasSetting
    {
        [JsonProperty("settings")]
        public Setting[] Settings { get; set; }

        [Serializable]
        public class Setting
        {
            [JsonProperty("id")]
            public int Id { get; set; } = 1;

            [JsonProperty("position")]
            public Vector3 Position { get; set; } = Vector3.zero;

            [JsonProperty("radius")]
            public float Radius { get; set; } = 1.0f;
        }
    }
}