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

        public GamePhase CurrentPhase => PartyRoster.CurrentPhase;
        public string CurrentEncounter => PartyRoster.CurrentEncounter;
        public bool IsInBattle => PartyRoster.CurrentPhase == GamePhase.Battle;

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
            PartyRoster.SetActiveMember(index);
        }

        public PartyMemberData GetActiveMember()
        {
            return PartyRoster.GetActiveMember();
        }

        public void StartBattle(string encounterName)
        {
            PartyRoster.CurrentPhase = GamePhase.Battle;
            PartyRoster.CurrentEncounter = encounterName;
        }

        public void EndBattle(bool playerWon)
        {
            PartyRoster.CurrentPhase = GamePhase.Exploration;
            PartyRoster.CurrentEncounter = null;

            if (playerWon)
            {
                foreach (var member in PartyRoster.Members)
                {
                    if (member != null)
                    {
                        member.Reset();
                    }
                }
            }
        }

        public void SyncFromRuntimeParty()
        {
            if (PartyManager.Instance == null)
            {
                return;
            }

            var runtimeMembers = PartyManager.Instance.Members;
            if (runtimeMembers == null || runtimeMembers.Count == 0)
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

            PartyRoster.ActiveMemberIndex = 0;
        }
    }
}
