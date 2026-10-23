using MyUnityGame.Core;
using UnityEngine.UIElements;

namespace MyUnityGame.UI
{
    public class PartyMenuPage : IGameMenuPage
    {
        public string Title => "Party";

        public void Build(VisualElement container)
        {
            var roster = GameManager.Instance != null ? GameManager.Instance.PartyRoster : null;
            if (roster == null || roster.Members == null || roster.Members.Count == 0)
            {
                var empty = new Label("No party loaded.");
                empty.AddToClassList("page__text");
                container.Add(empty);
                return;
            }

            for (var i = 0; i < roster.Members.Count; i++)
            {
                var member = roster.Members[i];
                if (member == null)
                {
                    continue;
                }

                var row = new VisualElement();
                row.AddToClassList("party-row");
                if (i == roster.ActiveMemberIndex)
                {
                    row.AddToClassList("party-row--active");
                }

                var name = new Label(member.Name);
                name.AddToClassList("party-row__name");
                var hp = new Label($"HP {member.CurrentHp}/{member.MaxHp}");
                hp.AddToClassList("party-row__hp");

                row.Add(name);
                row.Add(hp);
                container.Add(row);
            }
        }
    }
}
