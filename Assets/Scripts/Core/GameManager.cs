using System;
using System.Collections.Generic;
using MyUnityGame.Data;
using MyUnityGame.Gameplay;
using UnityEngine;

namespace MyUnityGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public PartyRoster PartyRoster { get; private set; } = new PartyRoster();
        private readonly HashSet<string> defeatedEncounters = new HashSet<string>(StringComparer.Ordinal);
        private GamePhase phaseBeforeMenu;

        public GamePhase CurrentPhase => PartyRoster.CurrentPhase;
        public string CurrentEncounter => PartyRoster.CurrentEncounter;
        public bool IsInBattle => PartyRoster.CurrentPhase == GamePhase.Battle;
        public bool IsMenuOpen => PartyRoster.CurrentPhase == GamePhase.Menu;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultState();
        }

        public void InitializeDefaultState()
        {
            if (PartyRoster == null)
            {
                PartyRoster = new PartyRoster();
            }

            PartyRoster.InitializeDefaults();
            SyncFromRuntimeParty();
        }

        public void SetActiveMember(int index)
        {
            var runtimeParty = PartyManager.Instance;
            if (runtimeParty != null && (index < 0 || index >= runtimeParty.Members.Count))
            {
                return;
            }

            if (runtimeParty != null)
            {
                runtimeParty.SetActiveMember(index);
                SyncFromRuntimeParty();
                return;
            }

            PartyRoster.SetActiveMember(index);
        }

        public PartyMemberData GetActiveMember()
        {
            return PartyRoster.GetActiveMember();
        }

        public void StartBattle(string encounterName)
        {
            if (string.IsNullOrWhiteSpace(encounterName))
            {
                throw new ArgumentException("An encounter name is required to start a battle.", nameof(encounterName));
            }

            SetMenuOpen(false);
            PartyRoster.CurrentPhase = GamePhase.Battle;
            PartyRoster.CurrentEncounter = encounterName;
        }

        public bool SetMenuOpen(bool isOpen)
        {
            if (isOpen && IsInBattle)
            {
                return false;
            }

            if (isOpen)
            {
                if (!IsMenuOpen)
                {
                    phaseBeforeMenu = CurrentPhase;
                }

                PartyRoster.CurrentPhase = GamePhase.Menu;
            }
            else if (IsMenuOpen)
            {
                PartyRoster.CurrentPhase = phaseBeforeMenu;
            }

            return true;
        }

        public void PrepareEncounter(string encounterName)
        {
            if (string.IsNullOrWhiteSpace(encounterName))
            {
                throw new ArgumentException("An encounter name is required.", nameof(encounterName));
            }

            PartyRoster.CurrentEncounter = encounterName;
        }

        public void EndBattle(bool playerWon)
        {
            var encounterName = PartyRoster.CurrentEncounter;
            SetMenuOpen(false);
            PartyRoster.CurrentPhase = GamePhase.Exploration;
            PartyRoster.CurrentEncounter = null;

            if (playerWon && !string.IsNullOrWhiteSpace(encounterName))
            {
                defeatedEncounters.Add(encounterName);
            }
        }

        public bool IsEncounterDefeated(string encounterName)
        {
            return !string.IsNullOrWhiteSpace(encounterName) &&
                   defeatedEncounters.Contains(encounterName);
        }

        public void SyncFromRuntimeParty()
        {
            if (PartyManager.Instance == null)
            {
                return;
            }

            var runtimeMembers = PartyManager.Instance.Members;
            if (runtimeMembers.Count == 0)
            {
                return;
            }

            PartyRoster.Members = new List<PartyMemberData>();
            foreach (var member in runtimeMembers)
            {
                PartyRoster.Members.Add(new PartyMemberData
                {
                    Name = member.Name,
                    MaxHp = member.MaxHp,
                    CurrentHp = member.CurrentHp,
                });
            }

            PartyRoster.ActiveMemberIndex = PartyManager.Instance.ActiveIndex;
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
