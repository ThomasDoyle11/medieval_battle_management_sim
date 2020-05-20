using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Squadron
{
    // List of all Squadrons
    public static List<Squadron> allSquadrons { get; set; } = new List<Squadron>();
    private static void AddSquadron(Squadron newSquadron)
    {
        newSquadron.squadronID = allSquadrons.Count;
        allSquadrons.Add(newSquadron);
    }

    // The unique ID of the Squadron
    [SerializeField]
    private int _squadronID;
    public int squadronID { get { return _squadronID; } private set { _squadronID = value; } }
    public static Squadron GetSquadron(int squadronID)
    {
        if (squadronID != -1)
        {
            return allSquadrons[squadronID];
        }
        else
        {
            return null;
        }
    }

    // The name of the Squadron
    [SerializeField]
    private string _name;
    public string name { get { return _name; } set { _name = value; } }

    // The region in which the Squadron resides
    [SerializeField]
    private int _region;
    public int region { get { return _region; } set { _region = value; } }
    public static Squadron[] GetAllSquadronsInRegion(int region)
    {
        return allSquadrons.FindAll(o => o.region == region).ToArray();
    }

    // The cooridnates of the Squadron's home
    [SerializeField]
    private int _location;
    public int location { get { return _location; } set { _location = value; } }

    // The average Battle Power of the Best Knights this Squadron can field
    [SerializeField]
    private int _battlePower;
    public int battlePower { get { return _battlePower; } private set { _battlePower = value; } }
    public void CalculateBattlePower()
    {
        battlePower = (int)GetTopKnights(Battle.battleSize).Select(o => o.battlePower).Average();
    }

    // The average Attack of the Best Knights this Squadron can field
    [SerializeField]
    private int _attack;
    public int attack { get { return _attack; } private set { _attack = value; } }
    public void CalculateAttack()
    {
        attack = (int)GetTopKnights(Battle.battleSize).Select(o => o.attack).Average();
    }

    // The average Defence of the Best Knights this Squadron can field
    [SerializeField]
    private int _defence;
    public int defence { get { return _defence; } private set { _defence = value; } }
    public void CalculateDefence()
    {
        defence = (int)GetTopKnights(Battle.battleSize).Select(o => o.defence).Average();
    }

    // The average Speed of the Best Knights this Squadron can field
    [SerializeField]
    private int _speed;
    public int speed { get { return _speed; } private set { _speed = value; } }
    public void CalculateSpeed()
    {
        speed = (int)GetTopKnights(Battle.battleSize).Select(o => o.speed).Average();
    }

    public void CalculateStats()
    {
        CalculateAttack();
        CalculateDefence();
        CalculateSpeed();
        CalculateBattlePower();
    }

    // Lord of the Squadron
    [SerializeField]
    private int _squadronLordID;
    public int squadronLordID
    {
        get { return _squadronLordID; }
        set
        {
            _squadronLordID = value;
            Person.GetPerson(_squadronLordID).squadronID = squadronID;
        }
    }
    public Person squadronLord
    {
        get
        {
            return Person.GetPerson(squadronLordID);
        }
    }

    // Amount of cash this Squadron has
    [SerializeField]
    private int _cash;
    public int cash { get { return _cash; } set { _cash = value; } }

    // A sigil that will represent the Squadron
    public Sigil squadronSigil { get; set; }

    // Three colours representing the Squadron
    [SerializeField]
    private MyColour[] _colours;
    public MyColour[] colours { get { return _colours; } private set { _colours = value; } }
    private void SetColours()
    {
        List<MyColour> remainingColours = MyColour.allColours;
        int rand = Random.Range(0, remainingColours.Count);
        MyColour colour1 = remainingColours[rand];

        remainingColours = remainingColours.FindAll(o => o != colour1);
        rand = Random.Range(0, remainingColours.Count);
        MyColour colour2 = remainingColours[rand];

        remainingColours = remainingColours.FindAll(o => o != colour1 && o != colour2);
        rand = Random.Range(0, remainingColours.Count);
        MyColour colour3 = remainingColours[rand];

        colours = new MyColour[] { colour1, colour2, colour3 };
    }

    // Current and previous Competitions the Squadron is involved in
    [SerializeField]
    private List<int> currentCompSquadIDs = new List<int>();
    [SerializeField]
    private List<int> previousCompSquadIDs = new List<int>();
    public void AddCompetition(CompSquad compSquad)
    {
        currentCompSquadIDs.Add(compSquad.compSquadID);
    }
    public void RemoveCompetition(CompSquad compSquad)
    {
        if (currentCompSquads.Contains(compSquad))
        {
            currentCompSquadIDs.Remove(compSquad.compSquadID);
            previousCompSquadIDs.Add(compSquad.compSquadID);
        }
    }
    public CompSquad GetCompSquad(Competition comp)
    {
        return currentCompSquads.First(o => o.comp == comp);
    }
     
    public CompSquad[] currentCompSquads
    {
        get
        {
            return currentCompSquadIDs.Select(o => CompSquad.GetCompSquad(o)).ToArray();
        }
    }
    public CompSquad[] previousCompSquads
    {
        get
        {
            return previousCompSquadIDs.Select(o => CompSquad.GetCompSquad(o)).ToArray();
        }
    }

    public Competition[] currentComps
    {
        get
        {
            return currentCompSquadIDs.Select(o => CompSquad.GetCompSquad(o).comp).ToArray();
        }
    }
    public Competition[] previousComps
    {
        get
        {
            return previousCompSquadIDs.Select(o => CompSquad.GetCompSquad(o).comp).ToArray();
        }
    }

    // Battles the Squadron is involved in
    [SerializeField]
    private List<int> futureBattleIDs = new List<int>();
    [SerializeField]
    private List<int> previousBattleIDs = new List<int>();
    public void AddBattle(Battle battle)
    {
        futureBattleIDs.Add(battle.battleID);
        futureBattleIDs.OrderBy(o => Battle.GetBattle(o).date);
    }
    public void RemoveBattle(Battle battle)
    {
        if (futureBattleIDs.Contains(battle.battleID))
        {
            futureBattleIDs.Remove(battle.battleID);
            previousBattleIDs.Add(battle.battleID);
        }
        else
        {
            Debug.Log("Battle not in Squadron's fixtures.");
        }
    }

    public Battle[] futureBattles
    {
        get
        {
            return futureBattleIDs.Select(o => Battle.GetBattle(o)).ToArray();
        }
    }
    public Battle[] previousBattles
    {
        get
        {
            return previousBattleIDs.Select(o => Battle.GetBattle(o)).ToArray();
        }
    }

    public Battle GetFutureBattlesOnDate(int date)
    {
        for (int i = 0; i < futureBattles.Length; i++)
        {
            if (futureBattles[i].date == date)
            {
                return futureBattles[i];
            }
        }
        return null;
    }
    public Battle GetPreviousBattlesOnDate(int date)
    {
        for (int i = 0; i < previousBattles.Length; i++)
        {
            if (previousBattles[i].date == date)
            {
                return previousBattles[i];
            }
        }
        return null;
    }

    // Counts of Battle wins and losses
    [SerializeField]
    private int _totalBattlesWon;
    public int totalBattlesWon { get { return _totalBattlesWon; } }
    private void IncreaseTotalBattlesWon()
    {
        _totalBattlesWon += 1;
    }

    [SerializeField]
    private int _totalBattlesLost;
    public int totalBattlesLost { get { return _totalBattlesLost; } }
    private void IncreaseTotalBattlesLost()
    {
        _totalBattlesLost += 1;
    }

    [SerializeField]
    private int _seasonBattlesWon;
    public int seasonBattlesWon { get { return _seasonBattlesWon; } }
    public void IncreaseSeasonBattlesWon()
    {
        _seasonBattlesWon += 1;
        IncreaseTotalBattlesWon();
    }
    public void ResetSeasonBattlesWon()
    {
        _seasonBattlesWon = 0;
    }

    [SerializeField]
    private int _seasonBattlesLost;
    public int seasonBattlesLost { get { return _seasonBattlesLost; } }
    public void IncreaseSeasonBattlesLost()
    {
        _seasonBattlesLost += 1;
        IncreaseTotalBattlesLost();
    }
    public void ResetSeasonBattlesLost()
    {
        _seasonBattlesLost = 0;
    }

    // All Knights in this Squadron
    [SerializeField]
    private List<int> allKnightIDs = new List<int>();
    private void AddToAllKnights(int knightID)
    {
        if (allKnightIDs.Contains(knightID))
        {
            Debug.Log("Knight is already a member of " + name + "'s Squadron.");
        }
        else
        {
            if (Person.GetPerson(knightID).squadronID != -1)
            {
                Person.GetPerson(knightID).squadron.RemoveFromAllKnights(knightID);
            }
            allKnightIDs.Add(knightID);
            Person.GetPerson(knightID).squadronID = squadronID;
            CalculateStats();
        }
    }
    private void RemoveFromAllKnights(int knightID)
    {
        if (allKnightIDs.Contains(knightID))
        {
            Person.GetPerson(knightID).squadronID = -1;
            allKnightIDs.Remove(knightID);
            CalculateStats();
        }
        else
        {
            Debug.Log("Knight is not a member of " + name + "'s full squadron.");
        }
    }
    public Person[] allKnights
    {
        get
        {
            return allKnightIDs.Select(o => Person.GetPerson(o)).ToArray();
        }
    }
    public Person[] GetTopKnights(int numberOfKnights)
    {
        return allKnights.OrderBy(o => o.battlePower).Take(numberOfKnights).ToArray();
    }

    // Purchase another Knight from another Squadron or the Open Market
    public void BuyKnight(Person knight)
    {
        if (knight.squadronID != -1)
        {
            if (cash >= knight.cost)
            {
                cash -= knight.cost;
                knight.squadron.cash += knight.cost;
                AddToAllKnights(knight.personID);
            }
            else
            {
                Debug.Log("Not enough cash (" + cash + " / " + knight.cost + ")");
            }
        }
        else
        {
            AddToAllKnights(knight.personID);
        }

    }

    // Return a new random Squadron
    public static Squadron RandomSquadron(int region)
    {
        int rand = Random.Range(0, ExternalData.unusedPlaceNamesCount() - 1);
        string name = ExternalData.UsePlaceName(rand);

        int squadronLordID = Person.RandomLord().personID;

        List<int> allKnightIDs = new List<int>();

        for (int i = 0; i < 20; i++)
        {
            Person newKnight = Person.RandomKnight();
            allKnightIDs.Add(newKnight.personID);
        }

        return new Squadron(name, region, squadronLordID, allKnightIDs);
    }

    // Return a new random Squadron with strength
    public static Squadron RandomSquadron(int strength, int region)
    {
        int rand = Random.Range(0, ExternalData.unusedPlaceNamesCount() - 1);
        string name = ExternalData.UsePlaceName(rand);

        int squadronLordID = Person.RandomLord().personID;

        List<int> allKnightIDs = new List<int>();

        for (int i = 0; i < 20; i++)
        {
            Person newKnight = Person.RandomKnight(strength);
            allKnightIDs.Add(newKnight.personID);
        }

        return new Squadron(name, region, squadronLordID, allKnightIDs);
    }

    // Constructor
    public Squadron(string name, int region, int squadronLordID, List<int> allKnightIDs)
    {
        AddSquadron(this);

        this.name = name;
        this.region = region;
        this.squadronLordID = squadronLordID;
        for (int i = 0; i < allKnightIDs.Count; i++)
        {
            AddToAllKnights(allKnightIDs[i]);
        }

        CalculateBattlePower();
        CalculateAttack();
        CalculateDefence();
        CalculateSpeed();
        SetColours();
    }
}
