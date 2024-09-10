using UnityEngine;
using HRYooba.Library;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Services;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Services
{
    public class AreaDataJsonManager : IAreaDataFileSaver, IAreaDataUpdater
    {
        public const string JsonPath = "areas_setting.json";
        private readonly IAreaDataRepository _repository;

        public AreaDataJsonManager(IAreaDataRepository repository)
        {
            _repository = repository;
            Load();
        }

        private void Load()
        {
            var json = JsonHelper.Load<AreasSetting>(JsonPath);
            if (json.Settings == null)
            {
                Debug.LogWarning($"[AreaDataJsonManager] {JsonPath} is null. Create new json file.");
                json = new AreasSetting
                {
                    Settings = new[] { new AreasSetting.Setting() }
                };
                JsonHelper.Save(JsonPath, json);
            }

            var areaData = new AreaData[json.Settings.Length];
            for (var i = 0; i < json.Settings.Length; i++)
            {
                var setting = json.Settings[i];
                areaData[i] = new AreaData(setting.Id, setting.Position, setting.Radius);
            }

            _repository.Clear();
            foreach (var data in areaData)
            {
                _repository.Add(data);
            }
        }

        public void Save()
        {
            var areaData = _repository.GetAll();

            var json = new AreasSetting
            {
                Settings = new AreasSetting.Setting[areaData.Length]
            };

            for (var i = 0; i < areaData.Length; i++)
            {
                var data = areaData[i];
                json.Settings[i] = new AreasSetting.Setting
                {
                    Id = data.Id,
                    Position = data.Position,
                    Radius = data.Radius
                };
            }

            JsonHelper.Save(JsonPath, json);
        }

        public void Update(AreaData areaData)
        {
            _repository.Update(areaData);
        }
    }
}