using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class breakableObstacle : MonoBehaviour
{
    private PlayerMovement _playerMove;

    [SerializeField] AudioSource destroySound;
    [SerializeField] private GameEvent onPlayerHitObstacle;
    [SerializeField] GameObject intactCube;
    [SerializeField] GameObject brokenCube;
    [SerializeField] GameObject model;
    [SerializeField] bool isTutorial;

    [Range(0.01f, 1f)]
    public float fuelSub = 0.1f;
    private float step = 1f;
    private bool playedSound = false;

    void Start()
    {
        _playerMove = PlayerMovement.Instance;

        if (!_playerMove) Debug.LogWarning("SpeedPlane Error: No PlayerMovement found");
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerMove.ReduceFuel(fuelSub);
            if (onPlayerHitObstacle)
                onPlayerHitObstacle.Raise();
            destroySound.spatialBlend = 0.0f;
        }
        else if (collider.gameObject.CompareTag("Enemy Ball"))
        {
            EnemyBall enemyBall = collider.gameObject.GetComponent<EnemyBall>();

            enemyBall.ReduceFuel(fuelSub);
            destroySound.spatialBlend = 1.0f;
        }

        intactCube.SetActive(false);
        brokenCube.SetActive(true);

        if (!playedSound)
        {
            destroySound.Play();
            playedSound = true;
        }
        
        StartCoroutine(TimerRoutine());

    }

    private IEnumerator TimerRoutine()
    {
        if(isTutorial == true)
        {
            step = 5f;
        }
        else
        {
            step = 1f;
        }
        yield return new WaitForSecondsRealtime(step);
        model.SetActive(false);
    }

}
