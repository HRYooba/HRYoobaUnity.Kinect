using System.Collections.Generic;
using UnityEngine;

namespace HRYooba.Kinect.Presentations.Users
{
    public class UserObject : MonoBehaviour
    {
        [SerializeField] private GameObject _cone = null;
        [SerializeField] private TextMesh _idText = null;
        [SerializeField] private GameObject _boxPrefab = null;
        private readonly List<GameObject> _boxes = new();

        private void Awake()
        {
            var boxCount = 32;
            for (var i = 0; i < boxCount; i++)
            {
                var box = Instantiate(_boxPrefab, transform);
                box.transform.localPosition = Vector3.zero;
                box.transform.localRotation = Quaternion.identity;
                box.name = $"Box{i}";
                _boxes.Add(box);
            }
        }

        private void OnDestroy()
        {
            foreach (var box in _boxes) Destroy(box);
            _boxes.Clear();
        }

        public void SetId(ulong id)
        {
            _idText.text = $"ID:{id}";
        }

        public void SetPosition(Vector3 position)
        {
            _cone.transform.localPosition = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            // _cone.transform.localRotation = rotation;
            _cone.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        }

        public void SetJoints((bool isTracking, Vector3 Position, Quaternion Rotation)[] joints)
        {
            for (var i = 0; i < joints.Length; i++)
            {
                var box = _boxes[i];
                var joint = joints[i];
                box.transform.localPosition = joint.Position;
                box.transform.localRotation = joint.Rotation;
            }
        }

        public void SetVisibleBoxes(bool isVisible)
        {
            foreach (var box in _boxes) box.SetActive(isVisible);
        }
    }
}