using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PinClusterParent : MonoBehaviour
{
    public abstract int PinValue(Pin pin);

    protected abstract int PinValue(int i);
}
