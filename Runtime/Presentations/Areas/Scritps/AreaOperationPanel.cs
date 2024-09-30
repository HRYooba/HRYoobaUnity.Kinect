using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using HRYooba.UI;

namespace HRYooba.Kinect.Presentations.Areas
{
    public class AreaOperationPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _idText = null;
        [SerializeField] private Vector3Controller _positionController = null;
        [SerializeField] private FloatController _radiusController = null;
        [SerializeField] private Button _saveButton = null;

        public Observable<Vector3> OnPositionChanged => _positionController.OnValueChangedObservable;
        public Observable<float> OnRadiusChanged => _radiusController.OnValueChangedObservable;
        public Observable<Unit> OnSaveButtonClicked => _saveButton.OnClickAsObservable();

        public void SetId(int id)
        {
            _idText.text = $"ID:{id}";
        }

        public void SetPosition(Vector3 position)
        {
            _positionController.Value = position;
        }

        public void SetRadius(float radius)
        {
            _radiusController.Value = radius;
        }
    }
}