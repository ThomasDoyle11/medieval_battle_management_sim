using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SigilItemOrganisation
{
    // All Item placements for this SigilItemOrganisation
    [SerializeField]
    private SigilItemPlacement[] _itemPlacements;
    public SigilItemPlacement[] itemPlacements { get { return _itemPlacements; } }

    // Public constructor for a SigilItemOrganisation
    public SigilItemOrganisation(SigilItemPlacement[] itemPlacements)
    {
        _itemPlacements = itemPlacements;
    }
}
