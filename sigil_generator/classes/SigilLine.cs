using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SigilLine
{
    // Start point of the line
    [SerializeField]
    private float[] _start;
    public Vector2 start { get { return new Vector2(_start[0], _start[1]); } }

    // End point of the line
    [SerializeField]
    private float[] _end;
    public Vector2 end { get { return new Vector2(_end[0], _end[1]); } }

    // The centre of the line
    public Vector2 centre
    {
        get
        {
            return (start + end) / 2;
        }
    }

    // The length of the line
    public float length
    {
        get
        {
            return (start - end).magnitude;
        }
    }

    // Rotation of the line clockwise from the y axis (0 <= rotation < 360)
    public float rotation
    {
        get
        {
            float returnFloat = Vector2.SignedAngle(Vector2.up, end - start);
            if (returnFloat < 0)
            {
                returnFloat += 360;
            }
            return returnFloat;
        }
    }

    // Anchor Min of the rectangle
    public Vector2 anchorMin
    {
        get
        {
            return new Vector2(centre.x - width / 2, centre.y - length / 2);
        }
    }

    // Anchor Max of the rectangle
    public Vector2 anchorMax
    {
        get
        {
            return new Vector2(centre.x + width / 2, centre.y + length / 2);
        }
    }

    // Add a line to an existing Transform
    public Transform AddSigilLine(Transform parent)
    {
        Transform newLineLayer = Sigil.newSigilLayer(parent);
        RectTransform newLineLayerRect = newLineLayer.GetComponent<RectTransform>();
        newLineLayerRect.anchorMin = anchorMin;
        newLineLayerRect.anchorMax = anchorMax;
        newLineLayerRect.localRotation = Quaternion.Euler(0f, 0f, rotation);

        return newLineLayer;
    }

    // Width of the line
    [SerializeField]
    private float _width;
    public float width { get { return _width; } }

    // Public constructor a SigilLine
    public SigilLine(float[] start, float[] end, float width)
    {
        if (start.Length == 2)
        {
            _start = start;
        }
        else
        {
            Debug.Log("Start must be a 2D coordinate (x,y)");
        }

        if (end.Length == 2)
        {
            _end = end;
        }
        else
        {
            Debug.Log("End must be a 2D coordinate (x,y)");
        }

        _width = width;
    }
}
