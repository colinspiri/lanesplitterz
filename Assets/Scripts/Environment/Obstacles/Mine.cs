using GameAudioScriptingEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public int explosionForce;

    private int _ballLayer;
    [SerializeField] private AudioClipRandomizer explosionSFX;
       
    private bool _destructing;

    private void Start()
    {
        _ballLayer = LayerMask.NameToLayer("Balls");
        _destructing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject colliderObj = other.gameObject;

        if (colliderObj.layer == _ballLayer && !_destructing)
        {
            // Player hit
            if (colliderObj.CompareTag("Player"))
            {
                colliderObj.GetComponent<PlayerMovement>().Explode(explosionForce);
                explosionSFX._spatialBlend = 0.0f;
            }
            // Enemy hit
            else
            {
                colliderObj.GetComponent<EnemyBall>().Explode(explosionForce);
                explosionSFX._spatialBlend = 1.0f;
            }

            explosionSFX.PlaySFX();

            Destroy(gameObject);

            _destructing = true;
        }
    }
}
