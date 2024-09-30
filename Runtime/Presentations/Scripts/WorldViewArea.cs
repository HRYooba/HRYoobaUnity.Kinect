using UnityEngine;
using UnityEngine.UI;

namespace HRYooba.Kinect.Presentations
{
    public class WorldViewArea : MonoBehaviour
    {
        [SerializeField] private Camera _directionYCamera = null;
        [SerializeField] private Camera _directionZCamera = null;
        [SerializeField] private Camera _directionXCamera = null;
        [SerializeField] private RectTransform _directionYWindow = null;
        [SerializeField] private RectTransform _directionZWindow = null;
        [SerializeField] private RectTransform _directionXWindow = null;
        [SerializeField] private Button _directionYButton = null;
        [SerializeField] private Button _directionZButton = null;
        [SerializeField] private Button _directionXButton = null;
        [SerializeField] private CameraController _cameraController = null;

        private void Awake()
        {
            ShowDirectionYWindow();

            _directionYButton.onClick.AddListener(ShowDirectionYWindow);
            _directionZButton.onClick.AddListener(ShowDirectionZWindow);
            _directionXButton.onClick.AddListener(ShowDirectionXWindow);
        }

        private void Update() 
        {
            // cameraのtextureのsizeを変更する
        }

        private void ShowDirectionYWindow()
        {
            _directionYCamera.gameObject.SetActive(true);
            _directionZCamera.gameObject.SetActive(false);
            _directionXCamera.gameObject.SetActive(false);

            _directionYWindow.gameObject.SetActive(true);
            _directionZWindow.gameObject.SetActive(false);
            _directionXWindow.gameObject.SetActive(false);

            _cameraController.SetTargetCamera(_directionYCamera, CameraController.Axis.Y);
        }

        private void ShowDirectionZWindow()
        {
            _directionYCamera.gameObject.SetActive(false);
            _directionZCamera.gameObject.SetActive(true);
            _directionXCamera.gameObject.SetActive(false);

            _directionYWindow.gameObject.SetActive(false);
            _directionZWindow.gameObject.SetActive(true);
            _directionXWindow.gameObject.SetActive(false);

            _cameraController.SetTargetCamera(_directionZCamera, CameraController.Axis.Z);
        }

        private void ShowDirectionXWindow()
        {
            _directionYCamera.gameObject.SetActive(false);
            _directionZCamera.gameObject.SetActive(false);
            _directionXCamera.gameObject.SetActive(true);

            _directionYWindow.gameObject.SetActive(false);
            _directionZWindow.gameObject.SetActive(false);
            _directionXWindow.gameObject.SetActive(true);

            _cameraController.SetTargetCamera(_directionXCamera, CameraController.Axis.X);
        }
    }
}