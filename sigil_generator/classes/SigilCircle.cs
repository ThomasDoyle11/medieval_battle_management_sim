using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SigilCircle
{
    // Centre of the circle (x,y)
    [SerializeField]
    private float[] _centre;
    public Vector2 centre { get { return new Vector2(_centre[0], _centre[1]); } }

    // Diameter of the circle
    [SerializeField]
    private float _diameter;
    public float diameter { get { return _diameter; } }
    public float radius { get { return _diameter / 2; } }

    // Anchor Min of the rectangle
    public Vector2 anchorMin
    {
        get
        {
            return new Vector2(centre.x - radius, centre.y - radius);
        }
    }

    // Anchor Max of the rectangle
    public Vector2 anchorMax
    {
        get
        {
            return new Vector2(centre.x + radius, centre.y + radius);
        }
    }

    // Add the SigilCircle to a given Transform
    public Transform AddSigilCircle(Transform parent)
    {

        Transform newCircleLayer = Sigil.newSigilLayer(parent);

        Sprite sigiCircleSprite = Resources.Load<Sprite>(Sigil.sigilLayerPrefabLocation + "circle");
        if (sigiCircleSprite != null)
        {
            newCircleLayer.GetComponent<Image>().sprite = sigiCircleSprite;
        }
        else
        {
            Debug.Log("No SigilCircle sprite found at '" + Sigil.sigilLayerPrefabLocation + "circle', returning empty object instead.");
        }

        RectTransform newCircleLayerRect = newCircleLayer.GetComponent<RectTransform>();
        newCircleLayerRect.anchorMin = anchorMin;
        newCircleLayerRect.anchorMax = anchorMax;

        return newCircleLayer;
    }

    // Public constructor for a SigilCircle
    public SigilCircle(float[] centre, float diameter)
    {
        if (centre.Length == 2)
        {
            _centre = centre;
        }
        else
        {
            Debug.Log("Centre must be a 2D coordinate (x,y)");
            _centre = new float[2] { 0.5f, 0.5f };
        }
        _diameter = diameter;
    }
}
