using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public int explosionForce;

    [SerializeField] private GameObject effects;

    private int _ballLayer;
    private AudioSource[] _audioSources;
    private bool _destructing;

    private void Start()
    {
        _ballLayer = LayerMask.NameToLayer("Balls");
        _audioSources = effects.GetComponents<AudioSource>();
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
                Debug.Log("Player explosion");

                colliderObj.GetComponent<PlayerMovement>().Explode(explosionForce);
            }
            // Enemy hit
            else
            {
                Debug.Log("Enemy explosion");

                colliderObj.GetComponent<EnemyBall>().Explode(explosionForce);
            }

            _audioSources[Random.Range(0, _audioSources.Length)].Play();

            Destroy(gameObject);

            _destructing = true;
        }
    }
}
