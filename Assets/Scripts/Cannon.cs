using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cannon : MonoBehaviour
{
    [Header("Launch parameters")]
    
    [SerializeField] private float minYaw;
    [SerializeField] private float maxYaw;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float maxLaunchForce;
    [SerializeField] private float minLaunchForce;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Transform launchPoint;

    [SerializeField] private Rigidbody ball;
    [SerializeField] private AudioSource launchSound;

    private bool launched;
    private Vector2 movementInput;
    private float launchForce;
    private Enemy _enemy;

    private void Awake()
    {
        ball.gameObject.SetActive(false);
    }

    private void Start()
    {
        _enemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        
        if (!_enemy) Debug.LogError("Cannon Error: Enemy not found. Is enemy parent disabled?");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!launched)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PowerLevelManager.Instance.EnablePowerSlider();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                Launch();
                return;
            }

            bool up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            bool down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            movementInput = Vector2.zero;
            if (up) movementInput += Vector2.up;
            if (down) movementInput += Vector2.down;
            if (left) movementInput += Vector2.left;
            if (right) movementInput += Vector2.right;
            
            if (movementInput.sqrMagnitude > float.Epsilon)
            {
                Vector3 diff = new Vector3(-movementInput.y, movementInput.x, 0) * (rotateSpeed * Time.deltaTime);
                Vector3 target = transform.rotation.eulerAngles + diff;
                target.x = Mathf.Clamp(RoundAngle(target.x), minPitch, maxPitch);
                target.y = Mathf.Clamp(RoundAngle(target.y), minYaw, maxYaw);
                transform.rotation = Quaternion.Euler(target);
            }
        }
    }

    // normalize angle to [-180,180]
    private float RoundAngle(float angle)
    {
        angle %= 360;
        return angle > 180 ? angle - 360 : angle < -180 ? angle + 360 : angle;
    }

    private void Launch()
    {
        launched = true;
        PowerLevelManager.Instance.DisablePowerSlider();
        
        // update launch force based on power level
        launchForce = PowerLevelManager.Instance.CalculateLaunchForce(minLaunchForce, maxLaunchForce);
        
        // Begin enemy launch sequence
        _enemy.LaunchSequence();

        StartCoroutine(LaunchWithEnemy());
    }

    private IEnumerator LaunchWithEnemy()
    {
        yield return new WaitUntil(() => _enemy.Launched);
        
        ball.gameObject.SetActive(true);
        ball.transform.position = launchPoint.position;
        ball.AddForce(launchForce * transform.forward, ForceMode.Impulse);

        launchSound.Play();
        
        CameraFollowObject.Instance.EnableFollow();
    }
}