using UnityEngine;

namespace MyUnityGame.Gameplay
{
    [RequireComponent(typeof(BattleManager))]
    public class BattleHud : MonoBehaviour
    {
        private enum Menu
        {
            Main,
            Fight,
            Switch,
            Bag
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

            var width = Mathf.Min(440f, Mathf.Max(220f, Screen.width - 32f));
            var height = Mathf.Min(360f, Mathf.Max(220f, Screen.height - 32f));
            var box = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            var content = new Rect(box.x + 20f, box.y + 38f, box.width - 40f, box.height - 54f);
            var buttonWidth = (content.width - 8f) * 0.5f;
            GUI.Box(box, battle.DemonMember != null ? $"{battle.DemonMember.Name} Encounter" : "Battle");

            GUILayout.BeginArea(content);
            try
            {
                DrawPartyStatus();

                if (battle.DemonMember != null)
                {
                    GUILayout.Label(battle.DemonMember.HpText, GUILayout.Height(24f));
                }

                if (battle.IsResolved)
                {
                    DrawBattleResult();
                    return;
                }

                if (battle.IsEnemyTurnPending)
                {
                    GUILayout.Label("The demon is preparing to attack...");
                    return;
                }

                switch (currentMenu)
                {
                    case Menu.Main:
                        DrawMainMenu(buttonWidth);
                        break;
                    case Menu.Fight:
                        DrawFightMenu();
                        break;
                    case Menu.Switch:
                        DrawSwitchMenu();
                        break;
                    case Menu.Bag:
                        DrawBagMenu();
                        break;
                }
            }
            finally
            {
                GUILayout.EndArea();
            }
        }

        private void DrawPartyStatus()
        {
            var members = battle.PartyMembers;
            if (members == null || members.Count == 0)
            {
                if (battle.PlayerMember != null)
                {
                    GUILayout.Label(battle.PlayerMember.HpText);
                }

                return;
            }

            foreach (var member in members)
            {
                if (member != null)
                {
                    GUILayout.Label(member.HpText);
                }
            }
        }

        private void DrawBattleResult()
        {
            GUILayout.Label(
                battle.PlayerWon
                    ? "The demon stands down."
                    : battle.PlayerMember == null
                        ? "No living party members remain."
                        : "The player withdraws from the fight.",
                GUILayout.Height(30f));

            if (GUILayout.Button("Return to the overworld", GUILayout.Height(42f)))
            {
                battle.ReturnToWorld();
            }
        }

        private void DrawMainMenu(float buttonWidth)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Fight", GUILayout.Width(buttonWidth), GUILayout.Height(42f)))
            {
                currentMenu = Menu.Fight;
            }

            if (GUILayout.Button("Flee", GUILayout.Width(buttonWidth), GUILayout.Height(42f)))
            {
                currentMenu = Menu.Main;
                battle.Flee();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Switch", GUILayout.Width(buttonWidth), GUILayout.Height(42f)))
            {
                currentMenu = Menu.Switch;
            }

            if (GUILayout.Button("Bag", GUILayout.Width(buttonWidth), GUILayout.Height(42f)))
            {
                currentMenu = Menu.Bag;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawFightMenu()
        {
            if (GUILayout.Button("Attack", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                battle.Attack();
                currentMenu = Menu.Main;
            }

            if (GUILayout.Button("Guard", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                battle.Guard();
                currentMenu = Menu.Main;
            }

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                currentMenu = Menu.Main;
            }
        }

        private void DrawSwitchMenu()
        {
            var members = battle.PartyMembers;
            if (members == null || members.Count == 0)
            {
                GUILayout.Label("No party members are available to switch to.");
            }
            else
            {
                for (var i = 0; i < members.Count; i++)
                {
                    var member = members[i];
                    if (member == null)
                    {
                        continue;
                    }

                    var label = $"{member.Name} HP: {member.CurrentHp}/{member.MaxHp}";
                    if (member == battle.PlayerMember)
                    {
                        GUILayout.Label($"{label} (Active)");
                    }
                    else if (!member.IsAlive)
                    {
                        GUILayout.Label($"{label} (Unable to fight)");
                    }
                    else if (GUILayout.Button(label, GUILayout.ExpandWidth(true), GUILayout.Height(38f)) &&
                             battle.SwitchMember(i))
                    {
                        currentMenu = Menu.Main;
                        return;
                    }
                }
            }

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                currentMenu = Menu.Main;
            }
        }

        private void DrawBagMenu()
        {
            GUILayout.Label("Your bag is empty.");
            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                currentMenu = Menu.Main;
            }
        }
    }
}
