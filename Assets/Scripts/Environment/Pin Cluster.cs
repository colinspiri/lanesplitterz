using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinCluster : PinClusterParent
{
    // private Dictionary<Pin, int> _childPoints;
    private Pin[] _children;
    private Dictionary<Pin, int> _pinIndices;

    private void Awake()
    {
        // _childPoints = new();

        _children = new Pin[transform.childCount];
        _pinIndices = new(transform.childCount);

        for (int i = 0; i < transform.childCount; i++)
        {
            Pin pin = transform.GetChild(i).GetComponent<Pin>();
            _children[i] = pin;
            _pinIndices[pin] = i;
            // pin.parentCluster = this;
        }
    }

    //private void UpdatePinValues(int i)
    //{
    //    // 0's children: 1 and 2
    //    // 1's children: 3 and 4
    //    // 2's children: 4 and 5
    //    // 3's children: 6 and 7
    //    // 4's children: 7 and 8
    //    // 5's children: 8 and 9

    //    // +1, +2 twice, +3 thrice

    //    // Fourth layer
    //    for (int i = 9; i >= 6; i--)
    //    {
    //        if (_children[i].pinState != Pin.PinState.Untouched)
    //        {
    //            _childPoints[_children[i]] = 0;
    //        }
    //        else
    //        {
    //            _childPoints[_children[i]] = _children[i].PointValue;
    //        }
    //    }

    //    // Third layer
    //    for (int i = 5; i >= 3; i--)
    //    {
    //        _childPoints[_children[i]] = _children[i + 3].PointValue + _children[i + 4].PointValue;

    //        if (_children[i].pinState != Pin.PinState.Untouched)
    //        {
    //            _childPoints[_children[i]] += _children[i].PointValue;
    //        }
    //    }

    //    // Second layer
    //    for (int i = 2; i >= 1; i--)
    //    {
    //        _childPoints[_children[i]] = _children[i + 2].PointValue + _children[i + 3].PointValue;

    //        if (_children[i].pinState != Pin.PinState.Untouched)
    //        {
    //            _childPoints[_children[i]] += _children[i].PointValue;
    //        }
    //    }

    //    // First layer
    //    _childPoints[_children[0]] = _children[1].PointValue + _children[2].PointValue;

    //    if (_children[0].pinState != Pin.PinState.Untouched)
    //    {
    //        _childPoints[_children[0]] += _children[0].PointValue;
    //    }
    //}

    //public int PinValue(Transform pin)
    //{
    //    return _childPoints[pin.GetComponent<Pin>()];
    //}

    //public int PinValue(GameObject pin)
    //{
    //    return _childPoints[pin.GetComponent<Pin>()];
    //}

    protected override int PinValue(int i)
    {
        int value = 0;

        // Add the pin's value to itself
        if (_children[i].pinState == Pin.PinState.Untouched)
        {
            value += _children[i].PointValue;
        }

        // First layer
        if (i == 0)
        {
            value = PinValue(1) + PinValue(2);
        }
        // Second layer
        else if (i < 3)
        {
            value = PinValue(i + 2) + PinValue(i + 3);
        }
        // Third layer
        else if (i < 6)
        {
            value = PinValue(i + 3) + PinValue(i + 4);
        }
        // Check for invalid index
        else if (i > 9)
        {
            Debug.LogError("PinCluster Error: Invalid pin index specified");
        }

        return value;
    }

    public override int PinValue(Pin pin)
    {
        return PinValue(_pinIndices[pin]);
    }
}
