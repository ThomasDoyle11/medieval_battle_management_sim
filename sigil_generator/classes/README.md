# Classes

Each of the C# scripts here is used to contain data about a Sigil.

## Sigil.cs

This can be thought of as the container class for the Sigil; if you want to access any information about the Sigil with regards to another class (such as Sigil Pattern or Sigil Item), then it can be accessed through this class, as it contains a reference to all these sub-classes (sometimes indirectly, i.e. through a child of a child). It defines 3 distinct colours for the Sigils; the primary and secondary colour are used for the pattern, and the secondary colour is used for Sigil Items rendered on the primary colour. The tertiary colour is used if the Sigil Item is rendered over both colours.

It also contains a method to return a randomly-generated Sigil using the data generated in 'data_generator' folder, and a method to draw the Sigil onto an object within the Unity scene.

## SigilPattern.cs

This script describes the geometric pattern of a Sigil, which is built up using the following 'primitive' patterns: **SigilCircle.cs**, **SigilRectangle.cs** and **SigilLines.cs**. This allows for a large degree of freedom in forming patterns, but not infinite freedom. The script also contains all possible ways in which the Sigil Items can be placed for this particular pattern.

## SigilItemPlacement.cs and SigilItemOrganisation.cs

The SigilItemPlacement.cs script describles a single way in which a Sigil Item can be placed in a script; describing the position, size and colour of the Sigil Item. The SigilItemOrganisiton.cs is simply a collecion of Sigil Item Placements, allowing a single Sigil Pattern to show it's Sigil Item more than once.

## SigilItem.cs

This script simply contains a reference to a Sigil Item image in the project resources which can be loaded into the scene.
