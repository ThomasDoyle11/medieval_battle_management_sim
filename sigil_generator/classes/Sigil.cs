using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Sigil
{
    // Number of colours to include in a Sigil
    public static int numColours = 3;

    // Location of the Sigil Layer Prefab
    public static string sigilLayerPrefabLocation = "Sigil/";

    // Load the pre-made for a transform a layer of the Sigil
    public static Transform newSigilLayer(Transform parent)
    {
        return Object.Instantiate(Resources.Load<Transform>(sigilLayerPrefabLocation + "SigilLayer"), parent);
    }

    // The Pattern of the Sigil
    [SerializeField]
    private SigilPattern _pattern;
    public SigilPattern pattern {  get { return _pattern; } }

    // The Item on the Sigil
    [SerializeField]
    private SigilItem _item;
    public SigilItem item { get { return _item; } }

    // The colours of the sigil
    [SerializeField]
    private MyColour[] _sigilColours;
    public MyColour[] sigilColours { get { return _sigilColours; } }

    // The organisation of the Item(s) on the Sigil
    [SerializeField]
    private SigilItemOrganisation _itemOrganisation;
    public SigilItemOrganisation itemOrganisation {  get { return _itemOrganisation; } }

    // Method to draw the Sigil on a given Transform
    public void DrawSigil(Transform sigilTransform)
    {
        // Add the background
        Transform background = newSigilLayer(sigilTransform);
        background.GetComponent<Image>().color = sigilColours[0].colour;

        // Add the Pattern
        Transform patternTransform = pattern.AddSigilPattern(sigilTransform);
        for (int i = 0; i < patternTransform.childCount; i++)
        {
            patternTransform.GetChild(i).GetComponent<Image>().color = sigilColours[1].colour;
        }

        // Add the Item(s)
        for (int i = 0; i < itemOrganisation.itemPlacements.Length; i++)
        {
            SigilItemPlacement newPlacement = itemOrganisation.itemPlacements[i];
            Transform placementTransform = newPlacement.AddSigilItemPlacement(sigilTransform);
            placementTransform.name = item.itemName + " " + i;
            Transform itemTransform = item.AddSigilItem(placementTransform);
            itemTransform.GetComponent<Image>().color = newPlacement.useAltColour ? sigilColours[2].colour : sigilColours[1].colour;
        }
    }

    // Create a random Sigil 
    public static Sigil RandomSigil()
    {
        SigilPattern newPattern = SigilPattern.RandomSigilPattern();
        SigilItem newItem = SigilItem.RandomSigilItem();
        MyColour[] sigilColours = MyColour.randomDistinctColours(numColours);
        return new Sigil(newPattern, newItem, sigilColours);
    }

    // Public contructor for a Sigil
    public Sigil(SigilPattern pattern, SigilItem item, MyColour[] sigilColours)
    {
        _pattern = pattern;
        _item = item;

        if (sigilColours.Length != numColours)
        {
            _sigilColours = new MyColour[numColours];

            if (sigilColours.Length != numColours)
            {
                Debug.Log("Only " + sigilColours.Length + " Sigil Colours provided, expected " + numColours + ", the rest have been chosen randomly");
            }
            else
            {
                Debug.Log("" + sigilColours.Length + " Sigil Colours provided, expected " + numColours + ", the rest have been ignored");
            }

            for (int i = 0; i < numColours; i++)
            {
                if (i < sigilColours.Length)
                {
                    _sigilColours[i] = sigilColours[i];
                }
                else
                {
                    _sigilColours[i] = MyColour.randomColour;
                }
            }
        }
        else
        {
            _sigilColours = sigilColours;
        }

        _itemOrganisation = _pattern.RandomSigilItemOrganisation();
        if (_itemOrganisation == null)
        {
            _itemOrganisation = new SigilItemOrganisation(new SigilItemPlacement[0]);
        }
    }
}
