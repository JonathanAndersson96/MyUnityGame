using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private enum BattleState
    {
        Inactive,
        Active,
        Resolved
    }

    private class CombatantStats
    {
        public string Name { get; }
        public int MaxHp { get; }
        public int CurrentHp { get; private set; }

        public CombatantStats(string name, int maxHp)
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

        public bool IsAlive => CurrentHp > 0;

        public string HpText => $"{Name} HP: {CurrentHp}/{MaxHp}";
    }

    private const int PlayerMaxHp = 20;
    private const int DemonMaxHp = 16;

    private BattleState battleState;
    private CombatantStats playerStats;
    private CombatantStats demonStats;
    private bool playerGuarding;
    private float enemyTurnDelay;
    private bool playerWon;
    private GameObject demonObject;

    public bool IsActive => battleState == BattleState.Active || battleState == BattleState.Resolved;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        playerStats = new CombatantStats("Player", PlayerMaxHp);
        demonStats = new CombatantStats("Demon", DemonMaxHp);
    }

    public void StartBattle()
    {
        if (battleState == BattleState.Active || battleState == BattleState.Resolved)
        {
            return;
        }

        demonObject = GameObject.Find("DemonEncounter");
        battleState = BattleState.Active;
        playerWon = false;
        playerStats.Reset();
        demonStats.Reset();
        playerGuarding = false;
        enemyTurnDelay = 0f;
        Debug.Log("Battle started against the demon.");
    }

    private void Update()
    {
        if (battleState != BattleState.Active)
        {
            return;
        }

        if (enemyTurnDelay > 0f)
        {
            enemyTurnDelay -= Time.deltaTime;
            if (enemyTurnDelay <= 0f)
            {
                EnemyTurn();
            }
        }
    }

    private void OnGUI()
    {
        if (battleState == BattleState.Inactive)
        {
            return;
        }

        var box = new Rect(Screen.width * 0.5f - 220f, Screen.height * 0.5f - 140f, 440f, 280f);
        GUI.Box(box, "Demon Encounter");

        GUILayout.BeginArea(box);
        try
        {
            GUILayout.Space(28f);
            GUILayout.Label(playerStats.HpText);
            GUILayout.Label(demonStats.HpText);

            if (battleState == BattleState.Resolved)
            {
                GUILayout.Label(playerWon ? "The demon stands down." : "The player withdraws from the fight.");
                if (GUILayout.Button("Return to the overworld"))
                {
                    battleState = BattleState.Inactive;

                    if (playerWon && demonObject != null)
                    {
                        Destroy(demonObject);
                        demonObject = null;
                    }
                }
                return;
            }

            if (!demonStats.IsAlive)
            {
                EndBattle(true);
                return;
            }

            if (!playerStats.IsAlive)
            {
                EndBattle(false);
                return;
            }

            if (GUILayout.Button("Attack"))
            {
                PlayerAttack();
            }

            if (GUILayout.Button("Guard"))
            {
                playerGuarding = true;
                StartEnemyTurn();
            }

            if (GUILayout.Button("Flee"))
            {
                EndBattle(false);
            }
        }
        finally
        {
            GUILayout.EndArea();
        }
    }

    private void PlayerAttack()
    {
        var damage = Random.Range(4, 9);
        demonStats.ApplyDamage(damage);
        Debug.Log($"Player hits for {damage} damage.");

        if (!demonStats.IsAlive)
        {
            EndBattle(true);
            return;
        }

        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        enemyTurnDelay = 0.6f;
    }

    private void EnemyTurn()
    {
        var damage = Random.Range(3, 7);
        if (playerGuarding)
        {
            damage = Mathf.Max(1, damage / 2);
            playerGuarding = false;
        }

        playerStats.ApplyDamage(damage);
        Debug.Log($"Demon hits for {damage} damage.");

        if (!playerStats.IsAlive)
        {
            EndBattle(false);
        }
    }

    private void EndBattle(bool playerWonResult)
    {
        battleState = BattleState.Resolved;
        playerWon = playerWonResult;

        if (playerWonResult && demonObject != null)
        {
            Destroy(demonObject);
            demonObject = null;
        }

        Debug.Log(playerWonResult ? "Victory over the demon." : "The player withdraws from the fight.");
    }
}
