using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakableObstacle : MonoBehaviour
{
    private PlayerMovement _playerMove;

    [SerializeField] AudioSource destroySound;
    [Range(0.01f, 1f)]
    [SerializeField] private float fuelSub = 0.1f;

    void Awake()
    {
        _playerMove = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();

        if (!_playerMove) Debug.LogError("SpeedPlane Error: No PlayerMovement found");
    }

    void OnTriggerEnter( Collider collider )
    {
        gameObject.SetActive(false);
        _playerMove.ReduceFuel(fuelSub);

        destroySound.Play();
    }

}
