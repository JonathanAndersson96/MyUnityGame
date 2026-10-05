using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private const int PlayerMaxHp = 20;
    private const int DemonMaxHp = 16;

    private bool isActive;
    private int playerHp = PlayerMaxHp;
    private int demonHp = DemonMaxHp;
    private bool playerGuarding;
    private float enemyTurnDelay;
    private bool battleEnded;
    private bool playerWon;
    private GameObject demon;

    public bool IsActive => isActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartBattle()
    {
        if (isActive)
        {
            return;
        }

        demon = GameObject.Find("DemonEncounter");
        isActive = true;
        battleEnded = false;
        playerWon = false;
        playerHp = PlayerMaxHp;
        demonHp = DemonMaxHp;
        playerGuarding = false;
        enemyTurnDelay = 0f;
        Debug.Log("Battle started against the demon.");
    }

    private void Update()
    {
        if (!isActive)
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
        if (!isActive)
        {
            return;
        }

        var box = new Rect(Screen.width * 0.5f - 220f, Screen.height * 0.5f - 140f, 440f, 280f);
        GUI.Box(box, "Demon Encounter");

        GUILayout.BeginArea(box);
        try
        {
            GUILayout.Space(28f);
            GUILayout.Label($"Player HP: {playerHp}/{PlayerMaxHp}");
            GUILayout.Label($"Demon HP: {demonHp}/{DemonMaxHp}");

            if (battleEnded)
            {
                GUILayout.Label("The demon stands down.");
                if (GUILayout.Button("Return to the overworld"))
                {
                    isActive = false;
                    battleEnded = false;

                    if (playerWon && demon != null)
                    {
                        Destroy(demon);
                        demon = null;
                    }
                }
                return;
            }

            if (demonHp <= 0)
            {
                EndBattle(true);
                return;
            }

            if (playerHp <= 0)
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
        demonHp = Mathf.Max(0, demonHp - damage);
        Debug.Log($"Player hits for {damage} damage.");

        if (demonHp <= 0)
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

        playerHp = Mathf.Max(0, playerHp - damage);
        Debug.Log($"Demon hits for {damage} damage.");

        if (playerHp <= 0)
        {
            EndBattle(false);
            return;
        }
    }

    private void EndBattle(bool playerWonResult)
    {
        battleEnded = true;
        isActive = true;
        playerWon = playerWonResult;

        if (playerWonResult && demon != null)
        {
            Destroy(demon);
            demon = null;
        }

        Debug.Log(playerWonResult ? "Victory over the demon." : "The player withdraws from the fight.");
    }
}
