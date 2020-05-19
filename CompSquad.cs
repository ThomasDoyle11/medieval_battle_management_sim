using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CompSquad
{
    // List of all CompSquads
    public static List<CompSquad> allCompSquads { get; set; } = new List<CompSquad>();
    private static void AddCompSquad(CompSquad newCompSquad)
    {
        newCompSquad.compSquadID = allCompSquads.Count;
        allCompSquads.Add(newCompSquad);
    }

    // Unique identifer for the CompSquad
    [SerializeField]
    private int _compSquadID;
    public int compSquadID { get { return _compSquadID; } private set { _compSquadID = value; } }
    public static CompSquad GetCompSquad(int compSquadID)
    {
        return allCompSquads[compSquadID];
    }

    // Competition of this CompSquad
    [SerializeField]
    private int _compID;
    public int compID { get { return _compID; } }
    public Competition comp
    {
        get
        {
            return Competition.GetCompetition(compID);
        }
    }

    // Squadron of this CompSquad
    [SerializeField]
    private int _squadronID;
    public int squadronID { get { return _squadronID; } }
    public Squadron squadron
    {
        get
        {
            return Squadron.GetSquadron(squadronID);
        }
    }

    // Points this Squadron has in this Competition
    [SerializeField]
    private int _points;
    public int points { get { return _points; } private set { _points = value; } }
    public void AddPoints(int amount)
    {
        points += amount;
    }

    // Rounds of this Competition this Squadron is involved in
    [SerializeField]
    private List<int> _roundIDs;
    public List<int> roundIDs { get { return _roundIDs; } private set { _roundIDs = value; } }
    public Round[] rounds
    {
        get
        {
            return roundIDs.Select(o => Round.GetRound(o)).ToArray();
        }
    }
    public void AddRound(Round newRound)
    {
        roundIDs.Add(newRound.roundID);
    }

    // Current rank of the Squadron in the Competition
    [SerializeField]
    private int _rank;
    public int rank
    {
        get { return _rank; }
        set
        {
            if (value >= 0 && value <= comp.compSquads.Length)
            {
                _rank = value;
            }
            else
            {
                Debug.Log("Value must be a positive integer less than the size of the competition.");
                value = 0;
            }

        }
    }

    // Battles won in this Competition by the Squadron
    [SerializeField]
    private int _battlesWon;
    public int battlesWon { get { return _battlesWon; } private set { _battlesWon = value; } }
    public void IncreaseBattlesWon()
    {
        battlesWon += 1;
        squadron.IncreaseSeasonBattlesWon();
    }

    // Battles lost in this Competition by the Squadron
    [SerializeField]
    private int _battlesLost;
    public int battlesLost { get { return _battlesLost; } private set { _battlesLost = value; } }
    public void IncreaseBattlesLost()
    {
        battlesLost += 1;
        squadron.IncreaseSeasonBattlesLost();
    }

    // Constructor for creating a CompSquad
    public CompSquad(Competition comp, Squadron squadron)
    {
        AddCompSquad(this);
        _compID = comp.compID;
        _squadronID = squadron.squadronID;
        _roundIDs = new List<int>();
        points = 0;
    }
}
