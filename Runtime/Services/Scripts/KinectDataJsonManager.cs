using UnityEngine;
using HRYooba.Library;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Services
{
    public class KinectDataJsonManager : IKinectDataFileSaver, IKinectDataUpdater
    {
        public const string JsonPath = "kinects_setting.json";
        private readonly IKinectDataRepository _repository;

        public KinectDataJsonManager(IKinectDataRepository repository)
        {
            _repository = repository;
            Load();
        }

        private void Load()
        {
            var json = JsonHelper.Load<KinectsSetting>(JsonPath);
            if (json.Settings == null)
            {
                Debug.LogWarning($"[KinectDataJsonManager] {JsonPath} is null. Create new json file.");
                json = new KinectsSetting
                {
                    Settings = new[] { new KinectsSetting.Setting() }
                };
                JsonHelper.Save(JsonPath, json);
            }

            var kinectData = new KinectData[json.Settings.Length];
            for (var i = 0; i < json.Settings.Length; i++)
            {
                var setting = json.Settings[i];
                kinectData[i] = new KinectData(
                    setting.Id,
                    setting.Position,
                    setting.EulerAngles,
                    setting.MinDepthDistance,
                    setting.MaxDepthDistance,
                    setting.BodyTrackingSensorOrientation,
                    setting.PrimaryUserAreaId);
            }

            _repository.Clear();
            foreach (var data in kinectData)
            {
                _repository.Add(data);
            }
        }

        public void Save()
        {
            var kinectData = _repository.GetAll();
            var json = new KinectsSetting
            {
                Settings = new KinectsSetting.Setting[kinectData.Length]
            };

            for (var i = 0; i < kinectData.Length; i++)
            {
                var data = kinectData[i];
                json.Settings[i] = new KinectsSetting.Setting
                {
                    Id = data.Id,
                    Position = data.Position,
                    EulerAngles = data.EulerAngles,
                    MinDepthDistance = data.MinDepthDistance,
                    MaxDepthDistance = data.MaxDepthDistance,
                    BodyTrackingSensorOrientation = data.BodyTrackingSensorOrientation
                };
            }

            JsonHelper.Save(JsonPath, json);
        }

        public void Update(KinectData data)
        {
            _repository.Update(data);
        }
    }
}