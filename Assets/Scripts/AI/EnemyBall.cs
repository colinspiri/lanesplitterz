using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class EnemyBall : MonoBehaviour
{
    [Header("Public Flags")]
    public bool enableFuelLoss = true;
    public bool enableExtraGravity = true;
    public bool attackPlayer = false;

    [Header("AI Configuration")]
    [Tooltip("Frequency in seconds for the AI to consider updating position")]
    [SerializeField] private float aiUpdateTime = 1f;
    [Tooltip("How far ahead obstacles can be detected")]
    public float visibleLimit = 20f;
    [Tooltip("Layers of objects the AI cares about")]
    [SerializeField] private LayerMask visibleLayers;
    [Tooltip("Time to straighten out after a turn")]
    [SerializeField] private float straightenSeconds;
    [Tooltip("The amount of time in the future to consider for possible positions")]
    [SerializeField] private float secondsToConsider;
    [Tooltip("The sampling rate of the time to consider")]
    [SerializeField] private float considerTimeSteps;
    [Tooltip("The difference in two potential positions needed to choose a new one (Prevents thrashing)")]
    [SerializeField] private float changePosDiff;
    [Tooltip("The aggression level toward the player if attacking them. 0 is none, 1 is full aggression")]
    [SerializeField] private float playerAggression = 0f;
    [Tooltip("Percent of fuel lost on hitting the player")]
    [SerializeField] private float playerFuelLoss;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float fuelWeight;
    [SerializeField] private float speedWeight;
    [SerializeField] private float pointWeight;
    [SerializeField] private float controlWeight;
    //[SerializeField] private float distWeight;
    //[SerializeField] private float angleWeight;
    // The position being moved to
    private Vector3 _targetPos;
    private float _targetScore;
    // A dictionary caching object values
    private Dictionary<GameObject, float> _valueCache;
    private int totalPoints = 0;
    private float totalSpeed = 0f;

    // The true update time for checking positions
    private float _posUpdateTime;

    [Header("Force specifications")] [SerializeField]
    private float turnForce;
    //[SerializeField] private float hookForceMultiplier;
    //[SerializeField] private float turnSpeedPerSecond = 0.1f;
    [SerializeField] private float slipperyMod = 10f;
    [SerializeField] private float minimumSpeed;
    public float extraGravity;

    [Header("Explosion specifications")]
    //[Tooltip("The amount of bounce for explosions to give the ball")]
    //[SerializeField] private float explosionUpwards;
    //[Tooltip("The maximum radius (in meters) from the center of the ball for an explosion")]
    //[SerializeField] private float explosionRadius;
    [SerializeField] private float explosionFOV;
    [SerializeField] private float explosionVerticality;

    [Header("Fuel specifications")]
    [Tooltip("Amount of fuel expended per second while steering left or right")]
    [SerializeField] private float turnFuel;
    [Tooltip("Amount of fuel expended per second while accelerating (not decelerating)")]
    [SerializeField] private float accelFuel;

    #region Private State
    
    // -1 for turning left, 0 for not turning, 1 for turning right
    private float _turnVal;
    private float _accelVal;
    // private bool _turning;
    [SerializeField] private float _fuelMeter = 1f;
    private float _currentSpin = 0f;
    private GameObject _ground = null;
    private Coroutine _turnRoutine = null;
    private Coroutine _straightenRoutine = null;
    private bool _moving = false;
    private bool _flying = true;
    private Vector3 _lastVelocity;
    private bool _initialized = false;
    private bool _outOfFuel = false;

    #endregion

    #region Start Caches

    private Rigidbody _myBody;
    private Quaternion _refInvRot;
    [SerializeField] private Transform rotationRef;
    [SerializeField] private AudioSource hitGutter;
    private Enemy _myParentScript;
    private Bounds _laneBounds;
    private int _pinLayer;
    private int _obstacleLayer;
    private int _ballLayer;
    private int _barrierLayer;
    private int _groundLayer;
    private SphereCollider _myCollider;
    private float _myRadius;

    #endregion
    
    #region Debugging

    [SerializeField] private bool showActualPositions = false;
    [SerializeField] private bool verboseActualPositions = false;
    [SerializeField] private bool showPossiblePositions = false;
    [SerializeField] private bool verbosePossiblePositions = false;
    [SerializeField] private EnemyPattern enemyPattern;
    [SerializeField] private GameObject gizmoObj;
    private EnemyPattern _possiblePositions;
    private int _posCount = 0;
    [SerializeField] private bool aiEnabled = true;
    
    #endregion

    #region MonoBehaviour Event Functions

    // misc
    private int _groundMask;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!_initialized)
        {
            // Clear enemy pattern object regardless of whether debugging is being used
            enemyPattern.Instantiate();

            if (_possiblePositions == null) _possiblePositions = new();
            _possiblePositions.Instantiate();


            _valueCache = new();
            _myBody = GetComponent<Rigidbody>();

            _outOfFuel = false;

            _initialized = true;
        }
    }

    private void Start()
    {
        GameObject boundsObj = GameObject.FindWithTag("AI Bounds");

        if (boundsObj)
        {
            _laneBounds = boundsObj.GetComponent<Collider>().bounds;
        }
        

        _pinLayer = LayerMask.NameToLayer("Pins");
        _obstacleLayer = LayerMask.NameToLayer("Obstacles");
        _ballLayer = LayerMask.NameToLayer("Balls");
        _groundLayer = LayerMask.NameToLayer("Ground");
        _barrierLayer = LayerMask.NameToLayer("Barriers");

        _myCollider = GetComponent<SphereCollider>();
        _myCollider.hasModifiableContacts = true;

        _myRadius = _myCollider.radius * transform.lossyScale.x;

        _groundMask = LayerMask.GetMask("Ground");

        _posUpdateTime = Mathf.Max(aiUpdateTime, Time.fixedDeltaTime);

        _myParentScript = transform.parent.GetComponent<Enemy>();

        // _possiblePositions = new();

        StartMovement();
    }

    private void OnEnable()
    {
        _flying = true;
    }

    private void OnDisable()
    {
        HaltMovement();
        // if (showActualPositions) enemyPattern.Instantiate();
    }

    public void EnableBall()
    {
        gameObject.SetActive(true);
        StartMovement();
        if (showActualPositions) enemyPattern.Instantiate();
    }

    private void FixedUpdate()
    {
        if (enableExtraGravity) ExtraGravity();
        
        Quaternion parentRotation = rotationRef.rotation;
        _refInvRot = Quaternion.Inverse(new Quaternion(parentRotation.x, 0f, parentRotation.z, parentRotation.w));

        Vector3 parentForward = (_refInvRot * rotationRef.forward).normalized;
        float forwardLinVelocity = Vector3.Project(_myBody.velocity, parentForward).magnitude;
        if (forwardLinVelocity < minimumSpeed) Accelerate(1f, false);

        // If out of fuel, stop trying to be smart
        if (_fuelMeter <= Mathf.Epsilon && !_outOfFuel)
        {
            _outOfFuel = true;

            Debug.Log("Enemy out of fuel");

            StopAllCoroutines();
            StartCoroutine(Straighten());
        }


        UpdateGround();
        // Hook();

        _lastVelocity = _myBody.velocity;
    }
    
    private void OnDrawGizmos()
    {
        if (showActualPositions)
        {
            for (int i = 0; i < enemyPattern.GetCount(); i++)
            {
                Gizmos.color = Color.Lerp(Color.red, Color.blue, i / 100f);
            
                Gizmos.DrawSphere(enemyPattern.GetPosition(i), 1.5f);

                if (i > 0)
                {
                    Gizmos.DrawLine(enemyPattern.GetPosition(i - 1), enemyPattern.GetPosition(i));
                }
            }
        }

        if (showPossiblePositions && _possiblePositions != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _possiblePositions.GetCount(); i++)
            {
                Gizmos.DrawSphere(_possiblePositions.GetPosition(i), 1.5f);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == _pinLayer)
        {
            ContactPoint[] contactList = new ContactPoint[collision.contactCount];

            collision.GetContacts(contactList);

            foreach (ContactPoint contact in contactList)
            {
                _myBody.AddForceAtPosition(-contact.impulse, contact.point, ForceMode.Impulse);
            }

            // HoldVelocity();
        }
    }

    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            ReduceFuel(playerFuelLoss);
        }
        else if (collider.gameObject.CompareTag("Lane Bounds"))
        {
            LockedToGround(true);
        }
        else if (collider.gameObject.CompareTag("Gutter"))
        {
            hitGutter.Play();
            StartCoroutine(GutterStraighten(0.25f));
        }
    }
    
    #endregion
    
    #region Movement Functions
    
    // public void Turn(float turnVal, bool expendFuel = true)
    // {
    //     if (expendFuel)
    //     {
    //         float fuelReduction = turnFuel * Time.fixedDeltaTime;
    //
    //         if (_fuelMeter <= Mathf.Epsilon)
    //         {
    //             return;
    //         }
    //         else
    //         {
    //             ReduceFuel(fuelReduction);
    //         }
    //     }
    //
    //
    //     _currentSpin += turnVal * turnSpeedPerSecond * Time.deltaTime;
    //     if (_currentSpin > 100) _currentSpin = 100;
    //     else if (_currentSpin < -100) _currentSpin = -100;
    //
    //     Vector3 linForce = (_refInvRot * rotationRef.right) * turnVal;
    //     Vector3 rotForce = (_refInvRot * rotationRef.up) * turnVal;
    //
    //     // Skid if on ice (reduce acceleration)
    //     // if (IsIcy())
    //     // {
    //     //     Debug.Log("Turning skidding on ice");
    //     //     
    //     //     linForce /= slipperyMod;
    //     //     rotForce /= slipperyMod;
    //     // }
    //
    //     // Linear acceleration
    //     _myBody.AddForce(linForce, ForceMode.Impulse);
    //
    //     // Rotational acceleration
    //     _myBody.AddTorque(rotForce, ForceMode.Impulse);
    // }
    
    public void Turn(float turnVal, bool expendFuel = true)
    {
        if (expendFuel)
        {
            float fuelReduction = turnFuel * Time.fixedDeltaTime;

            if (_fuelMeter <= Mathf.Epsilon)
            {
                return;
            }
            else
            {
                ReduceFuel(fuelReduction);
            }
        }


        Vector3 linForce = (_refInvRot * rotationRef.right) * turnVal;
        Vector3 rotForce = (_refInvRot * rotationRef.up) * turnVal;

        // Skid if on ice (reduce acceleration)
        // if (IsIcy())
        // {
        //     Debug.Log("Turning skidding on ice");
        //     
        //     linForce /= slipperyMod;
        //     rotForce /= slipperyMod;
        // }

        // Linear acceleration
        _myBody.AddForce(linForce, ForceMode.Impulse);

        // Rotational acceleration
        _myBody.AddTorque(rotForce, ForceMode.Impulse);
    }

    // Emulate frictional movement to the side
    //private void Hook()
    //{
    //    if (!Grounded() || IsIcy()) return;

    //    float hookForceMagnitude = _currentSpin * hookForceMultiplier;
    //    // I have absolutely no idea if what is in the parenthesis is correct (for player ball its _camInvRot * _myCam.right)
    //    Vector3 hookForce = (_refInvRot * rotationRef.right) * hookForceMagnitude;

    //    _myBody.AddForce(hookForce);

    //    /*Vector3 camUp = _camInvRot * _myCam.up; 
    //    float force = Vector3.Dot(_myBody.angularVelocity, camUp.normalized); 
    //    Turn(force * hookMultiplier, false);*/
    //}

    // accelVal is the force to accelerate with
    public void Accelerate(float accelVal, bool expendFuel = true)
    {
        Vector3 linearForce;
        Vector3 rotationalForce;

        Vector3 parentForward = (_refInvRot * rotationRef.forward).normalized;
        Vector3 parentRight = (_refInvRot * rotationRef.right).normalized;

        // Put on the brakes
        if (accelVal < 0)
        {
            Vector3 forwardLinVelocity = Vector3.Project(_myBody.velocity, parentForward);
            linearForce = forwardLinVelocity * accelVal;

            Vector3 forwardRotVelocity = Vector3.Project(_myBody.angularVelocity, parentRight);
            rotationalForce = forwardRotVelocity * accelVal;
        }
        // Accelerate ahead
        else
        {
            if (expendFuel)
            {
                float fuelReduction = accelFuel * Time.fixedDeltaTime;

                if (_fuelMeter <= Mathf.Epsilon)
                {
                    return;
                }
                else
                {
                    ReduceFuel(fuelReduction);
                }
            }

            linearForce = parentForward * accelVal;
            rotationalForce = parentRight * accelVal;
        }

        // Skid if on ice (reduce acceleration)
        if (IsIcy())
        {
            // Debug.Log("Acceleration skidding on ice");

            linearForce /= slipperyMod;
            rotationalForce /= slipperyMod;
        }

        // Linear acceleration
        _myBody.AddForce(linearForce, ForceMode.Impulse);

        // Rotational acceleration
        _myBody.AddTorque(rotationalForce, ForceMode.Impulse);
    }

    // Overload for acceleration in an arbitrary direction
    // Forces velocity along new direction, then adds extra force
    /* Can probably be split into multiple methods (See Steer() below) */
    public void Accelerate(Vector3 accelForce, bool expendFuel = true)
    {
        if (expendFuel)
        {
            float fuelReduction = accelFuel * Time.fixedDeltaTime;

            if (_fuelMeter <= Mathf.Epsilon)
            {
                return;
            }
            else
            {
                ReduceFuel(fuelReduction);
            }
        }

        Vector3 camUp = (_refInvRot * rotationRef.up).normalized;
        Vector3 linearForce = accelForce;
        Vector3 rotationalForce = Vector3.Cross(accelForce, camUp);

        // Skid if on ice (reduce acceleration)
        if (IsIcy())
        {
            // Debug.Log("Acceleration skidding on ice");

            linearForce /= slipperyMod;
            rotationalForce /= slipperyMod;
        }

        // Steer existing velocity toward direction of force
        _myBody.velocity = linearForce.normalized * _myBody.velocity.magnitude;
        _myBody.angularVelocity = rotationalForce.normalized * _myBody.angularVelocity.magnitude;

        // Linear acceleration 
        _myBody.AddForce(linearForce, ForceMode.Impulse);

        // Rotational acceleration
        _myBody.AddTorque(rotationalForce, ForceMode.Impulse);
    }

    private void UpdateGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, (_refInvRot * rotationRef.up) * -1f, out hit,
                _myCollider.radius + 1f, _groundMask))
        {
            _ground = hit.collider.gameObject;
        }
        else _ground = null;
    }

    // Returns true if on the ground, false otherwise
    private bool Grounded()
    {
        return _ground;
    }

    //Returns true is on icy ground, false if not
    private bool IsIcy()
    {
        return _ground && _ground.CompareTag("Icy");
    }

    // Consider other possible positions to move towards
    private IEnumerator CheckPositions()
    {
        while (true)
        {
            yield return new WaitForSeconds(_posUpdateTime);
            
            // Don't check positions while turning
            // if (_turning) conatinue;
            
            // Box cast to find objects ahead
            //Collider[] visibleObstacles = Physics.OverlapBox(transform.position,
            //    new Vector3(50f, 50f, visibleLimit), rotationRef.rotation, visibleLayers);
            
            float bestValue = Mathf.NegativeInfinity;
            Vector3 bestPos = Vector3.zero;
            float bestDir = 0f;
            // float bestTime = 0f;
            string bestString = null;
            float timeStep = secondsToConsider / considerTimeSteps;

            if (showPossiblePositions)
            {
                _possiblePositions.ClearPositions();
                // Debug.Log("Drawing possible positions for position " + _posCount + 1 + ":");
            }

            // Project positions horizontally in time
            for (float i = -1f * secondsToConsider, c = 0; i < secondsToConsider + 0.1f; i += timeStep)
            {
                float time = Mathf.Abs(i);
                // Negative going left, positive going right
                float dir = Mathf.Sign(i);

                // Project positions vertically in time
                // for (float j = time; j < secondsToConsider + 0.1f; j += timeStep)
                // float verticalTime = secondsToConsider - j;

                // Calculate predicted horizontal position
                // a = Fdt / (m * dt)
                Vector3 impulse = _refInvRot * rotationRef.right * (turnForce * dir);
                Vector3 accel = impulse / (_myBody.mass * Time.fixedDeltaTime);
                // Vector3 horizontalPos = PredictPosition(time, in accel, transform.position);

                // Calculate predicted final position
                // Vector3 predictedPos = PredictPosition(verticalTime, horizontalPos);

                Vector3 predictedPos = PredictPosition(time, in accel, transform.position);

                // If out of bounds, skip to next position
                if (predictedPos.x > _laneBounds.max.x ||
                    predictedPos.z > _laneBounds.max.z ||
                    predictedPos.x < _laneBounds.min.x ||
                    predictedPos.z < _laneBounds.min.z)
                {
                    continue;
                }

                if (showPossiblePositions)
                {
                    _possiblePositions.AddPosition(predictedPos, gizmoObj);
                }

                string possibleString = "";
                if (showPossiblePositions && verbosePossiblePositions) possibleString = "Possible target " + c + " at time " + i + "\n";

                c++;

                string predictedString = "";
                if (showActualPositions && verboseActualPositions) predictedString = "Actual target " + _posCount + "\n";

                // Quality of the position (more positive is better)
                float posValue = 0f;
                    
                // Expected fuel loss to reach position
                float expectedFuelLoss = turnFuel * time;
                // Deduct expected fuel cost from quality
                posValue -= expectedFuelLoss * fuelWeight;

                if (showPossiblePositions && verbosePossiblePositions)
                {
                    possibleString += "Expected fuel loss: " + expectedFuelLoss + "\n";
                    possibleString += "Scaled fuel cost: " + expectedFuelLoss * -fuelWeight + "\n";
                }

                if (showActualPositions && verboseActualPositions)
                {
                    predictedString += "Expected fuel loss: " + expectedFuelLoss + "\n";
                    predictedString += "Scaled fuel cost: " + expectedFuelLoss * -fuelWeight + "\n";
                }

                RaycastHit[] obstaclesAhead = Physics.SphereCastAll(predictedPos, 1f, _refInvRot * rotationRef.forward,
                    visibleLimit, visibleLayers);

                Vector3 diff = predictedPos - transform.position;

                RaycastHit[] obstaclesInBetween = Physics.SphereCastAll(transform.position, 1f, diff.normalized,
                    diff.magnitude + 0.5f, visibleLayers);

                HashSet<GameObject> obstacleCache = new();


                //Collider[] obstaclesAhead = Physics.OverlapBox(predictedPos,
                //    new Vector3(2f, 50f, visibleLimit), rotationRef.rotation, visibleLayers);

                // Evaluate all obstacles ahead of this position
                for (int k = 0; k < obstaclesAhead.Length; k++)
                {
                    GameObject obstacle = obstaclesAhead[k].collider.gameObject;

                    obstacleCache.Add(obstacle);
                        
                    // Ignore the enemy ball itself as an obstacle
                    if (obstacle == gameObject) continue;

                    Bounds obsBounds = obstaclesAhead[k].collider.bounds;

                    posValue += ScoreObstacle(obstacle, obsBounds, ref predictedString, ref possibleString, ref predictedPos, false);
                }

                // Evaluate all obstacles on the way to this position
                for (int k = 0; k < obstaclesInBetween.Length; k++)
                {
                    GameObject obstacle = obstaclesInBetween[k].collider.gameObject;

                    if (obstacleCache.Contains(obstacle))
                    {
                        continue;
                    }

                    // Ignore the enemy ball itself as an obstacle
                    if (obstacle == gameObject) continue;

                    Bounds obsBounds = obstaclesInBetween[k].collider.bounds;

                    posValue += ScoreObstacle(obstacle, obsBounds, ref predictedString, ref possibleString, ref predictedPos, true);
                }

                // Scale position value inversely by how far ahead it is
                //float secondsBound = secondsToConsider * 1.5f;
                //float timeScalar = 1f - Mathf.Pow(time / secondsBound, 2f);

                //predictedString += "Time scalar is " + timeScalar;

                //posValue *= timeScalar;

                // Reduces position value by 10% each 5 meters
                if (attackPlayer)
                {
                    float dist = Vector3.Distance(predictedPos, player.transform.position);

                    posValue *= 1f - ((dist * playerAggression) / 50f);

                    // posValue -= Mathf.Clamp(InvXDistance(predictedPos, player.transform.position, playerAggression, 2), 0f, 1f);
                }

                if (showPossiblePositions && verbosePossiblePositions) possibleString += "Value of position " + _posCount + ": " + posValue;

                if (showActualPositions && verboseActualPositions) predictedString += "Value of position " + _posCount + ": " + posValue;

                // if (showPossiblePositions) Debug.Log("Tentative position at time " + i + "\n" + predictedString);

                if (showPossiblePositions) Debug.Log(possibleString);

                // Update best positions if better position is found
                if (posValue > bestValue)
                {
                    bestValue = posValue;
                    bestPos = predictedPos;
                    bestDir = dir;
                    // bestTime = time;
                    if (showActualPositions) bestString = predictedString;
                }
            }

            // Move to best position if it's better than current target or you've passed the current target
            /* Assumes positive z is forward */
            if (bestValue - _targetScore >= changePosDiff || transform.position.z > _targetPos.z)
            {
                _posCount++;
                _targetPos = bestPos;
                _targetScore = bestValue;
                if (showActualPositions)
                {
                    Debug.Log(bestString);
                    if (bestString != null)
                    {
                        enemyPattern.AddPosition(bestPos, gizmoObj);
                    }
                }
                
                if (_turnRoutine != null)
                {
                    StopCoroutine(_turnRoutine);
                }
            
                _turnRoutine = StartCoroutine(TurnSequence(bestDir));
            }
        }
    }

    private float ScoreObstacle(GameObject obstacle, Bounds obsBounds, ref string predictedString, ref string possibleString, ref Vector3 predictedPos, bool inBetween)
    {
        if (showActualPositions)
        {
            predictedString += "Evaluating obstacle " + obstacle.name + " with instance ID " + obstacle.GetInstanceID() + "\n";
        }

        if (showPossiblePositions)
        {
            possibleString += "Evaluating obstacle " + obstacle.name + " with instance ID " + obstacle.GetInstanceID() + "\n";
        }

        // The value of the obstacle, positive meaning it benefits you, negative meaning it hurts
        float obsValue = 0f;

        // Pin
        if (obstacle.layer == _pinLayer)
        {
            Pin pin = obstacle.GetComponent<Pin>();

            // Pin is knocked down
            if (pin.pinState != Pin.PinState.Untouched)
            {
                obsValue = 0;
            }
            // Pin is part of cluster
            else if (pin.parentCluster)
            {
                obsValue = pointWeight * pin.parentCluster.PinValue(pin) / totalPoints;
            }
            // Pin is individual and hasn't been knocked down
            else
            {
                obsValue = _valueCache[obstacle];
            }
        }
        // Obstacle
        else if (obstacle.layer == _obstacleLayer)
        {
            if (obstacle.CompareTag("Speed Pad"))
            {
                obsValue = _valueCache[obstacle];
            }
            else if (obstacle.CompareTag("Damage Obstacle"))
            {
                billboardMovement billboardMove = obstacle.GetComponent<billboardMovement>();

                if (billboardMove.destroyed)
                {
                    obsValue = 0f;
                }
                else
                {
                    obsValue = _valueCache[obstacle];
                }
            }
            else if (obstacle.CompareTag("Mine"))
            {
                obsValue = _valueCache[obstacle];
            }
        }
        // Barrier
        else if (obstacle.layer == _barrierLayer)
        {
            breakableObstacle breakableObs = obstacle.GetComponent<breakableObstacle>();
            if (breakableObs.destroyed)
            {
                obsValue = 0f;
            }
            else
            {
                obsValue = _valueCache[obstacle];
            }
        }
        // Player ball
        else if (obstacle.layer == _ballLayer)
        {
            if (attackPlayer)
            {
                obsValue = 0f;
            }
            else
            {
                obsValue = _valueCache[obstacle];
            }
        }
        // Oil slick
        else if (obstacle.layer == _groundLayer)
        {
            if (obstacle.CompareTag("Icy"))
            {
                obsValue = _valueCache[obstacle];
            }
            else
            {
                return 0f;
            }
        }

        if (showActualPositions && verboseActualPositions) predictedString += "    Unscaled value: " + obsValue + "\n";

        if (showPossiblePositions && verbosePossiblePositions) possibleString += "    Unscaled value: " + obsValue + "\n";

        if (!inBetween)
        {
            Vector3 nearestPoint = obsBounds.ClosestPoint(predictedPos);

            // Scale obstacle value by cos of angle between obstacle and position
            // An angle of 90 degrees means it's unreachable and is worthless
            // An angle of 0 degrees means it's straight-ahead and worth its full value
            Vector3 posToObs = nearestPoint - predictedPos;
            posToObs.y = 0f;
            posToObs = posToObs.normalized;

            Vector3 parentForward = _refInvRot * rotationRef.forward;
            parentForward.y = 0f;
            parentForward = parentForward.normalized;

            // Dot product is equal to 1 at 0 degrees (obs *= 1), 0 at 90 degrees (obs *= 0), -1 at 180 degrees (obs *= 0)
            obsValue *= Mathf.Clamp(Vector3.Dot(posToObs, parentForward), 0f, 1f);

            if (showActualPositions && verboseActualPositions) predictedString += "    Value accounting for angle: " + obsValue + "\n";
            if (showPossiblePositions && verbosePossiblePositions) possibleString += "    Value accounting for angle: " + obsValue + "\n";

            // Scale obstacle value according to its distance from the position

            // Calculate inverse distance to obstacle

            //float invDist;

            //if (obsBounds.Contains(predictedPos))
            //{
            //    invDist = 1f;
            //}
            //else
            //{
            //    // float invDist = InvDistance(predictedPos, obstacle.transform.position, 0.9f);
            //    // float invDist = InvSquareDistanceBounds(predictedPos, obsBounds, 0.9f);
            //    invDist = InvDistanceBounds(predictedPos, obsBounds, 1f);
            //}

            //// Don't want distance below 0.1 to matter, so clamped to 1 (inv dist of 0.1 is 1)
            //invDist = Mathf.Clamp(invDist, 0f, 1f);
            //obsValue *= invDist;

            // If distance is <= 35 meters, full value
            // If distance is 70 meters, 75% value
            // If distance is 105 meters, 50% value
            // If distance is 140 meters, 25% value
            // If distance is 175 meters, 0% value
            // Value Scalar: 1 - (((dist / 35) - 1) / 4)
            // Simplified: 1.25 - (dist / 140f)

            float dist = Vector3.Distance(predictedPos, nearestPoint);

            // obsValue *= Mathf.Clamp(1.25f - (dist / 140f), 0f, 1f);

            obsValue *= 1.25f - (dist / 140f);

            if (showActualPositions && verboseActualPositions) predictedString += "    Value accounting for distance: " + obsValue + "\n";
            if (showPossiblePositions && verbosePossiblePositions) possibleString += "    Value accounting for distance: " + obsValue + "\n";
            // obsValue *= distWeight;
        }

        return obsValue;
    }

    public void ComputeValues()
    {
        Initialize();

        // Point givers
        Pin[] pins = FindObjectsOfType<Pin>();

        // Count total # of points available
        for (int i = 0; i < pins.Length; i++)
        {
            totalPoints += pins[i].PointValue;
        }

        // Cache pin values
        /* Not sure if I should include clustered points here... */
        for (int i = 0; i < pins.Length; i++)
        {
            _valueCache[pins[i].gameObject] = pointWeight * pins[i].PointValue / totalPoints;
        }

        // Damaging obstacles
        billboardMovement[] billboards = FindObjectsOfType<billboardMovement>();
        breakableObstacle[] barriers = FindObjectsOfType<breakableObstacle>();
        player = FindObjectOfType<PlayerParent>().GetComponentInChildren<PlayerMovement>(true);

        for (int i = 0; i < billboards.Length; i++)
        {
            _valueCache[billboards[i].gameObject] = -1f * fuelWeight * billboards[i].fuelSub;
        }

        for (int i = 0; i < barriers.Length; i++)
        {
            _valueCache[barriers[i].gameObject] = -1f * fuelWeight * barriers[i].fuelSub;
        }

        _valueCache[player.gameObject] = -1f * playerFuelLoss * fuelWeight;

        // Speed givers
        SpeedPlane[] boosters = FindObjectsOfType<SpeedPlane>();

        // Find total speed
        for (int i = 0; i < boosters.Length; i++)
        {
            // ds = ||dv|| = ||Fdt/m||

            /* Double check this equation */
            float value = boosters[i].speedMultiplier / _myBody.mass;

            totalSpeed += value;
        }

        // Cache speed values
        for (int i = 0; i < boosters.Length; i++)
        {
            _valueCache[boosters[i].gameObject] = speedWeight * boosters[i].speedMultiplier / (_myBody.mass * totalSpeed);
        }

        // Control limiters
        icyField[] oil = FindObjectsOfType<icyField>();
        Mine[] mines = FindObjectsOfType<Mine>();

        for (int i = 0; i < oil.Length; i++)
        {
            _valueCache[oil[i].gameObject] = controlWeight * -0.2f;
        }

        for (int i = 0; i < mines.Length; i++)
        {
            _valueCache[mines[i].gameObject] = controlWeight * -0.5f;
        }
    }

    // Turn, then straighten out
    private IEnumerator TurnSequence(float dir)
    {
        // _turning = true;
        
        if (_straightenRoutine != null)
        {
            StopCoroutine(_straightenRoutine);
        }
        
        float initialDiffSign = Mathf.Sign(_targetPos.x - transform.position.x);
        
        /* This will need to be refactored to account for turns */
        // Keep turning until you're at the new position
        float posDiff = _targetPos.x - transform.position.x;
        while (Mathf.Abs(posDiff) > Mathf.Epsilon &&
            Mathf.Abs(Mathf.Sign(posDiff) + initialDiffSign) > Mathf.Epsilon)
        {
            yield return new WaitForFixedUpdate();
            
            Turn(dir * turnForce);
            
            posDiff = _targetPos.x - transform.position.x;
        }
        
        // Straighten out once you're done turning
        _straightenRoutine = StartCoroutine(Straighten());
        
        // LinForceToVelocity(ForwardLinVelocity());
        // RotForceToVelocity(ForwardRotVelocity());
    }
    
    // Adjust velocity to forward
    private IEnumerator Straighten()
    {
        Vector3 upVelocity = Vector3.Project(_myBody.velocity, (_refInvRot * rotationRef.up).normalized);

        Vector3 forwardVelocity = Vector3.Project(_myBody.velocity, (_refInvRot * rotationRef.forward).normalized);

        Vector3 rightVelocity = Vector3.Project(_myBody.velocity, (_refInvRot * rotationRef.right).normalized);

        // Avoid dividing by zero!
        if (straightenSeconds < Mathf.Epsilon)
        {
            rightVelocity = Vector3.zero;
        }
        else
        {
            for (float t = 0f; t <= straightenSeconds; t += Time.fixedDeltaTime)
            {
                rightVelocity = Vector3.Lerp(rightVelocity, Vector3.zero, t / straightenSeconds);

                //Vector3 forwardVelocity = Vector3.Project(_myBody.velocity, (_refInvRot * rotationRef.up).normalized);

                //_myBody.velocity = Vector3.Lerp(_myBody.velocity, forwardVelocity, t / straightenSeconds);

                yield return new WaitForFixedUpdate();
            }
        }

        _myBody.velocity = upVelocity + forwardVelocity + rightVelocity;
    }

    private IEnumerator GutterStraighten(float straightenSeconds)
    {
        ResetParentForward(Vector3.forward);

        StartCoroutine(Straighten());

        yield break;
        
        //Vector3 forwardVelocity = (_refInvRot * rotationRef.forward).normalized;

        //float magnitude = _lastVelocity.magnitude;

        //forwardVelocity *= magnitude;

        //// Avoid dividing by zero!
        //if (straightenSeconds < Mathf.Epsilon)
        //{
        //    _myBody.velocity = forwardVelocity;

        //    yield break;
        //}

        //for (float t = 0f; t <= straightenSeconds; t += Time.fixedDeltaTime)
        //{
        //    _myBody.velocity = Vector3.Lerp(_myBody.velocity, forwardVelocity, t / straightenSeconds);

        //    yield return new WaitForFixedUpdate();
        //}
    }

    //private void HoldVelocity()
    //{
    //    _myBody.velocity = _lastVelocity;
    //}

    // Temporarily relieve the ball of control
    public void Stun(float seconds)
    {
        StartCoroutine(StunRoutine(seconds));
    }

    private IEnumerator StunRoutine(float seconds)
    {
        HaltMovement();

        yield return new WaitForSeconds(seconds);

        StartMovement();
    }

    // Halt all control
    private void HaltMovement()
    {
        if (_moving)
        {
            _moving = false;
        
            StopAllCoroutines();
        }
    }

    // Resume or begin control
    private void StartMovement()
    {
        if (!_moving)
        {
            if (aiEnabled)
            {
                _moving = true;

                _targetPos = transform.position;
                _targetScore = Mathf.NegativeInfinity;

                StartCoroutine(CheckPositions());

            }
        }
    }

    // Reset forward of parent
    public void ResetParentForward(Vector3 newForward)
    {
        _myParentScript.ResetForward(newForward);
    }
    
    #endregion
    
    #region Helper Functions

    // Calculate a future position based on time and constant acceleration
    private Vector3 PredictPosition(float time, in Vector3 turnAccel, in Vector3 initialPos)
    {
        // p = 0.5a(t^2) + v0*t + p0
        
        Vector3 pos = (turnAccel * (0.5f * Mathf.Pow(time, 2))) + (ForwardLinVelocity() * time) + (initialPos);

        return pos;
    }
    
    // Calculate a future position based on time and no acceleration
    private Vector3 PredictPosition(float time, in Vector3 initialPos)
    {
        // p = 0.5a(t^2) + v0*t + p0
        
        Vector3 pos = (ForwardLinVelocity() * time) + initialPos;

        return pos;
    }
    
    // Returns the forward linear velocity
    private Vector3 ForwardLinVelocity()
    {
        return Vector3.Project(_myBody.velocity,
            (_refInvRot * rotationRef.forward).normalized);
    }
    
    // Returns the forward rotational velocity
    private Vector3 ForwardRotVelocity()
    {
        return Vector3.Project(_myBody.angularVelocity,
            (_refInvRot * rotationRef.right).normalized);
    }
    
    // Deplete the fuel meter by some percent (between 0 and 1)
    public void ReduceFuel(float fuelPercent)
    {
        if (enableFuelLoss)
        {
            _fuelMeter -= Mathf.Clamp(fuelPercent, 0f, 1f);

            _fuelMeter = Mathf.Clamp(_fuelMeter, 0f, 1f);
        }
    }
    
    // Increase the fuel meter by some percent (between 0 and 1)
    public void RestoreFuel(float fuelPercent)
    {
        _fuelMeter += Mathf.Clamp(fuelPercent, 0f, 1f);
        
        _fuelMeter = Mathf.Clamp(_fuelMeter, 0f, 1f);
    }

    // Calculate the inverse squared distance from one position to another
    private float InvSquareDistance(Vector3 from, Vector3 to, float shift = 0f)
    {
        return Mathf.Pow(Vector3.Distance(from, to) + shift, -2);
    }
    
    // Calculate the inverse distance from one position to another
    private float InvDistance(Vector3 from, Vector3 to)
    {
        return Mathf.Pow(Vector3.Distance(from, to), -1);
    }

    private float InvXDistance(Vector3 from, Vector3 to, float scalar = 1f, float power = 1)
    {
        if (scalar < Mathf.Epsilon) scalar = 0.00001f;

        return Mathf.Pow(Mathf.Abs(from.x - to.x) * scalar, -power);
    }
    
    // Calculate the inverse squared distance from one position to a bounding box
    private float InvSquareDistanceBounds(Vector3 from, Bounds to, float shift = 0f)
    {
        float distance = Mathf.Pow(to.SqrDistance(from), 0.5f);
        return Mathf.Pow(distance + shift, -2);
    }
    
    // Calculate the inverse distance from one position to a bounding box
    /* This is inefficient and should be rewritten with a custom bound parsing method */
    private float InvDistanceBounds(Vector3 from, Bounds to, float power = 1f)
    {
        float distance = Mathf.Pow(to.SqrDistance(from), -0.5f * power);

        return distance;
    }
    
    // Applies the impulse needed for a desired linear velocity change
    private void LinForceToVelocity(Vector3 newVelocity)
    {
        // Add linear force to match change in parent linear acceleration
        // F = ma, F = mdv/dt
        // Fdt = mdv
        Vector3 velocityDiff = newVelocity - _myBody.velocity;
        _myBody.AddForce(_myBody.mass * velocityDiff, ForceMode.Impulse);
    }
    
    // Applies the impulse needed for a desired rotational velocity change
    private void RotForceToVelocity(Vector3 newVelocity)
    {
        // Add rotational force to match change in parent linear acceleration
        // T = Ia, T = Idv/dt
        // Tdt = Idv
        Vector3 velocityDiff = newVelocity - _myBody.velocity;
        // Debug.Log("PinCollider: Angular Velocity Diff is " + velocityDiff.magnitude);
        Vector3 inertia = _myBody.inertiaTensorRotation * _myBody.inertiaTensor;
        Vector3 Idv = MatVecProduct(inertia, velocityDiff);
        _myBody.AddTorque(Idv, ForceMode.Impulse);
    }
    
    // Treats 'diagonal' as the diagonal values of a diagonal 3x3 matrix
    private Vector3 MatVecProduct(Vector3 diagonal, Vector3 vec)
    {
        return new Vector3(diagonal.x * vec.x, diagonal.y * vec.y, diagonal.z * vec.z);
    }

    public void LockedToGround(bool locked)
    {
        if (locked)
        {
            _myBody.constraints = RigidbodyConstraints.FreezePositionY;
        }
        else
        {
            _myBody.constraints = RigidbodyConstraints.None;
        }

        _flying = !locked;
    }

    //public void Explode(float explosionForce)
    //{
    //    if (!_flying)
    //    {
    //        LockedToGround(false);

    //        float rightScalar = UnityEngine.Random.Range(explosionRadius * -1f, explosionRadius);

    //        Vector3 camRight = (_refInvRot * rotationRef.right).normalized;

    //        Vector3 offset = camRight * rightScalar;

    //        Vector3 explosionPos = transform.position + offset;

    //        Debug.Log("Enemy right scalar is " + rightScalar);

    //        _myBody.AddExplosionForce(explosionForce, explosionPos, 0f, explosionUpwards, ForceMode.Impulse);

    //    }
    //}

    public void Explode(float explosionForce)
    {
        if (!_flying)
        {
            LockedToGround(false);

            Vector3 camUp = (_refInvRot * rotationRef.up).normalized;
            Vector3 camRight = (_refInvRot * rotationRef.right).normalized;

            // Rotate around local x-axis
            float vertAngle = Mathf.Lerp(90f, 0f, explosionVerticality);
            Quaternion vertRotation = Quaternion.AngleAxis(vertAngle, camRight);

            // Rotate around local y-axis
            float halfFOV = explosionFOV / 2f;
            float horAngle = UnityEngine.Random.Range(halfFOV * -1, halfFOV);
            Debug.Log("Explosion angle: " + horAngle);
            Quaternion horRotation = Quaternion.AngleAxis(horAngle, camUp);

            camUp = horRotation * (vertRotation * camUp);

            // Apply force
            /* Assumes normalized camUp */
            _myBody.AddForce(camUp * explosionForce, ForceMode.Impulse);
        }
    }

    // Add extra gravity to land faster
    private void ExtraGravity()
    {
        if (_flying)
        {
            _myBody.AddForce(Vector3.up * extraGravity, ForceMode.Impulse);
        }
    }

    public void Embiggen(float multiplier)
    {
        transform.localScale *= multiplier;
    }

    public void NewThrow()
    {
        if (showActualPositions)
        {
            _posCount = 0;
            enemyPattern.ClearPositions();
        }
    }

    #endregion
}
