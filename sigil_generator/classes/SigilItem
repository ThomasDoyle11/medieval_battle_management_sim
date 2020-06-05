using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SigilItem
{
    // List of all SigilItems
    public static List<SigilItem> allSigilItems = new List<SigilItem>();

    // Location of SigilItem sprite files in storage
    public static string sigilItemLocation = Sigil.sigilLayerPrefabLocation + "SigilItems/";

    // Name assigned to this SigilIten
    [SerializeField]
    private string _itemName;
    public string itemName { get { return _itemName; } }

    // Location of the SigilItem sprite file
    // The file name in which the sigilItem is stored
    [SerializeField]
    private string _fileName;
    public string fileName { get { return _fileName; } }

    // Add a Sigil Item to the given Transform
    public Transform AddSigilItem(Transform parent)
    {
        Sprite sigilItemSprite = Resources.Load<Sprite>(sigilItemLocation + fileName);
        Transform newItemLayer = Sigil.newSigilLayer(parent);
        newItemLayer.name = itemName;
        if (sigilItemSprite != null)
        {
            newItemLayer.GetComponent<Image>().sprite = sigilItemSprite;
        }
        else
        {
            Debug.Log("No SigilItem found at " + sigilItemLocation + fileName + ", returning empty object instead.");
        }

        return newItemLayer;
    }

    // Returns a random SigilItem
    public static SigilItem RandomSigilItem()
    {
        int rand = Random.Range(0, allSigilItems.Count);
        return allSigilItems[rand];
    }

    // Public constructor for a SigilItem
    public SigilItem(string itemName, string fileName)
    {
        _itemName = itemName;
        _fileName = fileName;
    }
}
