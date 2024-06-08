using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Pin Collider Layer is " + LayerMask.LayerToName(collision.gameObject.layer));
    }
}
