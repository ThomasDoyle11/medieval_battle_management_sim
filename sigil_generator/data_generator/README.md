# Data Generator

The 'SigilPatternGenerator.py' tool shown here is essentially a timesaver to prevent having to manually define all possible patterns a Sigil could contain.

The Python file defines a class which, when saved as a JSON file, can be loaded as a 'SigilPattern.cs' file, found in the [Sigil Generator Classes](https://github.com/ThomasDoyle11/medieval_battle_management_sim/tree/master/sigil_generator/classes). The script also contains useful methods for transforming data, and the main method of the script uses these methods within a for loop to generate the data. So far, only 'striped' Sigils are defined, but the plan is to also include code for other common patterns such as 'chequered' and ones which contain circles.
