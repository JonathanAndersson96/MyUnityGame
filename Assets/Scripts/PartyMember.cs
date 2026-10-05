using UnityEngine;

public class PartyMember
{
    public string Name { get; }
    public int MaxHp { get; }
    public int CurrentHp { get; private set; }

    public PartyMember(string name, int maxHp)
    {
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
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
    }

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
    }

    public bool IsAlive => CurrentHp > 0;

    public string HpText => $"{Name} HP: {CurrentHp}/{MaxHp}";
}
