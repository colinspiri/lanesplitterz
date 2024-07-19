using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMeter : Meter
{
    public override void EnableMeter()
    {
        base.EnableMeter();
        EnableMouse();
    }

    public override void DisableMeter()
    {
        base.DisableMeter();
        DisableMouse();
    }

    public void EnableMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
