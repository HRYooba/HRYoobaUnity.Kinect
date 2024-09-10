using UnityEngine;
using UnityEngine.UI;
using R3;

namespace HRYooba.Kinect.Presentations
{
    [RequireComponent(typeof(Button))]
    public class JointVisibleButton : MonoBehaviour, IJointViewState
    {
        private ReactiveProperty<bool> _isVisible = new();
        public ReadOnlyReactiveProperty<bool> IsVisible => _isVisible;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.OnClickAsObservable()
                .Subscribe(_ => _isVisible.Value = !_isVisible.Value)
                .AddTo(this);
        }
    }
}