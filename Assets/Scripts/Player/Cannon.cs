using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using GameAudioScriptingEssentials;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class Cannon : MonoBehaviour
{
    [Header("Launch parameters")]
    [SerializeField] private float minYaw;
    [SerializeField] private float maxYaw;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private Transform pivot;

    [Header("Scriptable Objects")]
    [SerializeField] private MeterData powerMeterData;
    [SerializeField] private MeterData spinMeterData;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameState gameState;

    [Header("Events")]
    [SerializeField] private GameEvent ConfirmedCannonPosition;
    [SerializeField] private GameEvent ConfirmedCannonPositionTutorial;
    [SerializeField] private GameEvent LaunchedBall;
    [SerializeField] private MeterDataGameEvent ConfirmedLaunchPower;
    [SerializeField] private GameEvent ConfirmedLaunchPowerTutorial;
    [SerializeField] private MeterDataGameEvent ConfirmedInitialSpin;
    [SerializeField] private GameEvent ConfirmedInitialSpinTutorial;

    [Header("Audio")]
    [SerializeField] MusicController musicController;
    [SerializeField] private AudioSource launchSound;
    [SerializeField] private AdaptiveMusicContainer turnSound;

    [Space]
    [SerializeField] private Rigidbody ball;
    [SerializeField] private TrajectoryLine trajectoryLine;

    public bool acceptingInputs;
    private bool launched;
    private Vector2 movementInput;
    private Enemy _enemy;
    private int numSpacePressed = 0;
    private bool _isFirstThrow;

    private void Awake()
    {
        _enemy = GameObject.FindWithTag("Enemy Parent")?.GetComponent<Enemy>();
        if (!_enemy) Debug.LogWarning("Cannon Error: Enemy not found. Is enemy parent disabled?");

        _isFirstThrow = true;
    }
    private void Start()
    {
        RoundManager.OnNewThrow += Initialize;
        RoundManager.OnNewRound += Initialize;
        RoundManager.OnNewThrow += () => numSpacePressed = 0;
        RoundManager.OnNewRound += () => numSpacePressed = 0;
        RoundManager.OnNewRound += () => DetermineDisableInputs();
        Initialize();
    }

    private void Initialize()
    {
        //transform.rotation = Quaternion.identity;
        pivot.rotation = Quaternion.identity;

        launched = false;

        trajectoryLine.SetPositions();
        //launchForce = 0;
        //PowerLevelManager.Instance.DisablePowerSlider();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!launched)
        {
            if (acceptingInputs == true && Input.GetKeyDown(KeyCode.Space))
            {
                if (numSpacePressed == 0)
                {
                    ConfirmedCannonPosition.Raise();
                    if (_isFirstThrow && playerInfo.isPracticing) ConfirmedCannonPositionTutorial.Raise();
                }
                else if (numSpacePressed == 1)
                {
                    ConfirmedLaunchPower.Raise(powerMeterData);
                    if (_isFirstThrow && playerInfo.isPracticing) ConfirmedLaunchPowerTutorial.Raise();
                }
                else if (numSpacePressed == 2)
                {
                    ConfirmedInitialSpin.Raise(spinMeterData);
                    if (_isFirstThrow && playerInfo.isPracticing)
                    {
                        ConfirmedInitialSpinTutorial.Raise();
                        _isFirstThrow = false;
                    }

                    Launch();
                    numSpacePressed = 0;
                    return;
                }

                numSpacePressed++;
            }

            bool up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            bool down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            movementInput = Vector2.zero;
/*            if (up) movementInput += Vector2.up;
            if (down) movementInput += Vector2.down;*/
            if (left) movementInput += Vector2.left;
            if (right) movementInput += Vector2.right;

            if (movementInput.sqrMagnitude > float.Epsilon)
            {
                Vector3 diff = new Vector3(-movementInput.y, movementInput.x, 0) * (rotateSpeed * Time.deltaTime);
                Vector3 target = transform.rotation.eulerAngles + diff;
                target.x = Mathf.Clamp(RoundAngle(target.x), minPitch, maxPitch);
                target.y = Mathf.Clamp(RoundAngle(target.y), minYaw, maxYaw);
                //transform.rotation = Quaternion.Euler(target);
                pivot.rotation = Quaternion.Euler(target);
                trajectoryLine.SetPositions();
                if (!turnSound._isPlaying)
                {
                    turnSound.RunContainer();   
                }
                else if (turnSound._currentSection == 2)
                {
                    turnSound.TransitionSection(0);
                }
            }
            else if (turnSound._currentSection != 2)
            {
                turnSound.TransitionSection(0);
            }
            /*if (transform.rotation.x > maxPitch || transform.rotation.x < minPitch)
            {
                moving = false;
                turnSound.Stop();
            }*/
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

        musicController.Launch();

        // Begin enemy launch sequence
        if (_enemy) _enemy.LaunchSequence();

        StartCoroutine(LaunchWithEnemy());
    }

    private IEnumerator LaunchWithEnemy()
    {
        if (_enemy) yield return new WaitUntil(() => _enemy.Launched);

        ball.gameObject.SetActive(true);
        ball.transform.position = launchPoint.position;

        // Linear acceleration
        ball.AddForce(powerMeterData.meterValue * transform.forward, ForceMode.Impulse); // meterValue is the launch force
        PlayerMovement.Instance.Spin(spinMeterData.meterValue * -1f); // meterValue is the spin force

        LaunchedBall.Raise();

        // ball.GetComponent<PlayerMovement>().Spin(-1000f);

        launchSound.Play();
    }

    [YarnCommand("EnableInputs")]
    public bool EnableInputs() => acceptingInputs = true;

    [YarnCommand("DisableInputs")]
    public bool DisableInputs() => acceptingInputs = false;

    public void DetermineDisableInputs()
    {
        if (playerInfo.isPracticing == false) DisableInputs();
    }

/*    // used temporarily until corpo dialogue is implemented (we can enable inputs in the yarn file instead)
    public void DetermineEnableInputs()
    {
        if (playerInfo.isPracticing == false && gameState.currentLevelIndex >= 6) EnableInputs();
    }*/
}