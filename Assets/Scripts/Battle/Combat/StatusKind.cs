namespace MyUnityGame.Battle
{
    // One status per Element and per spirit-quad type. Durations, numbers and clauses are still open questions,
    // so only the identities and the planned effect of each are recorded here.
    public enum StatusKind
    {
        None,
        Burn,       // Damage over time; lowers Physical and Magic Defense.
        Soak,       // Lowers Speed.
        Trap,       // The target can't switch out.
        Float,      // Contact moves miss the target for 2 turns.
        Drain,      // Drains HP each turn to the demon that applied it.
        Plague,     // Loses HP each turn.
        Confuse,    // Lowers Magic Attack.
        Blind       // The target's attacks miss for 1 turn.
    }

    public static class StatusKinds
    {
        public static StatusKind ForType(DemonType type) => type switch
        {
            DemonType.Fire => StatusKind.Burn,
            DemonType.Water => StatusKind.Soak,
            DemonType.Earth => StatusKind.Trap,
            DemonType.Air => StatusKind.Float,
            DemonType.Life => StatusKind.Drain,
            DemonType.Death => StatusKind.Plague,
            DemonType.Shadow => StatusKind.Confuse,
            DemonType.Light => StatusKind.Blind,
            _ => StatusKind.None
        };
    }
}
