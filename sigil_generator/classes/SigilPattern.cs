using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SigilPattern
{
    // List of all SigilPatterns
    public static List<SigilPattern> allSigilPatterns { get; set; } = new List<SigilPattern>();

    // Name assigned to this pattern
    [SerializeField]
    private string _patternName;
    public string patternName { get { return _patternName; } }

    // All rectangles in the pattern
    [SerializeField]
    private SigilRectangle[] _rectangles;
    public SigilRectangle[] rectangles { get { return _rectangles; } }

    // All lines in the pattern
    [SerializeField]
    private SigilLine[] _lines;
    public SigilLine[] lines { get { return _lines; } }

    // All circles in the pattern
    [SerializeField]
    private SigilCircle[] _circles;
    public SigilCircle[] circles { get { return _circles; } }

    // All ways of organising SigilItems within this pattern
    [SerializeField]
    private SigilItemOrganisation[] _allItemOrganisations;
    public SigilItemOrganisation[] allItemOrganisations { get { return _allItemOrganisations; } }
    public SigilItemOrganisation RandomSigilItemOrganisation()
    {
        if (allItemOrganisations.Length > 0)
        {
            int rand = Random.Range(0, allItemOrganisations.Length);
            return allItemOrganisations[rand];
        }
        else
        {
            return null;
        }
    }

    // Add the Sigil Pattern to the parent Transform
    public Transform AddSigilPattern(Transform parent)
    {
        Transform sigilTopLayer = Sigil.newSigilLayer(parent);
        sigilTopLayer.name = patternName;

        // Draw rectangles
        for (int i = 0; i < rectangles.Length; i++)
        {
            Transform newRectangleLayer = rectangles[i].AddSigilRectangle(sigilTopLayer);
            newRectangleLayer.name = "rectangle " + i;
        }

        // Draw lines
        for (int i = 0; i < lines.Length; i++)
        {
            Transform newLineLayer = lines[i].AddSigilLine(sigilTopLayer);
            newLineLayer.name = "line " + i;
        }

        // Draw circles
        for (int i = 0; i < circles.Length; i++)
        {
            Transform newCircleLayer = circles[i].AddSigilCircle(sigilTopLayer);
            newCircleLayer.name = "circle " + i;
        }

        return sigilTopLayer;
    }

    // Returns a random SigilPattern
    public static SigilPattern RandomSigilPattern()
    {
        int rand = Random.Range(0, allSigilPatterns.Count);
        return allSigilPatterns[rand];
    }

    // Public constructor for a SigilPattern
    public SigilPattern(string patternName, SigilRectangle[] rectangles, SigilLine[] lines, SigilCircle[] circles, SigilItemOrganisation[] allItemOrganisations)
    {
        _patternName = patternName;
        _rectangles = rectangles;
        _lines = lines;
        _circles = circles;
        _allItemOrganisations = allItemOrganisations;
    }
}
