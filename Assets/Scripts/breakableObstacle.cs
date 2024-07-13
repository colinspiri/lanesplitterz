using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakableObstacle : MonoBehaviour
{
    private PlayerMovement _playerMove;

    [SerializeField] AudioSource destroySound;
    [Range(0.01f, 1f)]
    public float fuelSub = 0.1f;

    void Start()
    {
        _playerMove = PlayerMovement.Instance;

        if (!_playerMove) Debug.LogWarning("SpeedPlane Error: No PlayerMovement found");
    }

    void OnTriggerEnter( Collider collider )
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerMove.ReduceFuel(fuelSub);
        }
        else if (collider.gameObject.CompareTag("Enemy Ball"))
        {
            EnemyBall enemyBall = collider.gameObject.GetComponent<EnemyBall>();
            
            enemyBall.ReduceFuel(fuelSub);
        }

        gameObject.SetActive(false);
        destroySound.Play();
    }

}
