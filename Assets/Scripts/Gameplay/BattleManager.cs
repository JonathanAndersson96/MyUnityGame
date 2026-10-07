using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        private enum BattleState
        {
            Inactive,
            Active,
            Resolved
        }

        private enum BattleMenu
        {
            Main,
            Fight,
            Switch,
            Bag
        }

        private const int DemonMaxHp = 16;

        private BattleState battleState;
        private BattleMenu battleMenu;
        private PartyMember playerMember;
        private PartyMember demonMember;
        private bool playerGuarding;
        private float enemyTurnDelay;
        private bool playerWon;
        private GameObject demonObject;

        public bool IsActive => battleState == BattleState.Active || battleState == BattleState.Resolved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private PartyMember GetActivePartyMember()
        {
            if (PartyManager.Instance == null)
            {
                return null;
            }

            return PartyManager.Instance.ActiveMember;
        }

        public void StartBattle()
        {
            if (battleState == BattleState.Active || battleState == BattleState.Resolved)
            {
                return;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartBattle("Demon");
            }

            demonObject = GameObject.Find("DemonEncounter");
            playerMember = GetActivePartyMember() ?? new PartyMember("Player", 20);
            demonMember = new PartyMember("Demon", DemonMaxHp);

            battleState = BattleState.Active;
            playerWon = false;
            playerMember.Reset();
            demonMember.Reset();
            playerGuarding = false;
            enemyTurnDelay = 0f;
            battleMenu = BattleMenu.Main;
            Debug.Log("Battle started against the demon.");
        }

        private void Update()
        {
            if (battleState != BattleState.Active)
            {
                return;
            }

            if (enemyTurnDelay > 0f)
            {
                enemyTurnDelay -= Time.deltaTime;
                if (enemyTurnDelay <= 0f)
                {
                    EnemyTurn();
                }
            }
        }

        private void OnGUI()
        {
            if (battleState == BattleState.Inactive)
            {
                return;
            }

            if (playerMember == null)
            {
                playerMember = GetActivePartyMember() ?? new PartyMember("Player", 20);
            }

            if (demonMember == null)
            {
                demonMember = new PartyMember("Demon", DemonMaxHp);
            }

            var boxWidth = Mathf.Min(440f, Mathf.Max(220f, Screen.width - 32f));
            var boxHeight = Mathf.Min(360f, Mathf.Max(220f, Screen.height - 32f));
            var box = new Rect(
                (Screen.width - boxWidth) * 0.5f,
                (Screen.height - boxHeight) * 0.5f,
                boxWidth,
                boxHeight);
            var content = new Rect(box.x + 20f, box.y + 38f, box.width - 40f, box.height - 54f);
            var buttonWidth = (content.width - 8f) * 0.5f;
            GUI.Box(box, "Demon Encounter");

            GUILayout.BeginArea(content);
            try
            {
                if (PartyManager.Instance != null)
                {
                    foreach (var member in PartyManager.Instance.Members)
                    {
                        GUILayout.Label(member.HpText);
                    }
                }
                else if (playerMember != null)
                {
                    GUILayout.Label(playerMember.HpText);
                }

                GUILayout.Label(demonMember.HpText, GUILayout.Height(24f));

                if (battleState == BattleState.Resolved)
                {
                    GUILayout.Label(playerWon ? "The demon stands down." : "The player withdraws from the fight.", GUILayout.Height(30f));
                    if (GUILayout.Button("Return to the overworld", GUILayout.Height(42f)))
                    {
                        battleState = BattleState.Inactive;

                        if (playerWon && demonObject != null)
                        {
                            Destroy(demonObject);
                            demonObject = null;
                        }

                        SceneManager.LoadScene("SampleScene");
                    }
                    return;
                }

                if (!demonMember.IsAlive)
                {
                    EndBattle(true);
                    return;
                }

                if (!playerMember.IsAlive)
                {
                    EndBattle(false);
                    return;
                }

                if (enemyTurnDelay > 0f)
                {
                    GUILayout.Label("The demon is preparing to attack...");
                    return;
                }

                switch (battleMenu)
                {
                    case BattleMenu.Main:
                        DrawMainMenu(buttonWidth);
                        break;
                    case BattleMenu.Fight:
                        DrawFightMenu();
                        break;
                    case BattleMenu.Switch:
                        DrawSwitchMenu();
                        break;
                    case BattleMenu.Bag:
                        DrawBagMenu();
                        break;
                }
            }
            finally
            {
                GUILayout.EndArea();
            }
        }

        private void DrawMainMenu(float buttonWidth)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Fight", GUILayout.Width(buttonWidth), GUILayout.Height(42f)))
            {
                battleMenu = BattleMenu.Fight;
            }

            if (GUILayout.Button("Flee", GUILayout.Width(buttonWidth), GUILayout.Height(42f)))
            {
                EndBattle(false);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Switch", GUILayout.Width(buttonWidth), GUILayout.Height(42f)))
            {
                battleMenu = BattleMenu.Switch;
            }

            if (GUILayout.Button("Bag", GUILayout.Width(buttonWidth), GUILayout.Height(42f)))
            {
                battleMenu = BattleMenu.Bag;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawFightMenu()
        {
            if (GUILayout.Button("Attack", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                PlayerAttack();
            }

            if (GUILayout.Button("Guard", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                playerGuarding = true;
                StartEnemyTurn();
            }

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                battleMenu = BattleMenu.Main;
            }
        }

        private void DrawSwitchMenu()
        {
            var partyManager = PartyManager.Instance;
            if (partyManager == null || partyManager.Members.Count == 0)
            {
                GUILayout.Label("No party members are available to switch to.");
            }
            else
            {
                for (var i = 0; i < partyManager.Members.Count; i++)
                {
                    var member = partyManager.Members[i];
                    var label = $"{member.Name} HP: {member.CurrentHp}/{member.MaxHp}";

                    if (member == playerMember)
                    {
                        GUILayout.Label($"{label} (Active)");
                    }
                    else if (!member.IsAlive)
                    {
                        GUILayout.Label($"{label} (Unable to fight)");
                    }
                    else if (GUILayout.Button(label, GUILayout.ExpandWidth(true), GUILayout.Height(38f)))
                    {
                        partyManager.SetActiveMember(i);
                        playerMember = member;

                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.SetActiveMember(i);
                        }

                        battleMenu = BattleMenu.Main;
                        StartEnemyTurn();
                        return;
                    }
                }
            }

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                battleMenu = BattleMenu.Main;
            }
        }

        private void DrawBagMenu()
        {
            GUILayout.Label("Your bag is empty.");
            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true), GUILayout.Height(42f)))
            {
                battleMenu = BattleMenu.Main;
            }
        }

        private void PlayerAttack()
        {
            var damage = Random.Range(4, 9);
            demonMember.ApplyDamage(damage);
            Debug.Log($"Player hits for {damage} damage.");

            if (!demonMember.IsAlive)
            {
                EndBattle(true);
                return;
            }

            battleMenu = BattleMenu.Main;
            StartEnemyTurn();
        }

        private void StartEnemyTurn()
        {
            enemyTurnDelay = 0.6f;
        }

        private void EnemyTurn()
        {
            if (playerMember == null)
            {
                playerMember = GetActivePartyMember() ?? new PartyMember("Player", 20);
            }

            var damage = Random.Range(3, 7);
            if (playerGuarding)
            {
                damage = Mathf.Max(1, damage / 2);
                playerGuarding = false;
            }

            playerMember.ApplyDamage(damage);
            Debug.Log($"Demon hits for {damage} damage.");

            if (!playerMember.IsAlive)
            {
                EndBattle(false);
            }
        }

        private void EndBattle(bool playerWonResult)
        {
            if (battleState == BattleState.Resolved)
            {
                return;
            }

            battleState = BattleState.Resolved;
            playerWon = playerWonResult;
            playerGuarding = false;
            enemyTurnDelay = 0f;
            battleMenu = BattleMenu.Main;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndBattle(playerWonResult);
            }

            if (playerWonResult && demonObject != null)
            {
                Destroy(demonObject);
                demonObject = null;
            }

            Debug.Log(playerWonResult ? "Victory over the demon." : "The player withdraws from the fight.");
        }
    }
}
