namespace MyUnityGame.Gameplay
{
    public class PartyMember
    {
        public string Name { get; }
        public int MaxHp { get; }
        public int CurrentHp { get; private set; }

        public PartyMember(string name, int maxHp)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("A party member must have a name.", nameof(name));
            }

            if (maxHp <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(maxHp), "Maximum HP must be greater than zero.");
            }

            Name = name;
            MaxHp = maxHp;
            CurrentHp = maxHp;
        }

        public void Reset()
        {
            CurrentHp = MaxHp;
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentHp = amount >= CurrentHp ? 0 : CurrentHp - amount;
        }

        public void Heal(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentHp = amount >= MaxHp - CurrentHp ? MaxHp : CurrentHp + amount;
        }

        public bool IsAlive => CurrentHp > 0;

        public string HpText => $"{Name} HP: {CurrentHp}/{MaxHp}";
    }
}
