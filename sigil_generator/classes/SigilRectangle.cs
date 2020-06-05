using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SigilRectangle
{
    // Centre of the rectangle (x,y)
    [SerializeField]
    private float[] _centre;
    public Vector2 centre { get { return new Vector2(_centre[0], _centre[1]); } }

    // Width of the rectangle
    [SerializeField]
    private float _width;
    public float width { get { return _width; } }

    // Height of the rectangle
    [SerializeField]
    private float _height;
    public float height { get { return _height; } }

    // Anchor Min of the rectangle
    public Vector2 anchorMin
    {
        get
        {
            return new Vector2(centre.x - width / 2, centre.y - height / 2);
        }
    }

    // Anchor Max of the rectangle
    public Vector2 anchorMax
    {
        get
        {
            return new Vector2(centre.x + width / 2, centre.y + height / 2);
        }
    }

    // Rotation of the rectangle in degrees
    [SerializeField]
    private float _rotation;
    public float rotation { get { return _rotation; } }

    // Add a SigilRectangle to the given Transform
    public Transform AddSigilRectangle(Transform parent)
    {
        Transform newRectangleLayer = Sigil.newSigilLayer(parent);
        RectTransform newRectangleLayerRect = newRectangleLayer.GetComponent<RectTransform>();
        newRectangleLayerRect.anchorMin = anchorMin;
        newRectangleLayerRect.anchorMax = anchorMax;
        newRectangleLayerRect.localRotation = Quaternion.Euler(0f, 0f, rotation);

        return newRectangleLayer;
    }

    // Public constructor for a SigilRectangle
    public SigilRectangle(float[] centre, float width, float height, float rotation)
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

        _width = width;
        _height = height;
        _rotation = rotation % 360;
        if (rotation < 0 || rotation >= 360)
        {
            Debug.Log("Rotation outside the boundaries of 0 <= rotation < 360, set to " + _rotation + " instead of " + rotation + ".");
        }
    }
}
