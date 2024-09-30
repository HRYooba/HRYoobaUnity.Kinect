using UnityEngine;
using UnityEngine.VFX;

namespace HRYooba.Kinect.Presentations.Kinects
{
    public class PointCloudViewer : MonoBehaviour
    {
        [SerializeField] private VisualEffect _visualEffect = null;
        private Texture _depthMap = null;
        private Texture _vertexMap = null;

        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void SetEluerAngles(Vector3 eluerAngles)
        {
            transform.localRotation = Quaternion.Euler(eluerAngles);
        }

        public void SetDepthMap(Texture depthMap)
        {
            if (depthMap == null) return;
            _depthMap = depthMap;
        }

        public void SetVertexMap(Texture vertexMap)
        {
            if (vertexMap == null) return;
            _vertexMap = vertexMap;
        }

        public void SetVisible(bool isVisible)
        {
            _visualEffect.gameObject.SetActive(isVisible);
            if (isVisible)
            {
                if (_depthMap != null) _visualEffect.SetTexture("DepthMap", _depthMap);
                if (_vertexMap != null) _visualEffect.SetTexture("VertexMap", _vertexMap);
            }
        }
    }
}