using System;
using System.Collections.Generic;
using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    [RequireComponent(typeof(BattleHud))]
    public class BattleManager : MonoBehaviour
    {
        private enum BattleState
        {
            Inactive,
            Active,
            Resolved
        }

        private const string RulesResourcePath = "Gameplay/BattleRules";

        public static BattleManager Instance { get; private set; }

        [SerializeField] private BattleRules rules;

        private BattleState battleState;
        private PartyMember playerMember;
        private PartyMember demonMember;
        private bool playerGuarding;
        private float enemyTurnTimer;
        private bool playerWon;
        private GameObject demonObject;

        public bool IsActive => battleState == BattleState.Active || battleState == BattleState.Resolved;
        public bool IsResolved => battleState == BattleState.Resolved;
        public bool PlayerWon => playerWon;
        public bool IsEnemyTurnPending => enemyTurnTimer > 0f;
        public PartyMember PlayerMember => playerMember;
        public PartyMember DemonMember => demonMember;
        public IReadOnlyList<PartyMember> PartyMembers =>
            PartyManager.Instance != null ? PartyManager.Instance.Members : null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (rules == null)
            {
                rules = Resources.Load<BattleRules>(RulesResourcePath);
            }

            if (rules == null)
            {
                Debug.LogError($"Battle rules are missing from Resources/{RulesResourcePath}.", this);
                DisableManager();
            }
            else if (!rules.TryValidate(out var validationError))
            {
                Debug.LogError($"Battle rules are invalid: {validationError}", rules);
                DisableManager();
            }
        }

        public bool StartBattle(string encounterName = "Demon")
        {
            if (!enabled || rules == null)
            {
                return false;
            }

            if (battleState == BattleState.Active || battleState == BattleState.Resolved)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(encounterName))
            {
                Debug.LogError("Cannot start a battle without an encounter name.", this);
                return false;
            }

            var party = PartyManager.Instance;
            playerMember = party != null ? party.ActiveMember : null;
            if (party != null && (playerMember == null || !playerMember.IsAlive))
            {
                playerMember = null;
                for (var i = 0; i < party.Members.Count; i++)
                {
                    if (party.Members[i] != null && party.Members[i].IsAlive)
                    {
                        party.SetActiveMember(i);
                        playerMember = party.ActiveMember;
                        break;
                    }
                }

                if (playerMember == null)
                {
                    demonMember = new PartyMember(encounterName, rules.OpponentMaxHp);
                    GameManager.Instance?.StartBattle(encounterName);
                    Debug.LogWarning("The party has no living members and cannot fight.", this);
                    EndBattle(false);
                    return true;
                }
            }

            playerMember ??= new PartyMember("Player", 20);
            GameManager.Instance?.StartBattle(encounterName);
            demonMember = new PartyMember(encounterName, rules.OpponentMaxHp);

            battleState = BattleState.Active;
            playerWon = false;
            demonMember.Reset();
            playerGuarding = false;
            enemyTurnTimer = 0f;
            SyncPartyState();
            Debug.Log($"Battle started against {encounterName}.", this);
            return true;
        }

        private void Update()
        {
            if (battleState != BattleState.Active || enemyTurnTimer <= 0f)
            {
                return;
            }

            enemyTurnTimer = Mathf.Max(0f, enemyTurnTimer - Time.deltaTime);
            if (enemyTurnTimer <= 0f)
            {
                PerformEnemyTurn();
            }
        }

        public void Attack()
        {
            if (battleState != BattleState.Active || IsEnemyTurnPending)
            {
                return;
            }

            var damage = UnityEngine.Random.Range(rules.PlayerMinimumDamage, rules.PlayerMaximumDamage + 1);
            demonMember.ApplyDamage(damage);
            Debug.Log($"Player hits for {damage} damage.", this);

            if (!demonMember.IsAlive)
            {
                EndBattle(true);
                return;
            }

            StartEnemyTurn();
        }

        public void Guard()
        {
            if (battleState != BattleState.Active || IsEnemyTurnPending)
            {
                return;
            }

            playerGuarding = true;
            StartEnemyTurn();
        }

        public void Flee()
        {
            if (battleState == BattleState.Active)
            {
                EndBattle(false);
            }
        }

        public bool SwitchMember(int index)
        {
            var party = PartyManager.Instance;
            if (battleState != BattleState.Active || IsEnemyTurnPending || party == null ||
                index < 0 || index >= party.Members.Count)
            {
                return false;
            }

            var member = party.Members[index];
            if (member == null || member == playerMember || !member.IsAlive)
            {
                return false;
            }

            playerMember = member;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetActiveMember(index);
            }
            else
            {
                party.SetActiveMember(index);
            }
            StartEnemyTurn();
            return true;
        }

        public void SetEncounterObject(GameObject encounterObject)
        {
            if (battleState != BattleState.Inactive)
            {
                throw new InvalidOperationException("The encounter object can only be set before a battle starts.");
            }

            demonObject = encounterObject != null
                ? encounterObject
                : throw new ArgumentNullException(nameof(encounterObject));
        }

        public void ReturnToWorld()
        {
            if (!IsResolved)
            {
                return;
            }

            const string worldScenePath = "Assets/Scenes/SampleScene.unity";
            var worldSceneIndex = SceneUtility.GetBuildIndexByScenePath(worldScenePath);
            if (worldSceneIndex < 0)
            {
                Debug.LogError($"World scene is not available in Build Settings: {worldScenePath}", this);
                return;
            }

            battleState = BattleState.Inactive;
            if (playerWon && demonObject != null)
            {
                Destroy(demonObject);
                demonObject = null;
            }

            SceneManager.LoadScene(worldSceneIndex);
        }

        private void StartEnemyTurn()
        {
            enemyTurnTimer = rules.EnemyTurnDelay;
            if (enemyTurnTimer <= 0f)
            {
                PerformEnemyTurn();
            }
        }

        private void PerformEnemyTurn()
        {
            if (playerMember == null)
            {
                Debug.LogError("Cannot perform an enemy turn without an active party member.", this);
                EndBattle(false);
                return;
            }

            var damage = UnityEngine.Random.Range(rules.EnemyMinimumDamage, rules.EnemyMaximumDamage + 1);
            if (playerGuarding)
            {
                damage = Mathf.Max(rules.MinimumGuardDamage, damage / rules.GuardDamageDivisor);
                playerGuarding = false;
            }

            playerMember.ApplyDamage(damage);
            SyncPartyState();
            Debug.Log($"Demon hits for {damage} damage.", this);

            if (!playerMember.IsAlive)
            {
                EndBattle(false);
            }
        }

        private void EndBattle(bool won)
        {
            if (battleState == BattleState.Resolved)
            {
                return;
            }

            battleState = BattleState.Resolved;
            playerWon = won;
            playerGuarding = false;
            enemyTurnTimer = 0f;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndBattle(won);
                SyncPartyState();
            }

            if (won && demonObject != null)
            {
                Destroy(demonObject);
                demonObject = null;
            }

            Debug.Log(won ? "Victory over the demon." : "The player withdraws from the fight.", this);
        }

        private void SyncPartyState()
        {
            GameManager.Instance?.SyncFromRuntimeParty();
        }

        private void DisableManager()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            enabled = false;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
