# Data Generator

The 'SigilPatternGenerator.py' tool is essentially a timesaver to prevent having to manually define all possible patterns a Sigil could use. The Python file defines a class which, when saved as a JSON file, can be loaded as a 'SigilPattern.cs' scipt, found in the [Sigil Generator Classes](https://github.com/ThomasDoyle11/medieval_battle_management_sim/tree/master/sigil_generator/classes). The script also contains useful methods for transforming data, and the main method of the script uses these methods within a for loop to generate the data. So far, only 'striped' Sigils are defined, but the plan is to also include code for other common patterns such as 'chequered' and ones which contain circles.

The 'sigil_patterns.json' file is an example of the output from the generator.
