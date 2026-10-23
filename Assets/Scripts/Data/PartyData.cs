using System.Collections.Generic;

namespace MyUnityGame.Data
{
    public enum GamePhase
    {
        Exploration,
        Battle,
        Menu
    }

    [System.Serializable]
    public class PartyMemberData
    {
        public string Name;
        public int MaxHp;
        public int CurrentHp;

        public void Reset()
        {
            CurrentHp = MaxHp > 0 ? MaxHp : 0;
        }

        public void ApplyDamage(int amount)
        {
            if (MaxHp <= 0)
            {
                CurrentHp = 0;
                return;
            }

            CurrentHp = System.Math.Max(0, System.Math.Min(MaxHp, CurrentHp));
            if (amount > 0)
            {
                CurrentHp = amount >= CurrentHp ? 0 : CurrentHp - amount;
            }
        }

        public void Heal(int amount)
        {
            if (MaxHp <= 0)
            {
                CurrentHp = 0;
                return;
            }

            CurrentHp = System.Math.Max(0, System.Math.Min(MaxHp, CurrentHp));
            if (amount > 0)
            {
                CurrentHp = amount >= MaxHp - CurrentHp ? MaxHp : CurrentHp + amount;
            }
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

            ActiveMemberIndex = ClampActiveMemberIndex(ActiveMemberIndex);
            return Members[ActiveMemberIndex];
        }

        public void SetActiveMember(int index)
        {
            if (Members == null || Members.Count == 0)
            {
                return;
            }

            ActiveMemberIndex = ClampActiveMemberIndex(index);
        }

        public void InitializeDefaults()
        {
            if (Members == null)
            {
                Members = new List<PartyMemberData>();
            }

            for (var i = Members.Count - 1; i >= 0; i--)
            {
                var member = Members[i];
                if (member == null || string.IsNullOrWhiteSpace(member.Name) || member.MaxHp <= 0)
                {
                    Members.RemoveAt(i);
                    continue;
                }

                member.CurrentHp = System.Math.Max(0, System.Math.Min(member.MaxHp, member.CurrentHp));
            }

            if (Members.Count == 0)
            {
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

            ActiveMemberIndex = ClampActiveMemberIndex(ActiveMemberIndex);
            if (!System.Enum.IsDefined(typeof(GamePhase), CurrentPhase))
            {
                CurrentPhase = GamePhase.Exploration;
                CurrentEncounter = null;
            }
        }

        private int ClampActiveMemberIndex(int index)
        {
            return System.Math.Max(0, System.Math.Min(index, Members.Count - 1));
        }
    }
}
