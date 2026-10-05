using System.Collections.Generic;

namespace MyUnityGame.Data
{
    public enum GamePhase
    {
        Exploration,
        Battle,
        Party
    }

    [System.Serializable]
    public class PartyMemberData
    {
        public string Name;
        public int MaxHp;
        public int CurrentHp;

        public void Reset()
        {
            CurrentHp = MaxHp;
        }

        public void ApplyDamage(int amount)
        {
            CurrentHp = System.Math.Max(0, CurrentHp - amount);
        }

        public void Heal(int amount)
        {
            CurrentHp = System.Math.Min(MaxHp, CurrentHp + amount);
        }

        public bool IsAlive => CurrentHp > 0;
    }

    [System.Serializable]
    public class PartyRoster
    {
        public List<PartyMemberData> Members = new List<PartyMemberData>();
        public int ActiveMemberIndex;
        public GamePhase CurrentPhase = GamePhase.Exploration;
        public string CurrentEncounter;

        public PartyMemberData GetActiveMember()
        {
            if (Members == null || Members.Count == 0)
            {
                return null;
            }

            if (ActiveMemberIndex < 0)
            {
                ActiveMemberIndex = 0;
            }
            else if (ActiveMemberIndex >= Members.Count)
            {
                ActiveMemberIndex = Members.Count - 1;
            }

            return Members[ActiveMemberIndex];
        }

        public void SetActiveMember(int index)
        {
            if (Members == null || Members.Count == 0)
            {
                return;
            }

            ActiveMemberIndex = index;
            if (ActiveMemberIndex < 0)
            {
                ActiveMemberIndex = 0;
            }
            else if (ActiveMemberIndex >= Members.Count)
            {
                ActiveMemberIndex = Members.Count - 1;
            }
        }

        public void InitializeDefaults()
        {
            if (Members == null)
            {
                Members = new List<PartyMemberData>();
            }

            if (Members.Count != 0)
            {
                return;
            }

            Members = new List<PartyMemberData>
            {
                new PartyMemberData { Name = "Liora", MaxHp = 20, CurrentHp = 20 },
                new PartyMemberData { Name = "Neris", MaxHp = 18, CurrentHp = 18 },
                new PartyMemberData { Name = "Veyra", MaxHp = 22, CurrentHp = 22 },
            };

            ActiveMemberIndex = 0;
            CurrentPhase = GamePhase.Exploration;
            CurrentEncounter = null;
        }
    }
}
