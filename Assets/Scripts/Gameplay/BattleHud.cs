using MyUnityGame.Battle;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    [RequireComponent(typeof(BattleManager))]
    public class BattleHud : MonoBehaviour
    {
        private enum Menu
        {
            Main,
            Fight
        }

        private BattleManager battle;
        private Menu currentMenu;

        private void Awake()
        {
            battle = GetComponent<BattleManager>();
        }

        private void OnGUI()
        {
            if (battle == null || !battle.IsActive)
            {
                return;
            }

            var width = Mathf.Min(480f, Mathf.Max(240f, Screen.width - 32f));
            var height = Mathf.Min(440f, Mathf.Max(260f, Screen.height - 32f));
            var box = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            var content = new Rect(box.x + 20f, box.y + 38f, box.width - 40f, box.height - 54f);

            GUI.Box(box, $"Wild {battle.OpponentDemon.Species.DisplayName}");
            GUILayout.BeginArea(content);
            try
            {
                DrawDemon(battle.OpponentDemon);
                DrawDemon(battle.PlayerDemon);
                GUILayout.Space(8f);
                foreach (var line in battle.Messages)
                {
                    GUILayout.Label(line);
                }

                GUILayout.FlexibleSpace();
                if (battle.IsResolved)
                {
                    DrawBattleResult();
                }
                else if (currentMenu == Menu.Fight)
                {
                    DrawFightMenu();
                }
                else
                {
                    DrawMainMenu();
                }
            }
            finally
            {
                GUILayout.EndArea();
            }
        }

        private void DrawDemon(Demon demon)
        {
            var hp = battle.Tuning != null
                ? $"HP {demon.GetCurrentHp(battle.Tuning)}/{demon.GetStat(StatKind.Health, battle.Tuning)}"
                : string.Empty;
            GUILayout.Label($"{demon.Species.DisplayName} · {demon.Type} · Lv {demon.Level}   {hp}");
        }

        private void DrawMainMenu()
        {
            if (GUILayout.Button("Fight", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                currentMenu = Menu.Fight;
            }
        }

        private void DrawFightMenu()
        {
            var moves = battle.PlayerDemon.Moves;
            for (var i = 0; i < moves.Count; i++)
            {
                var move = moves[i];
                var label = $"{move.DisplayName}  ({move.Type} · {move.Category} · {move.Power})";
                if (GUILayout.Button(label, GUILayout.ExpandWidth(true), GUILayout.Height(38f)))
                {
                    currentMenu = Menu.Main;
                    battle.UseMove(i);
                    return;
                }
            }

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.Height(38f)))
            {
                currentMenu = Menu.Main;
            }
        }

        private void DrawBattleResult()
        {
            GUILayout.Label(battle.Outcome == BattleOutcome.Won
                ? "You won the battle."
                : "You lost the battle.", GUILayout.Height(30f));
            if (GUILayout.Button("Return to the overworld", GUILayout.Height(42f)))
            {
                battle.ReturnToWorld();
            }
        }
    }
}
