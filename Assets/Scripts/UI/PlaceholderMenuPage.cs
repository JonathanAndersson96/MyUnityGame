using System;
using UnityEngine.UIElements;

namespace MyUnityGame.UI
{
    public class PlaceholderMenuPage : IGameMenuPage
    {
        public PlaceholderMenuPage(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("A menu page needs a title.", nameof(title));
            }

            Title = title;
        }

        public string Title { get; }

        public void Build(VisualElement container)
        {
            var text = new Label($"{Title} is not built yet.");
            text.AddToClassList("page__text");
            container.Add(text);
        }
    }
}
