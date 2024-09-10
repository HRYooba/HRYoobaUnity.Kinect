using R3;

namespace HRYooba.Kinect.Presentations
{
    public interface IJointViewState
    {
        ReadOnlyReactiveProperty<bool> IsVisible { get; }
    }
}