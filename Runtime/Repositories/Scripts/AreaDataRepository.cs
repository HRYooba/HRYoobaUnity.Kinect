using System.Collections.Generic;
using System.Linq;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Repositories
{
    public class AreaDataRepository : IAreaDataRepository
    {
        private readonly Dictionary<int, AreaData> _areaDataDictionary = new();

        public void Add(AreaData areaData)
        {
            if (!_areaDataDictionary.ContainsKey(areaData.Id))
            {
                _areaDataDictionary.Add(areaData.Id, areaData);
            }
        }

        public void Update(AreaData areaData)
        {
            if (_areaDataDictionary.ContainsKey(areaData.Id))
            {
                _areaDataDictionary[areaData.Id] = areaData;
            }
        }

        public void Remove(int id)
        {
            if (_areaDataDictionary.ContainsKey(id))
            {
                _areaDataDictionary.Remove(id);
            }
        }

        public void Clear()
        {
            _areaDataDictionary.Clear();
        }

        public AreaData Get(int id)
        {
            return _areaDataDictionary.ContainsKey(id) ? _areaDataDictionary[id] : default;
        }

        public AreaData[] GetAll()
        {
            return _areaDataDictionary.Values.ToArray();
        }
    }
}