# Classes

This folder is a selection of C# scripts used in the **Mediecal Battle Management Simulation**. Of these scripts:

- Three of these scripts (Competition, Person and Squadron) can be thought of as entities, real-world 'objects' which contain data defining them.
- One can be thought of as an 'intermediary' class (CompSquad).
- One class is simply a container for logic to find a relationship between two Persons.

The organisation of these classes and the references to each other have been inspired by my experience in dealing with **Relational Databases**.

## Entities

The three entities mentioned reference each other heavily: Each Competition has multiple Squadrons, which in turn contain multiple Persons. The Competition will also contain multiple Battles, and each Battle will need to reference 2 Squadrons, and the Persons that will be involved in the Battle, as this won't be all the Persons in the Squadron. The Squadron, however will not need to reference the Battles, as these can be accessed through the Competition. The Squadron will be a part of multiple Competitions, creating a many-to-many relationship. This leads to the need for an 'intermediary' class...

## Intermediary

A Competition contains multiple Squadrons, a Squadron will be in multiple Competitions. There will be data specific to the relationship between each Squadron and Competition, such as the points earned from winning Battles, and this can be stored in the intermediary CompSquad class.

This relationships between the entities can be crudely summarised [here](https://github.com/ThomasDoyle11/medieval_battle_management_sim/blob/master/classes/entity-relationship.png).

## Relationship

The Relationship script is used to calculate the relationship between two Persons. It uses a recursive method which can be tuned as to how many generations it will search from the base Person. The script contains names for each relationship as is found in real life, from Mother-Daughter to 14th cousins twice-removed. This information is stored as 2 separate integers, the 'generation difference', which states the number of generations from the base Person to their relation, and the 'separation', which states how many generations from the base Person to the nearest common x-grandparent. It should be noted that the Relationship is unique from the base Person to their relation, and a function is provided which finds the inverse.

The class also keeps other data about the relationship, such as whether the two Persons are part of the same Dynasty, and makes use of data from the people such as their sex/gender to specify the names of relationships which depend on this.

---

#### It is important to note that this is just one way to model the entities and their relationships, and this could be subject to change. For example, at present each Person references only a single Squadron, their current one. This could, however, change in future versions when a Person may contain a history of previous Squadrons they have belonged too, and this may lead to the need for a further 'intermediary class' (PersonSquad) to contain any data necessary for that Persons time in that Squadron (such as `startDate` and `endDate`).
