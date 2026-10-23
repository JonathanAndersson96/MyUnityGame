using System.Collections.Generic;
using MyUnityGame.Battle;
using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    // Runs one battle scene's battle: hands the player's choices to the battle and keeps its messages.
    // The rules live in the battle library; this only connects them to the scene.
    [RequireComponent(typeof(BattleHud))]
    public class BattleManager : MonoBehaviour
    {
        private const string WorldScenePath = "Assets/Scenes/SampleScene.unity";

        public static BattleManager Instance { get; private set; }

        private readonly List<string> messages = new List<string>();
        private DemonBattle battle;

        public bool IsActive => battle != null;
        public bool IsResolved => battle != null && battle.Outcome != BattleOutcome.Ongoing;
        public BattleOutcome Outcome => battle != null ? battle.Outcome : BattleOutcome.Ongoing;
        public Demon PlayerDemon => battle?.PlayerDemon;
        public Demon OpponentDemon => battle?.OpponentDemon;
        public BattleTuning Tuning => PartyManager.Instance != null ? PartyManager.Instance.Tuning : null;
        public IReadOnlyList<string> Messages => messages;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool StartBattle(string encounterName, DemonDefinition wildDemonDefinition)
        {
            if (battle != null)
            {
                return false;
            }

            var party = PartyManager.Instance != null ? PartyManager.Instance.Party : null;
            if (party == null || party.Members.Count == 0 || Tuning == null)
            {
                Debug.LogError("Cannot start a battle without a party of demons and battle tuning.", this);
                return false;
            }

            if (!wildDemonDefinition.TryCreate(out var wildDemon, out var error))
            {
                Debug.LogError($"The wild demon is invalid: {error}", this);
                return false;
            }

            battle = new DemonBattle(party, wildDemon, Tuning);
            GameManager.Instance?.StartBattle(encounterName);
            messages.Clear();
            messages.Add($"A wild {wildDemon.Species.DisplayName} appeared!");
            messages.Add($"Go, {battle.PlayerDemon.Species.DisplayName}!");
            return true;
        }

        public void UseMove(int moveIndex)
        {
            if (battle == null || IsResolved || moveIndex < 0 || moveIndex >= battle.PlayerDemon.Moves.Count)
            {
                return;
            }

            messages.Clear();
            foreach (var battleEvent in battle.UseMove(moveIndex))
            {
                BattleMessages.Describe(battleEvent, messages);
            }

            if (IsResolved)
            {
                GameManager.Instance?.EndBattle(battle.Outcome == BattleOutcome.Won);
            }
        }

        public void ReturnToWorld()
        {
            if (!IsResolved)
            {
                return;
            }

            var worldSceneIndex = SceneUtility.GetBuildIndexByScenePath(WorldScenePath);
            if (worldSceneIndex < 0)
            {
                Debug.LogError($"World scene is not available in Build Settings: {WorldScenePath}", this);
                return;
            }

            SceneManager.LoadScene(worldSceneIndex);
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
