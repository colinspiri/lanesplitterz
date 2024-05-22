using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPinSound : MonoBehaviour
{
    private bool _hit;
    [SerializeField] AudioSource pinHit;
    [SerializeField] AudioSource bigFall;
    [SerializeField] AudioSource smallFall;

    public float wait = 0.15f;
    public IntReference intReference;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Pin") && !_hit)
        {
            _hit = true;
            pinHit.Play();
            StartCoroutine(PinsFall());
        }
    }

    private IEnumerator PinsFall()
    {
        yield return new WaitForSeconds(wait);
        if (intReference.Value > 5)
        {
            bigFall.Play();
        }
        else
        {
            smallFall.Play();
        }
    }
}
