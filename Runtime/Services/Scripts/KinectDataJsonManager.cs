using System.Linq;
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
        private KinectsSetting _initialSetting;

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
                    setting.PrimaryUserAreaId
                );
            }

            _repository.Clear();
            foreach (var data in kinectData)
            {
                _repository.Add(data);
            }

            _initialSetting = json;
        }

        public void Save()
        {
            var json = _initialSetting;
            var kinectData = _repository.GetAll();
            
            foreach (var setting in json.Settings)
            {
                var data = kinectData.FirstOrDefault(d => d.Id == setting.Id);
                if (data != null)
                {
                    setting.Position = data.Position;
                    setting.EulerAngles = data.EulerAngles;
                    setting.MinDepthDistance = data.MinDepthDistance;
                    setting.MaxDepthDistance = data.MaxDepthDistance;
                    setting.BodyTrackingSensorOrientation = data.BodyTrackingSensorOrientation;
                    setting.PrimaryUserAreaId = data.PrimaryUserAreaId;
                }
            }

            JsonHelper.Save(JsonPath, json);
        }

        public void Update(KinectData data)
        {
            _repository.Update(data);
        }
    }
}