using UnityEngine.UIElements;

namespace MyUnityGame.UI
{
    public interface IGameMenuPage
    {
        string Title { get; }

        // Called every time the page opens, so it always shows current data.
        void Build(VisualElement container);
    }
}
