using MyUnityGame.Battle;
using MyUnityGame.Gameplay;
using UnityEngine.UIElements;

namespace MyUnityGame.UI
{
    public class PartyMenuPage : IGameMenuPage
    {
        public string Title => "Party";

        public void Build(VisualElement container)
        {
            var partyManager = PartyManager.Instance;
            var members = partyManager != null && partyManager.Party != null ? partyManager.Party.Members : null;
            if (members == null || members.Count == 0)
            {
                var empty = new Label("Your party has no demons.");
                empty.AddToClassList("page__text");
                container.Add(empty);
                return;
            }

            foreach (var demon in members)
            {
                var row = new VisualElement();
                row.AddToClassList("party-row");

                var name = new Label($"{demon.Species.DisplayName} · {demon.Type} · Lv {demon.Level}");
                name.AddToClassList("party-row__name");
                var hp = new Label(HpText(demon, partyManager.Tuning));
                hp.AddToClassList("party-row__hp");

                row.Add(name);
                row.Add(hp);
                container.Add(row);
            }
        }

        private static string HpText(Demon demon, BattleTuning tuning) =>
            tuning != null
                ? $"HP {demon.GetCurrentHp(tuning)}/{demon.GetStat(StatKind.Health, tuning)}"
                : "HP ?";
    }
}
