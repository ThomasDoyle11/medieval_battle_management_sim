using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationship
{
    public Person basePerson;

    public Person relationPerson;

    // Maximum separation to search for
    // 0 will only be self, 1 will be parents and children, 2 will be grandparents, grandchildren and siblings etc.
    public static int maxDistance = 7;
    
    // Difference in generation
    public int generationDifference;

    // Distance to nearest common x-grandparent
    // If initial Person IS the common x-grandparent, value will be 0
    public int separation;

    // Number of parent-child links between individuals
    public int distance
    {
        get
        {
            return 2 * separation - generationDifference;
        }
    }

    // Separation according to relation
    public int inverseSeparation
    {
        get
        {
            int newSep;
            if (separation == -1)
            {
                newSep = -1;
            }
            else
            {
                newSep = separation - generationDifference;
            }
            return newSep;
        }
    }

    // The name given to the Relationship
    public string name;

    // If the two People share a common House
    public bool isDynastic;

    // The relationship between the second and first Person
    // e.g. if the Relationship is 'son' the inverse will be 'father' or 'mother'
    public Relationship inverse
    {
        get
        {
            return new Relationship(relationPerson, basePerson, inverseSeparation, -generationDifference, isDynastic);
        }
    }

    // People IDs that have already been checked for the relationship, to prevent duplicates
    private List<int> checkedIDs = new List<int>();

    // Check whether the given Person and any children below them are the relation
    private bool CheckPersonAndChildren(int newID, int relationID, int generationDifference, int separation)
    {
        this.generationDifference = generationDifference;
        this.separation = separation;

        // Debug.Log(generationDifference + ", " + separation + " (" + distance + ")");
        if (newID == relationID)
        {
            // Check if the people are the same
            return true;
        }
        else
        {
            checkedIDs.Add(newID);
            // Check the Persons children if we haven't exceeded maxmimum dist
            if (distance < maxDistance)
            {
                List<int> childrenIDs = Person.GetPerson(newID).childrenIDs;
                for (int i = 0; i < childrenIDs.Count; i++)
                {
                    if (!checkedIDs.Contains(childrenIDs[i]))
                    {
                        // Debug.Log("Checking " + childrenIDs[i]);
                        if (CheckPersonAndChildren(childrenIDs[i], relationID, generationDifference - 1, separation))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }

    // Check whether the parents of the given Person are the relation
    private bool CheckParents(int newID, int relationID, int generationDifference, int separation)
    {

        if (CheckPersonAndChildren(newID, relationID, generationDifference, separation))
        {
            return true;
        }
        else
        {
            this.generationDifference = generationDifference;
            this.separation = separation;

            if (distance < maxDistance)
            {
                int motherID = Person.GetPerson(newID).motherID;
                if (motherID != -1 && !checkedIDs.Contains(motherID))
                {
                    if (CheckParents(motherID, relationID, generationDifference + 1, separation + 1))
                    {
                        return true;
                    }
                }
                int fatherID = Person.GetPerson(newID).fatherID;
                if (fatherID != -1 && !checkedIDs.Contains(fatherID))
                {
                    if (CheckParents(fatherID, relationID, generationDifference + 1, separation + 1))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    // Get the name of the Relationship
    public string getName
    {
        get
        {
            string returnName = "";
            if (separation == -1)
            {
                returnName = "none";
            }
            else if (separation <= 1 || inverseSeparation <= 1)
            {
                if (separation == 0)
                {
                    if (generationDifference == 0)
                    {
                        returnName = "self";
                    }
                    else
                    {
                        if (relationPerson.isMale)
                        {
                            returnName += "son";
                        }
                        else
                        {
                            returnName += "daughter";
                        }
                    }
                }
                else if (separation == 1 && inverseSeparation != 0)
                {
                    if (generationDifference == 0)
                    {
                        if (relationPerson.isMale)
                        {
                            returnName = "brother";
                        }
                        else
                        {
                            returnName = "sister";
                        }
                    }
                    else if (generationDifference < 0)
                    {
                        if (relationPerson.isMale)
                        {
                            returnName += "nephew";
                        }
                        else
                        {
                            returnName += "niece";
                        }
                    }
                }
                else if (inverseSeparation == 0)
                {
                    if (relationPerson.isMale)
                    {
                        returnName = "father";
                    }
                    else
                    {
                        returnName = "mother";
                    }
                }
                else if (inverseSeparation == 1)
                {
                    if (relationPerson.isMale)
                    {
                        returnName = "uncle";
                    }
                    else
                    {
                        returnName = "aunt";
                    }
                }
                if (Mathf.Abs(generationDifference) > 1)
                {
                    returnName = "grand" + returnName;
                }
                if (Mathf.Abs(generationDifference) > 2)
                {
                    int greatness = Mathf.Abs(generationDifference) - 2;
                    for (int i = 0; i < greatness; i++)
                    {
                        returnName = "great-" + returnName;
                    }
                }
            }
            else if (distance <= maxDistance)
            {
                // Cousins and distant cousins
                int cousinNumber = separation - 1 - Mathf.Max(0, generationDifference);
                string ordinalSuffix;

                if (cousinNumber == 1)
                {
                    ordinalSuffix = "st";
                }
                else if (cousinNumber == 2)
                {
                    ordinalSuffix = "nd";
                }
                else if (cousinNumber == 3)
                {
                    ordinalSuffix = "rd";
                }
                else
                {
                    ordinalSuffix = "th";
                }

                int removedNumber = Mathf.Abs(generationDifference);
                string removedTerm;
                if (removedNumber == 0)
                {
                    removedTerm = "";
                }
                else if (removedNumber == 1)
                {
                    removedTerm = " once removed";
                }
                else if (removedNumber == 2)
                {
                    removedTerm = " twice removed";
                }
                else if (removedNumber == 3)
                {
                    removedTerm = " thrice removed";
                }
                else
                {
                    removedTerm = " " + removedNumber + " times removed";
                }


                string direction;
                if (generationDifference == 0)
                {
                    direction = "";
                }
                else if (generationDifference > 0)
                {
                    direction = " (above)";
                }
                else
                {
                    direction = " (below)";
                }
                returnName = cousinNumber + ordinalSuffix + " cousin" + removedTerm + direction;
            }
            else
            {
                returnName = "distant relation";
            }
            return returnName;
        }
    }

    // Constructor for the Relationship between two people which calculates the relationship
    public Relationship(Person basePerson, Person relationPerson)
    {
        this.basePerson = basePerson;
        this.relationPerson = relationPerson;

        isDynastic = basePerson.houseID == relationPerson.houseID;

        // CheckParents(basePerson.personID, relationPerson.personID, 0, 0);

        bool isRelated = CheckParents(basePerson.personID, relationPerson.personID, 0, 0);

        if (!isRelated)
        {
            separation = -1;
            generationDifference = 0;
        }

        name = getName;
    }

    // Constructor for the Relationship where the parameters are given, not calculated
    public Relationship(Person basePerson, Person relationPerson, int separation, int generationDifference, bool isDynastic)
    {
        this.basePerson = basePerson;
        this.relationPerson = relationPerson;
        this.separation = separation;
        this.generationDifference = generationDifference;
        this.isDynastic = isDynastic;
        name = getName;
    }
}
