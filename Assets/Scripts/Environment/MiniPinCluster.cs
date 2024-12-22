using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniPinCluster : PinClusterParent
{
    private List<Pin> _children;
    private Dictionary<Pin, int> _pinIndices;

    private void Awake()
    {
        _children = new List<Pin>(transform.childCount);
        _pinIndices = new(transform.childCount);

        float minZ = Mathf.Infinity;
        Pin bestPin = null;

        // Identify first pin in cluster (First layer)
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform pinBody = transform.GetChild(i);

            Pin pin = pinBody.GetComponent<Pin>();

            pin.parentCluster = this;

            if (pinBody.position.z < minZ)
            {
                minZ = pinBody.position.z;
                bestPin = pin;
            }
        }

        _children.Add(bestPin);
        _pinIndices[bestPin] = 0;

        // Add second layer of pins
        for (int i = 0; i < transform.childCount; i++)
        {
            Pin pin = transform.GetChild(i).GetComponent<Pin>();

            if (pin == _children[0])
            {
                continue;
            }
            else
            {
                _pinIndices[pin] = _children.Count;

                _children.Add(pin);
            }
        }
    }

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
        // Check for invalid index
        else if (i > 2)
        {
            Debug.LogError("MiniPinCluster Error: Invalid pin index specified");
        }

        return value;
    }

    public override int PinValue(Pin pin)
    {
        return PinValue(_pinIndices[pin]);
    }
}
