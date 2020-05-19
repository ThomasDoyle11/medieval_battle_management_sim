using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Linq;

[System.Serializable]
public class Competition
{
    // Enum for type of Competition
    // Used instead of derived classes
    public enum CompType { Cup, League };

    // List of all Competitions and sublists according to different conditions
    public static List<Competition> allCompetitions { get; set; } = new List<Competition>();
    private static void AddCompetition(Competition newCompetition)
    {
        newCompetition.compID = allCompetitions.Count;
        allCompetitions.Add(newCompetition);
    }
    public static Competition[] activeComps
    {
        get
        {
            return allCompetitions.FindAll(o => o.year == Date.getYear()).ToArray();
        }
    }
    public static Competition[] compsFromYear(int year)
    {
        return allCompetitions.FindAll(o => o.year == Date.getYear(year)).ToArray();
    }

    public static Competition[] allCups
    {
        get
        {
            return allCompetitions.FindAll(o => o.compType == CompType.Cup).ToArray();
        }
    }
    public static Competition[] activeCups
    {
        get
        {
            return System.Array.FindAll(activeComps, o => o.compType == CompType.Cup).ToArray();
        }
    }
    public static Competition[] cupsFromYear(int year)
    {
        return System.Array.FindAll(compsFromYear(year), o => o.compType == CompType.Cup).ToArray();
    }

    public static Competition[] allLeagues
    {
        get
        {
            return allCompetitions.FindAll(o => o.compType == CompType.League).ToArray();
        }
    }
    public static Competition[] activeLeagues
    {
        get
        {
            return System.Array.FindAll(activeComps, o => o.compType == CompType.League).ToArray();
        }
    }
    public static Competition[] leaguesFromYear(int year)
    {
        return System.Array.FindAll(compsFromYear(year), o => o.compType == CompType.League).ToArray();
    }

    public static Competition[] GetCompsFromYear(int year)
    {
        return allCompetitions.FindAll(o => o.year == year).ToArray();
    }

    public static Squadron[] CompetitionWinners(int year)
    {
        List<Squadron> returnList = new List<Squadron>();
        Competition[] yearComps = GetCompsFromYear(year);
        Squadron newSquadron;
        foreach (Competition comp in yearComps)
        {
            newSquadron = System.Array.Find(comp.compSquads, o => o.rank == 0).squadron;
            if (!returnList.Contains(newSquadron))
            {
                returnList.Add(newSquadron);
            }
        }
        return returnList.ToArray();
    }

    // Data to be provided externally - placeholders for now
    public static int numSquadronsInLeague = 20;
    public static int cupBasePrize = 100;
    public static int leagueBasePrize = 1000;
    public static int numSquadronsToPromote = 3;

    // Unique identifer for the Competition
    [SerializeField]
    private int _compID;
    public int compID { get { return _compID; } private set { _compID = value; } }
    public static Competition GetCompetition(int competitionID)
    {
        return allCompetitions[competitionID];
    }
    public static Competition GetCompetitionByFacts(CompType compType, int tier, int region, int year)
    {
        List<Competition> matchingComps;
        matchingComps = allCompetitions.FindAll(o => o.compType == compType).ToList();
        matchingComps = matchingComps.FindAll(o => o.tier == tier).ToList();
        matchingComps = matchingComps.FindAll(o => o.region == region).ToList();
        matchingComps = matchingComps.FindAll(o => o.year == year).ToList();
        if (matchingComps.Count == 1)
        {
            return matchingComps[0];
        }
        else if (matchingComps.Count == 0)
        {
            return null;
        }
        else
        {
            Debug.Log("Multiple matches: ");
            for (int i = 0; i < matchingComps.Count; i++)
            {
                Debug.Log(matchingComps[i].competitionFullName);
            }
            return null;
        }
    }

    // Type of Competition
    [SerializeField]
    private CompType _compType;
    public CompType compType { get { return _compType; } protected set { _compType = value; } }

    // The tier of the competition, 0 being the most competitive (-1 if not tiered)
    [SerializeField]
    private int _tier;
    public int tier { get { return _tier; } protected set { _tier = value; } }
    // If tiered and not max tier, the Competiton above this one
    public Competition competitionAbove
    {
        get
        {
            if (tier != -1 && tier != 0)
            {
                Competition matchingComp = GetCompetitionByFacts(compType, tier - 1, region, year);
                if (matchingComp != null)
                {
                    return matchingComp;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
    // If tiered and not min tier, the Competiton below this one
    public Competition competitionBelow
    {
        get
        {
            if (tier != -1)
            {
                Competition matchingComp = GetCompetitionByFacts(compType, tier + 1, region, year);
                if (matchingComp != null)
                {
                    return matchingComp;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }

    // The integer ID of the region of the competition (-1 if not region-based)
    [SerializeField]
    private int _region;
    public int region { get { return _region; } protected set { _region = value; } }

    // The year this Competition started
    [SerializeField]
    private int _year;
    public int year { get { return _year; } protected set { _year = value; } }

    // The season this Competition is in
    // Season object to be introduced in later version to collect Competitions together
    [SerializeField]
    private int _season;
    public int season { get { return _season; } protected set { _season = value; } }

    // Name of Competition
    [SerializeField]
    private string _competitionName;
    public string competitionName { get { return _competitionName; } set { _competitionName = value; } }
    // Full name (including year) of Competition
    public string competitionFullName {
        get
        {
            return competitionName + ((tier != -1) ? " " + (tier + 1) : "") + " (" + year + ")";
        }
    }

    // Weekday in which Comp Battles occur
    [SerializeField]
    private int _competitionWeekday;
    public int competitionWeekday { get { return _competitionWeekday; } protected set { _competitionWeekday = value; } }

    // First instance of competitionWeekday that will have a Battle
    [SerializeField]
    private int _intialWeek;
    public int initialWeek { get { return _intialWeek; } protected set { _intialWeek = value; } }

    // Weeks between each Battle
    [SerializeField]
    private int _weeksBetweenBattles;
    public int weeksBetweenBattles { get { return _weeksBetweenBattles; } protected set { _weeksBetweenBattles = value; } }

    // Number of Squadrons in the Comp
    public int numberOfSquadrons { get { return compSquadIDs.Count; } }

    // Competitiveness of the Competition (0 - 100)
    [SerializeField]
    private int _competitiveness;
    public int competitiveness { get { return _competitiveness; } }
    public void SetCompetitiveness()
    {
        float mean = (float)squadrons.Select(o => o.battlePower).Average();
        float squareMean = squadrons.Select(o => Mathf.Pow(o.battlePower, 2)).Average();
        float sd = Mathf.Pow(squareMean - Mathf.Pow(mean, 2), 0.5f);
        _competitiveness = (int)(100 - 2 * sd);
    }

    // The average Battle Power of all Squadrons
    [SerializeField]
    private int _battlePower;
    public int battlePower { get { return _battlePower; } private set { _battlePower = value; } }
    public void CalculateBattlePower()
    {
        battlePower = (int)squadrons.Select(o => o.battlePower).Average();
    }

    // All Squadrons in the competition - represented by their unique CompSquad for this Comp
    [SerializeField]
    private List<int> _compSquadIDs = new List<int>();
    public List<int> compSquadIDs
    {
        get { return _compSquadIDs; }
    }
    protected void AddSquadron(Squadron squadron)
    {
        CompSquad newCompSquad = new CompSquad(this, squadron);
        _compSquadIDs.Add(newCompSquad.compSquadID);
        newCompSquad.squadron.AddCompetition(newCompSquad);
    }
    public CompSquad GetCompSquad(Squadron squadron)
    {
        int compSquadID = compSquadIDs.First(o => CompSquad.GetCompSquad(o).squadronID == squadron.squadronID);
        return CompSquad.GetCompSquad(compSquadID);
    }

    public CompSquad[] compSquads
    {
        get
        {
            return compSquadIDs.Select(o => CompSquad.GetCompSquad(o)).ToArray();
        }
    }
    public Squadron[] squadrons
    {
        get
        {
            return compSquads.Select(o => o.squadron).ToArray();
        }
    }
    public Squadron GetSquadronAtRank(int rank)
    {
        return System.Array.Find(squadrons, o => o.GetCompSquad(this).rank == rank);
    }

    // Squadrons which will be promoted and demoted at the end of the season
    // Only used for Leagues
    public Squadron[] squadronsToPromote
    {
        get
        {
            Squadron[] returnSquadrons = new Squadron[numSquadronsToPromote];
            for (int i = 0; i < numSquadronsToPromote; i++)
            {
                returnSquadrons[i] = GetSquadronAtRank(i);
            }
            return returnSquadrons;
        }
    }
    public Squadron[] squadronsToDemote
    {
        get
        {
            Squadron[] returnSquadrons = new Squadron[numSquadronsToPromote];
            for (int i = 0; i < numSquadronsToPromote; i++)
            {
                returnSquadrons[i] = GetSquadronAtRank(numSquadronsInLeague - i - 1);
            }
            return returnSquadrons;
        }
    }

    public Squadron[] nextSeasonsSquadrons
    {
        get
        {
            Squadron[] returnSquadrons = new Squadron[numSquadronsInLeague];
            for (int i = 0; i < numberOfSquadrons; i++)
            {
                if (i < numSquadronsToPromote && competitionAbove != null)
                {
                    returnSquadrons[i] = competitionAbove.squadronsToDemote[i];
                }
                else if (i > numSquadronsInLeague - numSquadronsToPromote - 1 && competitionBelow != null)
                {
                    returnSquadrons[i] = competitionBelow.squadronsToPromote[numSquadronsInLeague - i - 1];
                }
                else
                {
                    returnSquadrons[i] = GetSquadronAtRank(i);
                }
            }
            return returnSquadrons;
        }
    }

    // Points awarded - only used in Leagues
    public int pointsForWin { get; } = 3;
    public int pointsForBonus { get; } = 1;

    // Conditions for a bonus for home or away team - only used in Leagues
    public bool ConditionsForHomeBonus(Battle battle)
    {
        if (battle.homeDamageDealt >= 100 / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ConditionsForAwayBonus(Battle battle)
    {
        if (battle.awayDamageDealt >= 100 / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // List of all Rounds in the Comp
    [SerializeField]
    private List<int> _roundIDs;
    public List<int> roundIDs { get { return _roundIDs; } protected set { _roundIDs = value; } }
    public Round[] rounds
    {
        get
        {
            return roundIDs.Select(o => Round.GetRound(o)).ToArray();
        }
    }
    public Round[] futureRounds
    {
        get
        {
            return System.Array.FindAll(rounds, o => o.date >= Date.currentDate);
        }
    }
    public Round[] previousRounds
    {
        get
        {
            return System.Array.FindAll(rounds, o => o.date < Date.currentDate);
        }
    }
    public Round GetRoundByDate(int date)
    {
        return rounds.FirstOrDefault(o => o.date == date);
    }
    public Round finalRound
    {
        get
        {
            return rounds[rounds.Length - 1];
        }
    }
    public void GenerateRounds()
    {
        // Firstly, generate empty Round objects
        for (int i = 0; i < roundDates.Count; i++)
        {
            Round newRound = new Round(this, i, roundDates[i]);
            roundIDs.Add(newRound.roundID);
        }

        if (compType == CompType.Cup)
        {
            // For a cup, only populate the first Round, as future Rounds will be decided by their preceeding Round
            int p = Mathf.CeilToInt(Mathf.Log(compSquads.Length, 2)) - 1;
            int sizeFirstRound = compSquads.Length - (int)Mathf.Pow(2, p);

            for (int i = 0; i < compSquads.Length; i++)
            {
                if (i < 2 * sizeFirstRound)
                {
                    if (i % 2 == 0)
                    {
                        Squadron homeSquad = compSquads[i].squadron;
                        Squadron awaySquad = compSquads[i + 1].squadron;
                        rounds[0].AddCompSquad(homeSquad);
                        rounds[0].AddCompSquad(awaySquad);

                        Battle newBattle = new Battle(homeSquad, awaySquad, rounds[0]);
                        rounds[0].AddBattle(newBattle);
                    }
                }
                else
                {
                    compSquads[i].AddPoints(1);
                    rounds[1].AddCompSquad(compSquads[i].squadron);
                }
            }
        }
        else if (compType == CompType.League)
        {
            // All Squadrons in all Rounds in a League
            for (int i = 0; i < rounds.Length; i++)
            {
                rounds[i].AddCompSquads(squadrons);
            }

            // Create a randomized order of Squadrons
            System.Random random = new System.Random();
            CompSquad[] randomizedCompSquads = compSquads.OrderBy((item) => random.Next()).ToArray();
            List<Squadron> randomizedSquadrons = new List<Squadron>();
            for (int i = 0; i < randomizedCompSquads.Length; i++)
            {
                randomizedSquadrons.Add(randomizedCompSquads[i].squadron);
            }

            List<List<Squadron[]>> unbalancedFixtureList = new List<List<Squadron[]>>();

            int numberOfDates = numberOfSquadrons - 1;
            int matchesPerDate = numberOfSquadrons / 2;

            for (int i = 0; i < numberOfDates; i++)
            {
                unbalancedFixtureList.Add(new List<Squadron[]>());

                for (int j = 0; j < matchesPerDate; j++)
                {
                    Squadron homeSquadron = randomizedSquadrons[(i + j) % (numberOfDates)];
                    Squadron awaySquadron;

                    // Circle algorithm rotating around initial squadron
                    if (j != 0)
                    {
                        awaySquadron = randomizedSquadrons[(numberOfDates - j + i) % (numberOfDates)];
                    }
                    else
                    {
                        awaySquadron = randomizedSquadrons[numberOfDates];
                    }

                    int fixtureDate = roundDates[i];
                    Squadron[] newBattle = { homeSquadron, awaySquadron };
                    unbalancedFixtureList[i].Add(newBattle);
                }
            }

            // Balance out dates of home and away battles
            List<List<Squadron[]>> balancedFixtures = new List<List<Squadron[]>>();

            int even = 0;
            int odd = numberOfSquadrons / 2;
            for (int i = 0; i < numberOfDates; i++)
            {
                if (i % 2 == 0)
                {
                    balancedFixtures.Add(unbalancedFixtureList[even]);
                    even += 1;
                }
                else
                {
                    balancedFixtures.Add(unbalancedFixtureList[odd]);
                    odd += 1;
                }
            }

            // Make every other Battle away for the initial squadron.
            for (int i = 0; i < numberOfDates; i++)
            {
                if (i % 2 == 1)
                {
                    Squadron[] newBattle = balancedFixtures[i][0];
                    Squadron homeSquadron = newBattle[0];
                    Squadron awaySquadron = newBattle[1];
                    balancedFixtures[i][0][0] = awaySquadron;
                    balancedFixtures[i][0][1] = homeSquadron;
                }
            }

            // Re randomize order for second half
            // Use same order and switch home and away for ease, however if we find a better algorithm we'll use it
            unbalancedFixtureList = new List<List<Squadron[]>>();
            for (int i = 0; i < numberOfDates; i++)
            {
                unbalancedFixtureList.Add(new List<Squadron[]>());

                for (int j = 0; j < matchesPerDate; j++)
                {
                    Squadron awaySquadron = randomizedSquadrons[(i + j) % (numberOfDates)];
                    Squadron homeSquadron;

                    // Rotate around last squadron
                    if (j != 0)
                    {
                        homeSquadron = randomizedSquadrons[(numberOfDates - j + i) % (numberOfDates)];
                    }
                    else
                    {
                        homeSquadron = randomizedSquadrons[numberOfDates];
                    }

                    int fixtureDate = roundDates[i];
                    Squadron[] newBattle = { homeSquadron, awaySquadron };
                    unbalancedFixtureList[i].Add(newBattle);
                }
            }

            // Balance out dates of home and away battles
            even = 0;
            odd = (numberOfSquadrons / 2);

            for (int i = 0; i < numberOfDates; i++)
            {
                if (i % 2 == 0)
                {
                    balancedFixtures.Add(unbalancedFixtureList[even++]);
                }
                else
                {
                    balancedFixtures.Add(unbalancedFixtureList[odd++]);
                }
            }

            for (int i = 0; i < numberOfDates; i++)
            {
                if (i % 2 == 1)
                {
                    Squadron[] newBattle = balancedFixtures[numberOfDates + i][0];
                    Squadron homeSquadron = newBattle[0];
                    Squadron awaySquadron = newBattle[1];
                    balancedFixtures[numberOfDates + i][0][0] = awaySquadron;
                    balancedFixtures[numberOfDates + i][0][1] = homeSquadron;
                }
            }

            for (int i = 0; i < balancedFixtures.Count; i++)
            {
                for (int j = 0; j < balancedFixtures[i].Count; j++)
                {
                    Battle newBattle = new Battle(balancedFixtures[i][j][0], balancedFixtures[i][j][1], rounds[i]);
                    rounds[i].AddBattle(newBattle);
                }
            }
        }
    }
    public void PerformRoundsToday()
    {
        Round round = rounds.FirstOrDefault(o => o.date == Date.currentDate);
        if (round != null)
        {
            round.PerformBattles();
        }
    }


    // Dates for all fixtures
    [SerializeField]
    private List<int> _roundDates;
    public List<int> roundDates { get { return _roundDates; } protected set { _roundDates = value; } }
    public void SetRoundDates()
    {
        roundDates = new List<int>();
        if (numberOfSquadrons > 0)
        {
            int numOfRoundDates;
            if (compType == CompType.League)
            {
                numOfRoundDates = 2 * (compSquads.Length - 1);
            }
            else
            {
                numOfRoundDates = Mathf.CeilToInt(Mathf.Log(numberOfSquadrons, 2));
            }
            int date;
            int firstDay = Date.getFirstWeekdayDateInYear(competitionWeekday, Date.getYear(Date.currentDate));
            for (int i = 0; i < numOfRoundDates; i++)
            {
                date = 7 * (weeksBetweenBattles * i + initialWeek) + firstDay;
                roundDates.Add(date);
            }
        }
        else
        {
            Debug.Log("No squadrons exist for dates to be set.");
        }
    }

    public int numRounds
    {
        get
        {
            return roundDates.Count;
        }
    }

    // Check if a given date has Battles (default current date)
    public bool dateHasBattles(int date)
    {
        for (int i = 0; i < roundDates.Count; i++)
        {
            if (roundDates[i] == date)
            {
                return true;
            }
        }
        return false;
    }
    public bool dateHasBattles()
    {
        return dateHasBattles(Date.currentDate);
    }

    // End the Competition
    public void EndCompetition()
    {
        if (compType == CompType.Cup)
        {
            Debug.Log(GetSquadronAtRank(0).name + " wins " + competitionFullName + "!");
        }
        else if (compType == CompType.League)
        {
            Debug.Log(GetSquadronAtRank(0).name + " wins " + competitionFullName + "!");
        }
    }

    // Set the rank of each Squadron in the CompSquad
    public void SetRanks()
    {
        CompSquad[] rankedSquads = compSquads.OrderByDescending(o => o.points).ThenBy(o => o.squadron.name).ToArray();
        for (int i = 0; i < compSquads.Length; i++)
        {
            compSquads[i].rank = System.Array.IndexOf(rankedSquads, compSquads[i]);
        }
    }

    // Generate League of 20 random Squadrons
    public static Competition RandomLeague(string leagueName, int tier, int region)
    {
        List<Squadron> squadrons = new List<Squadron>();
        int numberOfSquadrons = 20;
        int strength = tier == -1 ? 3 : 3 - tier;
        for (int i = 0; i < numberOfSquadrons; i++)
        {
            Squadron newSquadron = Squadron.RandomSquadron(strength, region);
            squadrons.Add(newSquadron);
        }

        return League(leagueName, squadrons.ToArray(), tier, region);
    }

    // Generate League based on List of Squadrons
    public static Competition League(string leagueName, Squadron[] squadrons, int tier, int region)
    {
        return new Competition(leagueName, CompType.League, squadrons, tier, region, 6, 1, 1);
    }

    // Generate Cup based on a list of Comps
    public static Competition CupFromComps(string cupName, List<Competition> comps, int tier, int region, int weekday, int initialWeek)
    {
        List<CompSquad> allCompSquads = new List<CompSquad>();
        for (int i = 0; i < comps.Count; i++)
        {
            allCompSquads.AddRange(comps[i].compSquads);
        }

        List<Squadron> squads = new List<Squadron>();
        for (int i = 0; i < allCompSquads.Count; i++)
        {
            squads.Add(allCompSquads[i].squadron);
        }

        return new Competition(cupName, CompType.Cup, squads.ToArray(), tier, region, weekday, initialWeek, 3);
    }

    // Generate Cup based on a list of Squadrons
    public static Competition Cup(string cupName, Squadron[] squadrons, int tier, int region, int weekday, int initialWeek)
    {
        return new Competition(cupName, CompType.Cup, squadrons, tier, region, weekday, initialWeek, 3);
    }

    // Constructor for creating a Competition
    public Competition(string competitionName, CompType compType, Squadron[] squadrons, int tier, int region, int competitionWeekday, int initialWeek, int weeksBetweenBattles)
    {
        AddCompetition(this);

        year = Date.getYear();
        this.tier = tier;
        this.region = region;
        this.initialWeek = initialWeek;
        this.compType = compType;
        this.competitionName = competitionName;
        this.competitionWeekday = competitionWeekday;
        this.weeksBetweenBattles = weeksBetweenBattles;

        System.Random random = new System.Random();
        Squadron[] randomizedSquads = squadrons.OrderBy((item) => random.Next()).ToArray();
        for (int j = 0; j < randomizedSquads.Length; j++)
        {
            AddSquadron(randomizedSquads[j]);
        }

        roundIDs = new List<int>();

        SetRoundDates();
        GenerateRounds();
        SetCompetitiveness();
        CalculateBattlePower();

        SetRanks();
    }
}
