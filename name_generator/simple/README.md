
# Simple Name Generator

## input
This folder contains the only necessary input file; a .txt file with 4 lines of name-parts. The 4 lines are, respectively: prefixes, 'starts', 'ends' and suffixes, and each of these parts are used to generate the place name.

## PlaceNameGenerator.py and output

The 'PlaceNameGenerator.py' file uses the name-parts within the input file to generate random place names, with the user specifying the probability of the name containing a prefix and/or suffix. The output is then written to a file in the output folder.

## file_checker.py

This simple script goes through the input file and removes any duplicates so that they don't skew the results (which is useful if you're manually adding name-parts to the input and erroneously input one twice). It also allows the user to search for strings within each name-part to see what name-parts already exist.
