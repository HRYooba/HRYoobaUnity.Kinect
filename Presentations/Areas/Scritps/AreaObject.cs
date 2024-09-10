using UnityEngine;

namespace HRYooba.Kinect.Presentations.Areas
{
    public class AreaObject : MonoBehaviour
    {
        [SerializeField] private GameObject _model = null;
        [SerializeField] private TextMesh _idText = null;

        public void SetId(int id)
        {
            _idText.text = $"ID:{id}";
        }

        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void SetRadius(float radius)
        {
            var scale = _model.transform.localScale;
            _model.transform.localScale = new Vector3(radius * 2.0f, scale.y, radius * 2.0f);
        }
    }
}