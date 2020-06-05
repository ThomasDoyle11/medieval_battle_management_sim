using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SigilItemPlacement
{
    // 2D location of the SigilItem coord min of 0 and max of 1
    [SerializeField]
    private float[] _centre;
    public Vector2 centre { get { return new Vector2(_centre[0], _centre[1]); } }

    // Size of the SigilItem, between 0 and 1
    [SerializeField]
    private float _size;
    public float size { get { return _size; } }

    // Use Tertiary colour if SigilItem will overlap both Primary and Secondary colour regions, currently must be set manually
    [SerializeField]
    private bool _useAltColour;
    public bool useAltColour { get { return _useAltColour; } }

    // Anchor Min of the rectangle
    public Vector2 anchorMin
    {
        get
        {
            return new Vector2(centre.x - size / 2, centre.y - size / 2);
        }
    }

    // Anchor Max of the rectangle
    public Vector2 anchorMax
    {
        get
        {
            return new Vector2(centre.x + size / 2, centre.y + size / 2);
        }
    }

    // Add placement template to a Transform
    public Transform AddSigilItemPlacement(Transform parent)
    {
        Transform newPlacementLayer = Sigil.newSigilLayer(parent);
        RectTransform newPlacementLayerRect = newPlacementLayer.GetComponent<RectTransform>();
        newPlacementLayerRect.anchorMin = anchorMin;
        newPlacementLayerRect.anchorMax = anchorMax;

        return newPlacementLayer;
    }

    // Public constructor for a SigilItemPlacement
    public SigilItemPlacement(float[] centre, float size, bool useAltColour)
    {
        if (size <= 1 && size >= 0)
        {
            _size = size;
        }
        else
        {
            Debug.Log("Incompatible Item size, set to 0.5");
            _size = 0.5f;
        }

        if (centre.Length == 2)
        {
            if (centre[0] >= 0 && centre[1] >= 0 && centre[0] <= 1 && centre[1] <= 1)
            {
                _centre = centre;
            }
            else
            {
                Debug.Log("Incompatible Item coords, set to (0.5,0.5)");
                _centre = new float[2] { 0.5f, 0.5f };
            }
        }
        else
        {
            Debug.Log("Incompatible Item coords dimensions, set to (0.5,0.5)");
            _centre = new float[2] { 0.5f, 0.5f };
        }

        _useAltColour = useAltColour;
    }
}
