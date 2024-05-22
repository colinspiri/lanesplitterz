using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPinSound : MonoBehaviour
{
    private bool _hit;
    [SerializeField] AudioSource pinHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Pin") && !_hit)
        {
            _hit = true;
            pinHit.Play();
        }
    }
}
