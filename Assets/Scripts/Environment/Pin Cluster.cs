using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinCluster : MonoBehaviour
{
    private Dictionary<Pin, int> _childPoints;

    private void Awake()
    {
        _childPoints = new();
    }

    private void Start()
    {
        Pin[] children = new Pin[transform.childCount];
        
        for (int i = 0; i < transform.childCount; i++)
        {
            Pin pin = transform.GetChild(i).GetComponent<Pin>();
            children[i] = pin;
            pin.parentCluster = this;
        }

        // 0's children: 1 and 2
        // 1's children: 3 and 4
        // 2's children: 4 and 5
        // 3's children: 6 and 7
        // 4's children: 7 and 8
        // 5's children: 8 and 9

        // +1, +2 twice, +3 thrice

        // Fourth layer
        for (int i = 9; i >= 6; i--)
        {
            if (children[i].pinState != Pin.PinState.Untouched)
            {
                _childPoints[children[i]] = 0;
            }
            else
            {
                _childPoints[children[i]] = children[i].PointValue;
            }
        }

        // Third layer
        for (int i = 5; i >= 3; i--)
        {
            if (children[i].pinState != Pin.PinState.Untouched)
            {
                _childPoints[children[i]] = 0;
            }
            else
            {
                _childPoints[children[i]] = children[i].PointValue + children[i + 3].PointValue + children[i + 4].PointValue;
            }
        }

        // Second layer
        for (int i = 2; i >= 1; i--)
        {
            if (children[i].pinState != Pin.PinState.Untouched)
            {
                _childPoints[children[i]] = 0;
            }
            else
            {
                _childPoints[children[i]] = children[i].PointValue + children[i + 2].PointValue + children[i + 3].PointValue;
            }
        }

        // First layer
        if (children[0].pinState != Pin.PinState.Untouched)
        {
            _childPoints[children[0]] = 0;
        }
        else
        {
            _childPoints[children[0]] = children[0].PointValue + children[1].PointValue + children[2].PointValue;
        }
    }

    public int PinValue(Transform pin)
    {
        return _childPoints[pin.GetComponent<Pin>()];
    }

    public int PinValue(GameObject pin)
    {
        return _childPoints[pin.GetComponent<Pin>()];
    }

    public int PinValue(Pin pin)
    {
        return _childPoints[pin];
    }
}
