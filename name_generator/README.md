# Name Generator

The name generator modules can be used to generate random names, be that the name of a place, person, or possibly anything else you could think of.

## simple

This folder contains a simple method for generating only place names. It requires the user to populate 4 lists as inputs: 'prefix', 'start', 'end' and 'suffix'. A random 'start' and 'end' are then concatenated to form the core of the place name, and a random prefix and/or suffix may or may not be added depending on user-defined chances. For example, the place name 'New Yorkshire City' would be generated from a prefix of 'New', a 'start' of 'York', an 'end' of 'shire' and a suffix of 'City'. More example output can be seen in the folder. Some extra logic goes on in concatenating the 'start' and 'end' parts of the name to ensure the names don't look off. For example, given the values 'Harrow' and 'water', concatenating them to form 'Harrowwater' looks wrong due to the double 'w', thus the code will detect this and omit one of the 'w's, resulting in 'Harrowater'. Similarly, if a 'start' ends in a double letter and an 'end' begins with the same letter, one instance of the letter will be omitted ('Grass' + 'side' => 'Grasside').

## markov

This folder contains a project for using Markov methods to generate names for various things. The user need only provide a list of existing names for which they can base their output on and choose a few options (such as whether to include non-alphabetical characters) and the module will generate a json which contains the Markov-trained data, from which the module can generate names indefinitely.

Discrete Markov processes are defined as events who's next state is dependent only in its current state. This definition is extended to discrete Markov processes with Memory 'm', for which the next state is dependent only on the previous 'm' states. This second, more general definition is that which is used for this module, as it encompasses the first definiton and allows for a custom memory. In our case, the 'event' is which letter is chosen next in a word, and this will be dependent on the previous 'm' letters in the word.

## comparison
It is clear that the Markov method produces greater variety and allows for more customisation, however this is at the cost of sometimes getting completely nonsensical names depending on the value for 'memory' you use. The simple method excels at creating realstic place names, but you may come across duplicates or near-duplicates sooner than you'd have wanted to, depending on the length of your input lists.
