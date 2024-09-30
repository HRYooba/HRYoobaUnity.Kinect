using UnityEngine;

namespace HRYooba.Kinect.Presentations.Kinects
{
    public class KinectObject : MonoBehaviour
    {
        [SerializeField] private GameObject _model = null;
        [SerializeField] private TextMesh _idText = null;

        public void SetId(string id)
        {
            _idText.text = $"ID:{id}";
        }

        public void SetIsValid(bool isValid)
        {
            _idText.color = isValid ? Color.white : Color.red;
        }

        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void SetEluerAngles(Vector3 eluerAngles)
        {
            _model.transform.localRotation = Quaternion.Euler(eluerAngles);
        }
    }
}