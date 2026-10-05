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

        private const int DemonMaxHp = 16;

        private BattleState battleState;
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
            playerMember = GetActivePartyMember();
            demonMember = new PartyMember("Demon", DemonMaxHp);

            if (playerMember == null)
            {
                playerMember = new PartyMember("Player", 20);
            }

            battleState = BattleState.Active;
            playerWon = false;
            playerMember.Reset();
            demonMember.Reset();
            playerGuarding = false;
            enemyTurnDelay = 0f;
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

            var box = new Rect(Screen.width * 0.5f - 220f, Screen.height * 0.5f - 140f, 440f, 280f);
            GUI.Box(box, "Demon Encounter");

            GUILayout.BeginArea(box);
            try
            {
                GUILayout.Space(28f);

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

                GUILayout.Label(demonMember.HpText);

                if (battleState == BattleState.Resolved)
                {
                    GUILayout.Label(playerWon ? "The demon stands down." : "The player withdraws from the fight.");
                    if (GUILayout.Button("Return to the overworld"))
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

                if (GUILayout.Button("Attack"))
                {
                    PlayerAttack();
                }

                if (GUILayout.Button("Guard"))
                {
                    playerGuarding = true;
                    StartEnemyTurn();
                }

                if (GUILayout.Button("Flee"))
                {
                    EndBattle(false);
                }
            }
            finally
            {
                GUILayout.EndArea();
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

            StartEnemyTurn();
        }

        private void StartEnemyTurn()
        {
            enemyTurnDelay = 0.6f;
        }

        private void EnemyTurn()
        {
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
            battleState = BattleState.Resolved;
            playerWon = playerWonResult;

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
