using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Person
{
    // Enum for type of Person
    // Used instead of derived classes
    public enum PersonType { Youth, Knight, Lord, Trainer, Healer };
    public string prenominal
    {
        get
        {
            if (personType == PersonType.Knight)
            {
                return "Sir ";
            }
            else if (personType == PersonType.Lord)
            {
                return "Lord ";
            }
            else
            {
                return "";
            }
        }
    }

    // List of all People, and sublists based on certain conditions
    public static List<Person> allPeople { get; set; } = new List<Person>();
    private static void AddPerson(Person person)
    {
        person.personID = allPeople.Count;
        allPeople.Add(person);
    }

    public static Person[] allKnights
    {
        get
        {
            return allPeople.FindAll(o => o.personType == PersonType.Knight).ToArray();
        }
    }
    public static Person[] livingKnights
    {
        get
        {
            return System.Array.FindAll(allKnights, o => o.deathDate == -1);
        }
    }

    public static Person[] allLords
    {
        get
        {
            return allPeople.FindAll(o => o.personType == PersonType.Lord).ToArray();
        }
    }
    public static Person[] livingLords
    {
        get
        {
            return System.Array.FindAll(allLords, o => o.deathDate == -1);
        }
    }

    public static Person[] allTrainers
    {
        get
        {
            return allPeople.FindAll(o => o.personType == PersonType.Trainer).ToArray();
        }
    }
    public static Person[] livingTrainers
    {
        get
        {
            return System.Array.FindAll(allTrainers, o => o.deathDate == -1);
        }
    }

    public static Person[] allHealers
    {
        get
        {
            return allPeople.FindAll(o => o.personType == PersonType.Healer).ToArray();
        }
    }
    public static Person[] livingHealers
    {
        get
        {
            return System.Array.FindAll(allHealers, o => o.deathDate == -1);
        }
    }
    
    // Unique identifer for the Person
    [SerializeField]
    private int _personID;
    public int personID { get { return _personID; } private set { _personID = value; } }
    public static Person GetPerson(int personID)
    {
        if (personID != -1)
        {
            return allPeople[personID];
        }
        else
        {
            return null;
        }
    }

    // Type of Person
    [SerializeField]
    private PersonType _personType;
    public PersonType personType { get { return _personType; } set { _personType = value; } }

    // Squadron the Person belongs to, -1 if no Squadron
    [SerializeField]
    private int _squadronID;
    public int squadronID { get { return _squadronID; } set { _squadronID = value; } }
    public Squadron squadron
    {
        get
        {
            return Squadron.GetSquadron(squadronID);
        }
    }

    // Mother of the Person
    [SerializeField]
    private int _motherID;
    public int motherID {
        get
        {
            return _motherID;
        }
        set
        {
            GetPerson(value).AddChild(this);
            _motherID = value;
        }
    }
    public Person mother
    {
        get
        {
            return GetPerson(motherID);
        }
        set
        {
            motherID = value.personID;
        }
    }

    // Father of the Person
    [SerializeField]
    private int _fatherID;
    public int fatherID
    {
        get
        {
            return _fatherID;
        }
        set
        {
            GetPerson(value).AddChild(this);
            _fatherID = value;
        }
    }
    public Person father
    {
        get
        {
            return GetPerson(fatherID);
        }
        set
        {
            fatherID = value.personID;
        }
    }

    // Partner of the Person
    [SerializeField]
    private int _partnerID;
    public int partnerID
    {
        get
        {
            return _partnerID;
        }
        set
        {
            GetPerson(value).partnerID = personID;
            _partnerID = value;
        }
    }
    public Person partner
    {
        get
        {
            return GetPerson(partnerID);
        }
        set
        {
            partnerID = value.partnerID;
        }
    }

    // Children of the Person
    [SerializeField]
    private List<int> _childrenIDs;
    public List<int> childrenIDs { get { return _childrenIDs; } set { _childrenIDs = value; } }
    public Person[] children
    {
        get
        {
            return childrenIDs.Select(o => GetPerson(o)).ToArray();
        }
    }
    private void AddChild(Person person)
    {
        _childrenIDs.Add(person.personID);
    }

    // Relationship to Person
    public Relationship RelationshipToPerson(Person person)
    {
        return new Relationship(this, person);
    }

    // Gender of Person
    [SerializeField]
    private bool _isMale;
    public bool isMale { get { return _isMale; } set { _isMale = value; } }
    public string gender
    {
        get
        {
            return isMale ? "male" : "female";
        }
    }

    // First name of the Person
    [SerializeField]
    private string _firstName;
    public string firstName { get { return _firstName; } set { _firstName = value; } }

    // ID of the House of the Person
    [SerializeField]
    private int _houseID;
    public int houseID { get { return _houseID; } private set { _houseID = value; } }
    public House house
    {
        get
        {
            return House.GetHouse(houseID);
        }
    }

    // Date of birth of the Person
    [SerializeField]
    private int _birthDate;
    public int birthDate { get { return _birthDate; } private set { _birthDate = value; } }
    public int ageInDays
    {
        get
        {
            if (!isDead)
            {
                return Date.currentDate - birthDate;
            }
            else
            {
                return deathDate - birthDate;
            }
        }
    }
    public int ageInYears
    {
        get
        {
            return ageInDays / 365;
        }
    }

    // Date Person died, -1 if still alive
    [SerializeField]
    private int _deathDate;
    public int deathDate { get { return _deathDate; } private set { _deathDate = value; } }
    public bool isDead
    {
        get
        {
            return deathDate != -1;
        }
    }
    public void Die()
    {
        deathDate = Date.currentDate;
        Debug.Log(fullName + " has died.");
    }

    // Return initial of first name followed by House name (e.g. A. Knightman).
    public string initialisedName
    {
        get
        {
            return firstName.Substring(0, 1) + ". " + house.houseName;
        }
    }
    // The first name and House name (e.g. Andrew Knightman).
    public string fullName
    {
        get
        {
            return firstName + " " + house.houseName;
        }
    }
    // The full title of the Person
    public string fullTitle
    {
        get
        {
            return prenominal + firstName + " of House " + house.houseName;
        }
    }

    // Knight stats

    // Attack stat - used to deal damange to enemies
    [SerializeField]
    private int _attack;
    public int attack
    {
        get { return _attack; }
        set
        {
            _attack = Mathf.Clamp(value, 0, 100);
        }
    }

    // Defence stat - used to defend from enemies and reduce losses
    [SerializeField]
    private int _defence;
    public int defence
    {
        get { return _defence; }
        set
        {
            _defence = Mathf.Clamp(value, 0, 100);
        }
    }

    // Speed stat - used to peform better manouvres
    [SerializeField]
    private int _speed;
    public int speed
    {
        get { return _speed; }
        set
        {
            _speed = Mathf.Clamp(value, 0, 100);
        }
    }

    // Troops stat - high troops gives better performance - it is reduced by battles and increases over time.
    [SerializeField]
    private int _troops;
    public int troops
    {
        get { return _troops; }
        set
        {
            _troops = Mathf.Clamp(value, 0, 100);
        }
    }
    // Troops regained per day - currently constant for all knights but will eventually rely on a variety of factors.
    public void RegainDailyTroops()
    {
        troops += 3;
    }

    // A stat to summarise the Knight stats of a Person
    public int battlePower
    {
        get
        {
            return (2 * attack + 2 * defence + speed) / 5;
        }
    }
    // A stat to summarise the Knight stats of a Person, accounting for troop losses
    public int battleReadiness
    {
        get
        {
            return battlePower * troops / 100;
        }
    }

    // Lord stats

    // Leadership stat - shows how good a Lord they are.
    [SerializeField]
    private int _leadership;
    public int leadership
    {
        get { return _leadership; }
        set
        {
            _leadership = Mathf.Clamp(value, 0, 100);
        }
    }

    // Healer stats

    // Healing stat - how quickly the Healer can heal the troops
    [SerializeField]
    private int _healing;
    public int healing
    {
        get { return _healing; }
        set
        {
            _healing = Mathf.Clamp(value, 0, 100);
        }
    }

    // Trainer stats

    // Training stat - how well the Trainer can train the Knights
    [SerializeField]
    private int _training;
    public int training
    {
        get { return _training; }
        set
        {
            _training = Mathf.Clamp(value, 0, 100);
        }
    }

    // Cost of the Knight, based on stats with some random variation
    [SerializeField]
    private int _cost;
    public int cost { get { return _cost; } private set { _cost = value; } }
    private void CalculateCost()
    {
        int maxPower = 100;
        int maxRand = 2000;
        int maxCost = 10000;
        int rand = Random.Range(-maxRand, maxRand);
        float eccentricity = 2;
        cost = (int)(maxRand + (maxCost - maxRand) * Mathf.Pow((float)battlePower / maxPower, eccentricity) + rand);
    }

    // Generate Knight
    public static Person Knight(string firstName, House house, int birthDate, bool isMale, int attack, int defence, int speed, int leadership, int healing, int training)
    {
        return new Person(PersonType.Knight, firstName, house, birthDate, isMale, attack, defence, speed, leadership, healing, training);
    }

    // Generate Knight with random facts but not random stats
    public static Person RandomFactKnight(int attack, int defence, int speed)
    {
        int leadership = Random.Range(0, 30);
        int healing = Random.Range(0, 10);
        int training = Random.Range(0, 10);

        return RandomPerson(PersonType.Knight, attack, defence, speed, leadership, healing, training);
    }

    // Generate random Knight
    public static Person RandomKnight()
    {
        int attack = Random.Range(0, 100);
        int defence = Random.Range(0, 100);
        int speed = Random.Range(0, 100);

        return RandomFactKnight(attack, defence, speed);
    }

    // Generate random Knight with stats roughly equal to strength
    public static Person RandomKnight(int strength)
    {
        strength = Mathf.Clamp(strength, 0, 100);
        int min = Mathf.Clamp(strength - 20, 0, 100);
        int mid = strength;
        int max = Mathf.Clamp(strength + 20, 0, 100);

        float power = 1.5f;
        int attack = Utilities.RandomCurve(min, mid, max, power, true);
        int defence = Utilities.RandomCurve(min, mid, max, power, true);
        int speed = Utilities.RandomCurve(min, mid, max, power, true);

        return RandomFactKnight(attack, defence, speed);
    }

    // Generate Lord
    public static Person Lord(string firstName, House house, int birthDate, bool isMale, int attack, int defence, int speed, int leadership, int healing, int training)
    {
        return new Person(PersonType.Lord, firstName, house, birthDate, isMale, attack, defence, speed, leadership, healing, training);
    }
    // Generate random Lord
    public static Person RandomLord()
    {
        int attack = Random.Range(0, 10);
        int defence = Random.Range(0, 10);
        int speed = Random.Range(0, 10);

        int leadership = Random.Range(0, 100);
        int healing = Random.Range(0, 10);
        int training = Random.Range(0, 10);

        return RandomPerson(PersonType.Lord, attack, defence, speed, leadership, healing, training);
    }

    // Generate Healer
    public static Person Healer(string firstName, House house, int birthDate, bool isMale, int attack, int defence, int speed, int leadership, int healing, int training)
    {
        return new Person(PersonType.Healer, firstName, house, birthDate, isMale, attack, defence, speed, leadership, healing, training);
    }

    // Generate Trainer
    public static Person Trainer(string firstName, House house, int birthDate, bool isMale, int attack, int defence, int speed, int leadership, int healing, int training)
    {
        return new Person(PersonType.Trainer, firstName, house, birthDate, isMale, attack, defence, speed, leadership, healing, training);
    }

    // Generate Person with random name, House and age
    public static Person RandomPerson(PersonType personType, int attack, int defence, int speed, int leadership, int healing, int training)
    {
        bool isMale = Random.value <= 0.5;
        int rand;
        string firstName;
        if (isMale)
        {
            rand = Random.Range(0, ExternalData.allMaleFirstNames.Count);
            firstName = ExternalData.allMaleFirstNames[rand];
        }
        else
        {
            rand = Random.Range(0, ExternalData.allFemaleFirstNames.Count);
            firstName = ExternalData.allFemaleFirstNames[rand];
        }

        rand = Random.Range(0, House.allHouses.Count - 1);
        House house = House.allHouses[rand];

        int birthDate = Date.currentDate - Random.Range(16 * 365, 31 * 365);

        return new Person(personType, firstName, house, birthDate, isMale, attack, defence, speed, leadership, healing, training);
    }

    // Constructor
    public Person(PersonType personType, string firstName, House house, int birthDate, bool isMale, int attack, int defence, int speed, int leadership, int healing, int training)
    {
        AddPerson(this);

        this.personType = personType;
        this.firstName = firstName;
        houseID = house.houseID;
        this.birthDate = birthDate;
        deathDate = -1;
        this.isMale = isMale;

        this.attack = attack;
        this.defence = defence;
        this.speed = speed;
        this.leadership = leadership;
        this.healing = healing;
        this.training = training;
        troops = 100;

        _squadronID = -1;
        _childrenIDs = new List<int>();
        _motherID = -1;
        _fatherID = -1;

        CalculateCost();

        this.house.AddHouseMember(this);
    }
}
