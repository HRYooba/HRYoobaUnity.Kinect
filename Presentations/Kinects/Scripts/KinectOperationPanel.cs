using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using HRYooba.UI;

namespace HRYooba.Kinect.Presentations.Kinects
{
    public class KinectOperationPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _infoText = null;
        [SerializeField] private TextMeshProUGUI _bodyTrackingSensorOrientationText = null;
        [SerializeField] private RawImage _colorImage = null;
        [SerializeField] private RawImage _depthImage = null;
        [SerializeField] private RawImage _bodyImage = null;
        [SerializeField] private RawImage _primaryUserImage = null;
        [SerializeField] private RawImage _pointCloudImage = null;
        [SerializeField] private Button _pointCloudButton = null;
        [SerializeField] private Vector3Controller _positionController = null;
        [SerializeField] private Vector3Controller _eulerAnglesController = null;
        [SerializeField] private FloatController _minDepthDistanceController = null;
        [SerializeField] private FloatController _maxDepthDistanceController = null;
        [SerializeField] private Button _saveButton = null;

        public Observable<Vector3> OnPositionChanged => _positionController.OnValueChangedObservable;
        public Observable<Vector3> OnEulerAnglesChanged => _eulerAnglesController.OnValueChangedObservable;
        public Observable<float> OnMinDepthDistanceChanged => _minDepthDistanceController.OnValueChangedObservable;
        public Observable<float> OnMaxDepthDistanceChanged => _maxDepthDistanceController.OnValueChangedObservable;
        public Observable<Unit> OnPointCloudButtonClicked => _pointCloudButton.OnClickAsObservable();
        public Observable<Unit> OnSaveButtonClicked => _saveButton.OnClickAsObservable();

        public void SetIdText(string id)
        {
            _infoText.text = $"SensorID:{id}";
        }

        public void SetBodyTrackingSensorOrientation(int orientation)
        {
            var text = orientation switch
            {
                0 => "Default",
                1 => "Clockwise90",
                2 => "Clockwise180",
                3 => "Clockwise270",
                _ => "Error"
            };
            _bodyTrackingSensorOrientationText.text = $"SensorOrientationType: {text}";

            RotateTexture(orientation, _colorImage);
            RotateTexture(orientation, _depthImage);
            RotateTexture(orientation, _bodyImage);
            RotateTexture(orientation, _primaryUserImage);
            RotateTexture(orientation, _pointCloudImage);
        }

        public void SetIsValid(bool isValid)
        {
            _infoText.color = isValid ? Color.white : Color.red;
        }

        public void SetColorImage(Texture texture)
        {
            _colorImage.texture = texture;
        }

        public void SetBodyImage(Texture texture)
        {
            _bodyImage.texture = texture;
        }

        public void SetDepthImage(Texture texture)
        {
            _depthImage.texture = texture;
        }

        public void SetPrimaryUserImage(Texture texture)
        {
            _primaryUserImage.texture = texture;
        }

        public void SetPointCloudImage(Texture texture)
        {
            _pointCloudImage.texture = texture;
        }

        public void SetPosition(Vector3 position)
        {
            _positionController.Value = position;
        }

        public void SetEulerAngles(Vector3 eulerAngles)
        {
            _eulerAnglesController.Value = eulerAngles;
        }

        public void SetMinDepthDistance(float distance)
        {
            _minDepthDistanceController.Value = distance;
        }

        public void SetMaxDepthDistance(float distance)
        {
            _maxDepthDistanceController.Value = distance;
        }

        public void SetActivePointCloudButton(bool isActive)
        {
            _pointCloudButton.image.color = isActive ? Color.white : Color.black;
        }

        private void RotateTexture(int orientation, RawImage image)
        {
            var angle = orientation switch
            {
                0 => 0,
                1 => -90,
                2 => 90,
                3 => -180,
                _ => 0
            };
            image.transform.localEulerAngles = new Vector3(0, 0, angle);

            switch (orientation)
            {
                case 1:
                case 2:
                    var imageSiblingIndex = image.transform.GetSiblingIndex();
                    var obj = Instantiate(new GameObject($"{image.name} DestroyObject"), image.transform.parent);
                    var parent = obj.AddComponent<RectTransform>();
                    parent.name = $"{image.name} Parent";

                    image.transform.SetParent(parent);
                    image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    image.rectTransform.anchoredPosition = Vector2.zero;

                    parent.SetSiblingIndex(imageSiblingIndex);
                    parent.sizeDelta = new Vector2(image.rectTransform.sizeDelta.y, image.rectTransform.sizeDelta.x);

                    Destroy(GameObject.Find($"{image.name} DestroyObject"));
                    break;
            }
        }
    }
}