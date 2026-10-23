using System;
using System.Collections.Generic;
using MyUnityGame.Battle;
using MyUnityGame.Data;
using UnityEngine;

namespace MyUnityGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private readonly HashSet<string> defeatedEncounters = new HashSet<string>(StringComparer.Ordinal);
        private GamePhase phaseBeforeMenu;

        public GamePhase CurrentPhase { get; private set; } = GamePhase.Exploration;
        public string CurrentEncounter { get; private set; }
        public DemonDefinition CurrentWildDemon { get; private set; }
        public bool IsInBattle => CurrentPhase == GamePhase.Battle;
        public bool IsMenuOpen => CurrentPhase == GamePhase.Menu;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartBattle(string encounterName)
        {
            if (string.IsNullOrWhiteSpace(encounterName))
            {
                throw new ArgumentException("An encounter name is required to start a battle.", nameof(encounterName));
            }

            SetMenuOpen(false);
            CurrentPhase = GamePhase.Battle;
            CurrentEncounter = encounterName;
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

                CurrentPhase = GamePhase.Menu;
            }
            else if (IsMenuOpen)
            {
                CurrentPhase = phaseBeforeMenu;
            }

            return true;
        }

        public void PrepareEncounter(string encounterName, DemonDefinition wildDemon)
        {
            if (string.IsNullOrWhiteSpace(encounterName))
            {
                throw new ArgumentException("An encounter name is required.", nameof(encounterName));
            }

            CurrentEncounter = encounterName;
            CurrentWildDemon = wildDemon ?? throw new ArgumentNullException(nameof(wildDemon));
        }

        public void EndBattle(bool playerWon)
        {
            var encounterName = CurrentEncounter;
            SetMenuOpen(false);
            CurrentPhase = GamePhase.Exploration;
            CurrentEncounter = null;
            CurrentWildDemon = null;

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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
