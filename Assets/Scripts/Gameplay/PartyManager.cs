using System;
using MyUnityGame.Battle;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    // Keeps the player's party of demons alive across scene loads.
    public class PartyManager : MonoBehaviour
    {
        private const string StartingPartyResourcePath = "Battle/StartingParty";
        private const string TuningResourcePath = "Battle/BattleTuning";

        public static PartyManager Instance { get; private set; }

        public Party Party { get; private set; }
        public BattleTuning Tuning { get; private set; }

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

            Tuning = Resources.Load<BattleTuning>(TuningResourcePath);
            if (Tuning == null)
            {
                Debug.LogError($"Battle tuning is missing from Resources/{TuningResourcePath}.", this);
            }

            ResetToStartingParty();
        }

        public void ResetToStartingParty()
        {
            var startingParty = Resources.Load<PartyDefinition>(StartingPartyResourcePath);
            if (startingParty == null)
            {
                Debug.LogError($"The starting party is missing from Resources/{StartingPartyResourcePath}.", this);
                Party = new Party(Array.Empty<Demon>());
                return;
            }

            if (!startingParty.TryCreateParty(out var party, out var error))
            {
                Debug.LogError($"The starting party is invalid: {error}", startingParty);
                Party = new Party(Array.Empty<Demon>());
                return;
            }

            Party = party;
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
