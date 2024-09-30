using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HRYooba.Kinect.Presentations
{
    [RequireComponent(typeof(Image))]
    public class CameraController : MonoBehaviour, IDragHandler, IScrollHandler
    {
        [SerializeField] private float _dragSensitivity = 0.1f;
        [SerializeField] private float _zoomSensitivity = 2f;

        private Camera _targetCamera;
        public enum Axis
        {
            X,
            Y,
            Z
        }
        private Axis _axis;

        public void SetTargetCamera(Camera camera, Axis axis)
        {
            _targetCamera = camera;
            _axis = axis;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var delta = Vector3.zero;
            switch (_axis)
            {
                case Axis.X:
                    delta = new Vector3(0, -eventData.delta.y, eventData.delta.x) * _dragSensitivity;
                    break;

                case Axis.Y:
                    delta = new Vector3(-eventData.delta.x, 0, -eventData.delta.y) * _dragSensitivity;
                    break;

                case Axis.Z:
                    delta = new Vector3(-eventData.delta.x, -eventData.delta.y, 0) * _dragSensitivity;
                    break;
            }
            _targetCamera.transform.localPosition += delta * Time.deltaTime;
        }

        public void OnScroll(PointerEventData eventData)
        {
            var scrollAmount = eventData.scrollDelta.y * _zoomSensitivity;
            _targetCamera.orthographicSize -= scrollAmount * Time.deltaTime;
        }
    }
}