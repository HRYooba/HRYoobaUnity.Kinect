using System.Collections.Generic;
using System.Linq;
using HRYooba.Kinect.Core;
using HRYooba.Kinect.Core.Repositories;

namespace HRYooba.Kinect.Repositories
{
    public class UserDataRepository : IUserDataRepository
    {
        private readonly Dictionary<ulong, UserData> _userDataDictionary = new();

        public void Add(UserData userData)
        {
            if (!_userDataDictionary.ContainsKey(userData.Id))
            {
                _userDataDictionary.Add(userData.Id, userData);
            }
        }

        public void Update(UserData userData)
        {
            if (_userDataDictionary.ContainsKey(userData.Id))
            {
                _userDataDictionary[userData.Id] = userData;
            }
        }

        public void Remove(ulong id)
        {
            if (_userDataDictionary.ContainsKey(id))
            {
                _userDataDictionary.Remove(id);
            }
        }

        public void Clear()
        {
            _userDataDictionary.Clear();
        }

        public UserData Get(ulong id)
        {
            return _userDataDictionary.ContainsKey(id) ? _userDataDictionary[id] : default;
        }

        public UserData[] GetAll()
        {
            return _userDataDictionary.Values.ToArray();
        }
    }
}