using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }

    private readonly List<PartyMember> members = new List<PartyMember>();
    private int activeIndex;

    public IReadOnlyList<PartyMember> Members => members;
    public PartyMember ActiveMember => members.Count == 0 ? null : members[activeIndex];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (members.Count == 0)
        {
            InitializeDefaultParty();
        }
    }

    public void InitializeDefaultParty()
    {
        members.Clear();
        members.Add(new PartyMember("Liora", 20));
        members.Add(new PartyMember("Neris", 18));
        members.Add(new PartyMember("Veyra", 22));
        activeIndex = 0;
    }

    public void SetActiveMember(int index)
    {
        if (members.Count == 0)
        {
            return;
        }

        activeIndex = Mathf.Clamp(index, 0, members.Count - 1);
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
}
