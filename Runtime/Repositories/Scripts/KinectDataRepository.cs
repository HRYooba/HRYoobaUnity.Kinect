using System.Collections.Generic;
using System.Linq;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Repositories
{
    public class KinectDataRepository : IKinectDataRepository
    {
        private readonly Dictionary<string, KinectData> _kinectDataDictionary = new();

        public void Add(KinectData kinectData)
        {
            if (!_kinectDataDictionary.ContainsKey(kinectData.Id))
            {
                _kinectDataDictionary.Add(kinectData.Id, kinectData);
            }
        }

        public void Update(KinectData kinectData)
        {
            if (_kinectDataDictionary.ContainsKey(kinectData.Id))
            {
                _kinectDataDictionary[kinectData.Id] = kinectData;
            }
        }

        public void Remove(string id)
        {
            if (_kinectDataDictionary.ContainsKey(id))
            {
                _kinectDataDictionary.Remove(id);
            }
        }

        public void Clear()
        {
            _kinectDataDictionary.Clear();
        }

        public KinectData Get(string id)
        {
            return _kinectDataDictionary.ContainsKey(id) ? _kinectDataDictionary[id] : default;
        }

        public KinectData[] GetAll()
        {
            return _kinectDataDictionary.Values.ToArray();
        }
    }
}