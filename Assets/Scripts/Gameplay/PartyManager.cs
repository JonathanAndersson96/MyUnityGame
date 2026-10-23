using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyUnityGame.Core;
using MyUnityGame.Data;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    public class PartyManager : MonoBehaviour
    {
        public static PartyManager Instance { get; private set; }

        private readonly List<PartyMember> members = new List<PartyMember>();
        private int activeIndex;
        private ReadOnlyCollection<PartyMember> membersView;

        public IReadOnlyList<PartyMember> Members => membersView ?? (membersView = members.AsReadOnly());
        public int ActiveIndex => activeIndex;
        public PartyMember ActiveMember => members.Count == 0 ? null : members[activeIndex];

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.SetParent(null, true);
            DontDestroyOnLoad(gameObject);

            var roster = GameManager.Instance != null ? GameManager.Instance.PartyRoster : null;
            if (roster != null && roster.Members != null && roster.Members.Count > 0)
            {
                InitializeFromRoster(roster);
            }
            else if (members.Count == 0)
            {
                InitializeDefaultParty();
            }

            GameManager.Instance?.SyncFromRuntimeParty();
        }

        public void InitializeDefaultParty()
        {
            var defaultRoster = new PartyRoster();
            defaultRoster.InitializeDefaults();
            InitializeFromRoster(defaultRoster);
        }

        private void InitializeFromRoster(PartyRoster roster)
        {
            members.Clear();
            var selectedIndex = -1;
            for (var i = 0; i < roster.Members.Count; i++)
            {
                var data = roster.Members[i];
                if (data == null || string.IsNullOrWhiteSpace(data.Name) || data.MaxHp <= 0)
                {
                    Debug.LogWarning("Skipping an invalid party member while initializing the runtime party.", this);
                    continue;
                }

                var member = new PartyMember(data.Name, data.MaxHp);
                member.ApplyDamage(data.MaxHp - Mathf.Clamp(data.CurrentHp, 0, data.MaxHp));
                if (i == roster.ActiveMemberIndex)
                {
                    selectedIndex = members.Count;
                }

                members.Add(member);
            }

            if (members.Count == 0)
            {
                InitializeDefaultParty();
                return;
            }

            activeIndex = selectedIndex >= 0
                ? selectedIndex
                : Mathf.Clamp(roster.ActiveMemberIndex, 0, members.Count - 1);
        }

        public void SetActiveMember(int index)
        {
            if (index < 0 || index >= members.Count)
            {
                return;
            }

            activeIndex = index;
        }

        public void NextMember()
        {
            if (members.Count == 0)
            {
                return;
            }

            activeIndex = (activeIndex + 1) % members.Count;
        }

        public void ResetParty()
        {
            foreach (var member in members)
            {
                member.Reset();
            }
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
