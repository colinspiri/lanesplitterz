using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TestVFX : MonoBehaviour
{
    VisualEffect ballSmoke;
    // Start is called before the first frame update
    void Start()
    {
        ballSmoke = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        // if press right arrow key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key was pressed.");
            ballSmoke.SetUInt("SmokeRate", 1000);
        }
        else
        {
            ballSmoke.SetUInt("SmokeRate", 0);
            // ballSmokeLeft.SetUInt("SmokeRate", 0);
            // ballSmokeLeft.enabled = false;
        }
    }
}
