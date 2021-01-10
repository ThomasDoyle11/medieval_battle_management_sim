---
layout: default
---

# Saved Data

This section shows some examples of the data that can saved from one session of 'Medieval Battle Management Simulation' and loaded in another.

The saved data makes use of the fact that all entities such as `Person`, `Competition` and `Squadron` have unique integer IDs, meaning each class need only serialize primitive types that serve as a reference to another entity.

This behaviour can be seen in [squadron_example.json](https://github.com/ThomasDoyle11/medieval_battle_management_sim/blob/master/saved_data/squadron_example.json), where the saved data contains the following data:

| Key | Value |
| --- | --- |
| `_squadronID` | `125` |
| `_allKnightIDs` | `[2626, 2627, ...]` |

The `squadronID` can then be used by other entities which need reference a Squadron, such as a Person, who is a member of a Squadron, or a Competition, which contains multiple Squadrons. Similarly, this saved data contains a collection of integer IDs, `_allKnightIDs` which contain references to all Knights (which are just Person entities with an enum to differentiate their 'type').

The classes from which this data is generated can be found in the [classes](https://github.com/ThomasDoyle11/medieval_battle_management_sim/tree/master/classes) folder.
