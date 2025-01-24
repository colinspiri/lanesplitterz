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

    public GameObject launchVFX;
    public bool acceptingInputs;

    private bool launched;
    private Vector2 movementInput;
    private Enemy _enemy;
    private int numSpacePressed = 0;
    private bool _isFirstThrow;
    private static Cannon _instance = null;

    private void Awake()
    {
        _isFirstThrow = true;
        if (_instance == null)
        {
            _instance = this;
        }
    }
    private void Start()
    {
        RoundManager.OnNewThrow += Initialize;
        RoundManager.OnNewRound += Initialize;
        RoundManager.OnNewThrow += () => numSpacePressed = 0;
        RoundManager.OnNewRound += () => numSpacePressed = 0;
        RoundManager.OnNewRound += () => DetermineDisableInputs();
        RoundManager.OnNewThrow += () => DisableInputsTutorial();
        DisableInputs();
        UpdateEnemy();
        Initialize();
    }

    private void Initialize()
    {
        pivot.rotation = Quaternion.identity;
        trajectoryLine.SetPositions();
        launched = false;
    }

    private void Update()
    {
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

                    if (_isFirstThrow && playerInfo.isPracticing) _isFirstThrow = false;

                    Launch();
                    numSpacePressed = 0;
                    return; 
                }

                numSpacePressed++;
            }

            bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            movementInput = Vector2.zero;
            if (left) movementInput += Vector2.left;
            if (right) movementInput += Vector2.right;

            if (movementInput.sqrMagnitude > float.Epsilon)
            {
                Vector3 diff = new Vector3(-movementInput.y, movementInput.x, 0) * (rotateSpeed * Time.deltaTime);
                Vector3 target = transform.rotation.eulerAngles + diff;
                target.x = Mathf.Clamp(RoundAngle(target.x), minPitch, maxPitch);
                target.y = Mathf.Clamp(RoundAngle(target.y), minYaw, maxYaw);
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

        if (gameState.currentThrow == 1)
        {
            PinManager.Instance.playerWhitePinCount = 0;
            PinManager.Instance.playerGoldPinCount = 0;
            PinManager.Instance.enemyWhitePinCount = 0;
            PinManager.Instance.enemyGoldPinCount = 0;

        }
        
        playerInfo.isReady = false;

        StartCoroutine(LaunchWithEnemy());
    }

    private IEnumerator LaunchWithEnemy()
    {
        if (_enemy) yield return new WaitUntil(() => _enemy.Launched);

        ball.gameObject.SetActive(true);
        ball.transform.position = launchPoint.position;

        // play vfx
        if (launchVFX)
        {
            ParticleSystem vfx = launchVFX.GetComponent<ParticleSystem>();
            vfx.Play();
        }

        // Linear acceleration
        ball.AddForce(powerMeterData.meterValue * transform.forward, ForceMode.Impulse); // meterValue is the launch force

        LaunchedBall.Raise();

        launchSound.Play();
    }

    [YarnCommand("EnableInputs")]
    public bool EnableInputs() => acceptingInputs = true;

    [YarnCommand("DisableInputs")]
    public void DisableInputs() => acceptingInputs = false;

    public void DetermineDisableInputs()
    {
        if (playerInfo.isPracticing == false) DisableInputs();
    }

    private void DisableInputsTutorial()
    {
        if (!playerInfo.isPracticing) return;
        if (playerInfo.isPracticing == true && gameState.isTutorialSecondThrow) DisableInputs();
    }

    public static void UpdateEnemy()
    {
        _instance._enemy = GameObject.FindWithTag("Enemy Parent")?.GetComponent<Enemy>();
        if (!_instance._enemy) Debug.LogWarning("Cannon Error: Enemy not found. Is enemy parent disabled?");
    }
}