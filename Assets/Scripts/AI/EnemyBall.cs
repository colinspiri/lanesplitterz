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
    [Tooltip("Percent of fuel lost on hitting the player")]
    [SerializeField] private float playerFuelLoss;
    [SerializeField] private float fuelWeight;
    [SerializeField] private float speedWeight;
    [SerializeField] private float pointWeight;
    [SerializeField] private float distWeight;
    [SerializeField] private float angleWeight;
    // The position being moved to
    private Vector3 _targetPos;
    private float _targetScore;
    // A dictionary caching object values
    private Dictionary<GameObject, float> _valueCache;
    // The true update time for checking positions
    private float _posUpdateTime;

    [Header("Force specifications")] [SerializeField]
    private float turnForce;
    [SerializeField] private float hookForceMultiplier;
    [SerializeField] private float turnSpeedPerSecond = 0.1f;
    [SerializeField] private float slipperyForce = 10f;

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
    private float _fuelMeter = 1f;
    private float _currentSpin = 0f;
    private GameObject _ground = null;
    private Coroutine _turnRoutine = null;
    private Coroutine _straightenRoutine = null;

    #endregion

    #region Start Caches

    private Rigidbody _myBody;
    private Quaternion _refInvRot;
    [SerializeField] private Transform rotationRef;
    private Enemy _myParentScript;
    private Bounds _laneBounds;
    private int _pinLayer;
    private int _obstacleLayer;
    private int _ballLayer;
    private SphereCollider _myCollider;
    private float _myRadius;

    #endregion
    
    #region Debugging

    [SerializeField] private bool showActualPositions = false;
    [SerializeField] private bool showPossiblePositions = false;
    [SerializeField] private EnemyPattern enemyPattern;
    [SerializeField] private GameObject gizmoObj;
    private List<Vector3> _possiblePositions;
    private int _posCount = 0;
    
    #endregion

    #region MonoBehaviour Event Functions

    // misc
    private int _groundMask;

    private void Awake()
    {
        // Clear enemy pattern object regardless of whether debugging is being used
        enemyPattern.Instantiate();
    }

    private void Start()
    {
        _myBody = GetComponent<Rigidbody>();
        
        _laneBounds = GameObject.FindWithTag("Lane Bounds").GetComponent<Collider>().bounds;

        _pinLayer = LayerMask.NameToLayer("Pins");
        _obstacleLayer = LayerMask.NameToLayer("Obstacles");
        _ballLayer = LayerMask.NameToLayer("Balls");

        _valueCache = new();

        _myCollider = GetComponent<SphereCollider>();
        _myCollider.hasModifiableContacts = true;

        _myRadius = _myCollider.radius * transform.lossyScale.x;

        _groundMask = LayerMask.GetMask("Ground");

        _posUpdateTime = Mathf.Max(aiUpdateTime, Time.fixedDeltaTime);

        _myParentScript = transform.parent.GetComponent<Enemy>();

        if (showPossiblePositions) _possiblePositions = new();

        StartMovement();
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
        Quaternion parentRotation = rotationRef.rotation;
        _refInvRot = Quaternion.Inverse(new Quaternion(parentRotation.x, 0f, parentRotation.z, parentRotation.w));

        UpdateGround();
        // Hook();
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
            for (int i = 0; i < _possiblePositions.Count; i++)
            {
                Gizmos.DrawSphere(_possiblePositions[i], 1.5f);
            }
        }
    }

    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            ReduceFuel(playerFuelLoss);
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
    //     //     linForce /= slipperyForce;
    //     //     rotForce /= slipperyForce;
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
        //     linForce /= slipperyForce;
        //     rotForce /= slipperyForce;
        // }

        // Linear acceleration
        _myBody.AddForce(linForce, ForceMode.Impulse);

        // Rotational acceleration
        _myBody.AddTorque(rotForce, ForceMode.Impulse);
    }

    // Emulate frictional movement to the side
    private void Hook()
    {
        if (!Grounded() || IsIcy()) return;

        float hookForceMagnitude = _currentSpin * hookForceMultiplier;
        // I have absolutely no idea if what is in the parenthesis is correct (for player ball its _camInvRot * _myCam.right)
        Vector3 hookForce = (_refInvRot * rotationRef.right) * hookForceMagnitude;

        _myBody.AddForce(hookForce);

        /*Vector3 camUp = _camInvRot * _myCam.up; 
        float force = Vector3.Dot(_myBody.angularVelocity, camUp.normalized); 
        Turn(force * hookMultiplier, false);*/
    }

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

            linearForce /= slipperyForce;
            rotationalForce /= slipperyForce;
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

            linearForce /= slipperyForce;
            rotationalForce /= slipperyForce;
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
            // if (_turning) continue;
            
            // Box cast to find objects ahead
            Collider[] visibleObstacles = Physics.OverlapBox(transform.position,
                new Vector3(50f, 50f, visibleLimit), rotationRef.rotation, visibleLayers);
            
            float bestValue = Mathf.NegativeInfinity;
            Vector3 bestPos = Vector3.zero;
            float bestDir = 0f;
            // float bestTime = 0f;
            string bestString = null;
            float timeStep = secondsToConsider / considerTimeSteps;

            if (showPossiblePositions)
            {
                _possiblePositions.Clear();
                // Debug.Log("Drawing possible positions for position " + _posCount + 1 + ":");
            }

            // Project positions horizontally in time
            for (float i = -1f * secondsToConsider; i < secondsToConsider + 0.1f; i += timeStep)
            {
                float time = Mathf.Abs(i);
                // Negative going left, positive going right
                float dir = Mathf.Sign(i);
                
                // Project positions vertically in time
                for (float j = time; j < secondsToConsider + 0.1f; j += timeStep)
                {
                    float verticalTime = secondsToConsider - j;

                    // Calculate predicted horizontal position
                    // a = Fdt / (m * dt)
                    Vector3 impulse = _refInvRot * rotationRef.right * (turnForce * dir);
                    Vector3 accel = impulse / (_myBody.mass * Time.fixedDeltaTime);
                    Vector3 horizontalPos = PredictPosition(time, in accel, transform.position);
                    
                    // Calculate predicted final position
                    Vector3 predictedPos = PredictPosition(verticalTime, horizontalPos);
                    
                    // If out of bounds, skip to next position
                    if (predictedPos.x > _laneBounds.max.x ||
                        predictedPos.z > _laneBounds.max.z ||
                        predictedPos.x < _laneBounds.min.x ||
                        predictedPos.z < _laneBounds.min.z)
                    {
                        continue;
                    }
                    
                    if (showPossiblePositions) _possiblePositions.Add(predictedPos);

                    string predictedString = "";
                    if (showActualPositions) predictedString = "Predicted position " + _posCount + "\n";

                    // Quality of the position (more positive is better)
                    float posValue = 0f;
                    
                    // Expected fuel loss to reach position
                    float expectedFuelLoss = turnFuel * time;
                    // Deduct expected fuel cost from quality
                    posValue -= expectedFuelLoss * fuelWeight;

                    if (showActualPositions) predictedString += "Expected fuel loss: " + expectedFuelLoss + "\n";

                    // Evaluate all visible obstacles relative to this position
                    for (int k = 0; k < visibleObstacles.Length; k++)
                    {
                        GameObject obstacle = visibleObstacles[k].gameObject;
                        Bounds obsBounds = visibleObstacles[k].bounds;

                        // Ignore unreachable obstacles from position
                        // Doesn't ignore the player, since they can catch up
                        /* Assumes positive z is always forward */
                        if (obstacle.layer != _ballLayer &&
                            predictedPos.z - obstacle.transform.position.z > _myRadius) continue;

                        if (showActualPositions) predictedString += "Evaluating obstacle " + obstacle.name + ":\n";

                        // The value of the obstacle, positive meaning it benefits you, negative meaning it hurts
                        /* We could precompute all obstacle values when the level loads as an alternative */
                        float obsValue;

                        // If value isn't cached, calculate it for the first time
                        if (!_valueCache.TryGetValue(obstacle, out obsValue))
                        {
                            obsValue = 0f;

                            // Pin
                            if (obstacle.layer == _pinLayer)
                            {
                                Pin pin = obstacle.GetComponent<Pin>();
                                if (pin.pinState == Pin.PinState.Untouched)
                                {
                                    obsValue = pointWeight * pin.PointValue;
                                }
                                else
                                {
                                    obsValue = 0f;
                                }
                            }
                            // Obstacle
                            else if (obstacle.layer == _obstacleLayer)
                            {
                                if (obstacle.CompareTag("Speed Pad"))
                                {
                                    // ds = ||dv|| = ||Fdt/m||

                                    SpeedPlane speedPlane = obstacle.GetComponent<SpeedPlane>();

                                    float speedChange = speedPlane.speedMultiplier / _myBody.mass;

                                    obsValue = speedWeight * speedChange;
                                }
                                else if (obstacle.CompareTag("Damage Obstacle"))
                                {
                                    breakableObstacle breakableObs;
                                    billboardMovement billboardMove;
                                    if (obstacle.TryGetComponent(out breakableObs))
                                    {
                                        obsValue = -1f * fuelWeight * breakableObs.fuelSub;
                                    }
                                    else if (obstacle.TryGetComponent(out billboardMove))
                                    {
                                        obsValue = -1f * fuelWeight * billboardMove.fuelSub;
                                    }
                                    else
                                    {
                                        Debug.LogError("Enemy error: Invalid damaging obstacle " +
                                                       obstacle.name + " found");
                                    }
                                }
                            }
                            // Player ball
                            else if (obstacle.layer == _ballLayer)
                            {
                                obsValue = -1f * playerFuelLoss * fuelWeight;
                            }

                            _valueCache.Add(obstacle, obsValue);
                        }

                        if (showActualPositions) predictedString += "    Unscaled value: " + obsValue + "\n";

                        // Scale obstacle value by cos of angle between obstacle and position
                        // An angle of 90 degrees means it's unreachable and is worthless
                        // An angle of 0 degrees means it's straight-ahead and worth its full value
                        Vector3 posToObs = obstacle.transform.position - predictedPos;
                        posToObs.y = 0f;
                        posToObs = posToObs.normalized;

                        Vector3 parentForward = _refInvRot * rotationRef.forward;
                        parentForward.y = 0f;
                        parentForward = parentForward.normalized;

                        obsValue *= Mathf.Clamp(Vector3.Dot(posToObs, parentForward), 0f, Mathf.Infinity) * angleWeight;

                        if (showActualPositions) predictedString += "    Value accounting for angle: " + obsValue + "\n";

                        // Scale obstacle value by inverse distance from it to position

                        // Calculate inverse distance to obstacle
                        // Clamped to prevent value from ever exceeding 1
                        // Distance ceases to matter below 0.1 meters due to shift
                        // float invDist = InvDistance(predictedPos, obstacle.transform.position, 0.9f);
                        float invDist = InvDistanceBounds(predictedPos, obsBounds, 0.9f);
                        invDist = Mathf.Clamp(invDist, 0f, 1f);
                        obsValue *= invDist;
                        if (showActualPositions) predictedString += "    Value accounting for distance: " + obsValue + "\n";
                        obsValue *= distWeight;

                        posValue += obsValue;
                    }

                    if (showActualPositions) predictedString += "Value of position " + _posCount + ": " + posValue;

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
            }

            // Move to best position if it's better than current target or you've passed the current target
            /* Assumes positive z is forward */
            if (bestValue - _targetScore >= changePosDiff || transform.position.z > _targetPos.z)
            {
                _posCount++;
                _targetPos = bestPos;
                _targetScore = bestValue;
                if (showActualPositions) Debug.Log(bestString);
                enemyPattern.AddPosition(bestPos, gizmoObj);
                
                if (_turnRoutine != null)
                {
                    StopCoroutine(_turnRoutine);
                }
            
                _turnRoutine = StartCoroutine(TurnSequence(bestDir));
            }
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
        _straightenRoutine = StartCoroutine(Straighten(ForwardLinVelocity()));
        
        // LinForceToVelocity(ForwardLinVelocity());
        // RotForceToVelocity(ForwardRotVelocity());
    }
    
    // Adjust velocity to forward
    private IEnumerator Straighten(Vector3 forwardVelocity)
    {
        // Avoid dividing by zero!
        if (straightenSeconds < Mathf.Epsilon)
        {
            _myBody.velocity = forwardVelocity;

            yield break;
        }
        
        for (float t = 0f; t <= straightenSeconds; t += Time.fixedDeltaTime)
        {
            _myBody.velocity = Vector3.Lerp(_myBody.velocity, forwardVelocity, t / straightenSeconds);
            
            yield return new WaitForFixedUpdate();
        }
        
        // _turning = false;
    }
    
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
        StopAllCoroutines();
    }

    // Resume or begin control
    private void StartMovement()
    {
        _targetPos = transform.position;
        _targetScore = Mathf.NegativeInfinity;
        
        StartCoroutine(CheckPositions());
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
    private float InvSquareDistance(Vector3 from, Vector3 to)
    {
        return Mathf.Pow(Vector3.Distance(from, to), -2);
    }
    
    // Calculate the inverse distance from one position to another
    private float InvDistance(Vector3 from, Vector3 to, float shift = 0f)
    {
        return Mathf.Pow(Vector3.Distance(from, to) + shift, -1);
    }
    
    // Calculate the inverse squared distance from one position to a bounding box
    private float InvSquareDistanceBounds(Vector3 from, Bounds to)
    {
        return Mathf.Pow(to.SqrDistance(from), -1);
    }
    
    // Calculate the inverse distance from one position to a bounding box
    /* This is inefficient and should be rewritten with a custom bound parsing method */
    private float InvDistanceBounds(Vector3 from, Bounds to, float shift = 0f)
    {
        float distance = Mathf.Pow(to.SqrDistance(from), 0.5f);
        return Mathf.Pow(distance + shift, -1);
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
    
    #endregion
}
