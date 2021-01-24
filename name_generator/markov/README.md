# Markov Name Generator

## raw_data

This folder contains a .txt file, the only input file necessary for this tool. The file simply needs to contain names of a particular theme, separated by new lines.

## refine_data.py and data

The refine_data.py file takes the raw data and cleans it. The cleaning process includes removing duplicates, removing any names which contain unnacceptable characters (as decided by the user), converts it all to lower cases and sorting into alphabetical order. The clean data is then written to the a new .txt file, in the data folder.

## train_data.py and trained_data

The train_data.py file takes a refined .txt file (which can be supplied by the refine_data.py script or provided separately) and generates a json representing the probabilities of any given letter following a sequence of other letters, with the maximum number of other letters measured being equal to the 'memory' defined by the user (a larger 'memory' will mean letters must follow more specific patterns, for example: If the name 'derbyshire' is in the data set, with a memory of 1, the letter 'r' must follow either 'e' or 'i', but with a memory of 2, it must follow either 'de' or 'hi', giving less freedom. Of course, in an actual data set you would expect many more occurences of the letter 'r' throughout the data and thus more ways for it to appear.

It should also be noted that two special 'characters' are always included for each: 'start' and 'end' and for all intents and purposes act as normal characters in the Markov processes, with the specific caveat that the first letter of a generated name must follow a 'start' character (so if no names in the original data set begin with 'q', then none will in the generated names); and when 'end' is chosen as the next character of the name, the name ends. Despite the user defining a memory for training the data, the initial letters chosen before the memory value us reached will need to use a slightly lesser memory being that the new generated name does not yet reach the memory requirement.

The trained data is written to a .json file, in the trained_data folder.

## generate_names.py and output

The generate_names.py file takes a trained data .json file (which can be supplied by the train_data.py script or provided separately) and generates however many new names the user specifies. The user may also specify a minimum and maximum length of the new names, and if a name finishes before the minimum length is met, or hasn't reached the 'end' character by the time the maximum length is met, then the new name is discarded and it tries again. The output names are written to a new .txt file, in the output folder.
