using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaitForInput : CustomYieldInstruction
{
    private KeyCode[] _keys;
    
    public override bool keepWaiting
    {
        get
        {
            return _keys.All(key => !Input.GetKeyDown(key));
        }
    }

    public WaitForInput(KeyCode key)
    {
        _keys = new []{key};
    }
    
    public WaitForInput(KeyCode[] keys)
    {
        _keys = keys;
    }
}
