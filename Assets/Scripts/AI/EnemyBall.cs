using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float fuelWeight;
    [SerializeField] private float speedWeight;
    [SerializeField] private float pointWeight;
    // The position being moved to
    private Vector3 _targetPos;
    // A dictionary caching object values
    private Dictionary<GameObject, float> _valueCache;

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
    private bool _turning;
    private float _fuelMeter = 1f;
    private float _currentSpin = 0f;
    private GameObject _ground = null;

    #endregion

    #region Start Caches

    private Rigidbody _myBody;
    private Quaternion _parentInvRot;
    private Transform _myParent;
    private Bounds _laneBounds;
    private int _pinLayer;
    private int _obstacleLayer;
    private SphereCollider _myCollider;
    private float _myRadius;
    private Coroutine _checkPositions = null;

    #endregion
    
    #region Debugging

    [SerializeField] private bool debug = false;
    [SerializeField] private EnemyPattern enemyPattern;
    [SerializeField] private GameObject gizmoObj;
    
    #endregion

    #region MonoBehaviour Event Functions

    // misc
    private int _groundMask;

    private void Start()
    {
        _myParent = transform.parent;
        _myBody = GetComponent<Rigidbody>();
        
        _laneBounds = GameObject.FindWithTag("Lane Bounds").GetComponent<Collider>().bounds;

        _pinLayer = LayerMask.NameToLayer("Pins");
        _obstacleLayer = LayerMask.NameToLayer("Obstacles");

        _valueCache = new();

        _myCollider = GetComponent<SphereCollider>();
        _myCollider.hasModifiableContacts = true;

        _myRadius = _myCollider.radius * transform.lossyScale.x;

        _groundMask = LayerMask.GetMask("Ground");

        StartCoroutine(CheckPositions());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (debug) enemyPattern.Instantiate();
    }

    public void EnableBall()
    {
        gameObject.SetActive(true);
        _checkPositions = StartCoroutine(CheckPositions());
        if (debug) enemyPattern.Instantiate();
    }

    private void FixedUpdate()
    {
        Quaternion parentRotation = _myParent.rotation;
        _parentInvRot = Quaternion.Inverse(new Quaternion(parentRotation.x, 0f, parentRotation.z, parentRotation.w));

        UpdateGround();
        Hook();
    }
    
    private void OnDrawGizmos()
    {
        if (debug)
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
    }
    
    #endregion
    
    #region Movement Functions
    
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


        _currentSpin += turnVal * turnSpeedPerSecond * Time.deltaTime;
        if (_currentSpin > 100) _currentSpin = 100;
        else if (_currentSpin < -100) _currentSpin = -100;

        Vector3 linForce = (_parentInvRot * _myParent.right) * turnVal;
        Vector3 rotForce = (_parentInvRot * _myParent.up) * turnVal;

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
        Vector3 hookForce = (_parentInvRot * _myParent.right) * hookForceMagnitude;

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

        Vector3 parentForward = (_parentInvRot * _myParent.forward).normalized;
        Vector3 parentRight = (_parentInvRot * _myParent.right).normalized;

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

        Vector3 camUp = (_parentInvRot * _myParent.up).normalized;
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

        if (Physics.Raycast(transform.position, (_parentInvRot * _myParent.up) * -1f, out hit,
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

    #endregion

    // Consider other possible positions to move towards
    private IEnumerator CheckPositions()
    {
        while (true)
        {
            yield return new WaitForSeconds(aiUpdateTime);
            
            // Don't check positions while turning
            // if (_turning) continue;
            
            // Box cast to find objects ahead
            Collider[] visibleObstacles = Physics.OverlapBox(transform.position,
                new Vector3(50f, 50f, visibleLimit), _myParent.rotation, visibleLayers);
            
            float bestValue = Mathf.NegativeInfinity;
            Vector3 bestPos = transform.position;
            float bestDir = 0f;
            float bestTime = 0f;

            // Measure different positions across different time steps (1.5 seconds left to 1.5 seconds right)
            for (float i = -1f * secondsToConsider; i < secondsToConsider + 0.1f; i += (secondsToConsider / considerTimeSteps))
            {
                float time = Mathf.Abs(i);
                float dir = Mathf.Sign(i);
                
                
                // Calculate predicted position at time

                // a = Fdt / (m * dt)
                Vector3 impulse = _parentInvRot * _myParent.right * (turnForce * dir);
                Vector3 accel = impulse / (_myBody.mass * Time.fixedDeltaTime);
                
                Vector3 predictedPos = PredictPosition(time, in accel);

                // Make sure new position isn't out-of-bounds
                
                // Bounds posBounds = new Bounds(predictedPos, _myBounds.size);
                
                // If out of bounds, skip to next position
                if (predictedPos.x > _laneBounds.max.x ||
                    predictedPos.z > _laneBounds.max.z ||
                    predictedPos.x < _laneBounds.min.x ||
                    predictedPos.z < _laneBounds.min.z)
                {
                    continue;
                }

                // Quality of the position (more positive is better)
                float posValue = 0f;
                // Expected fuel loss to reach position
                float expectedFuelLoss = turnFuel * time;

                // Deduct expected fuel cost from quality
                posValue -= expectedFuelLoss * fuelWeight;
                
                // Evaluate all visible obstacles for this position
                for (int j = 0; j < visibleObstacles.Length; j++)
                {
                    GameObject obstacle = visibleObstacles[j].gameObject;

                    // Ignore obstacles behind position
                    if (obstacle.transform.position.z < predictedPos.z) continue;

                    // The value of the obstacle, positive meaning it benefits you, negative meaning it hurts
                    float obsValue;
                    
                    /* We could precompute all obstacle values when the level loads as an alternative */

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
                                obsValue = pointWeight * obstacle.GetComponent<Pin>().PointValue;
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
                                    Debug.LogError("Enemy error: Invalid damaging obstacle " + obstacle.name + " found");
                                }
                            }
                        }

                        _valueCache.Add(obstacle, obsValue);
                    }
                    
                    // Scale obstacle value by inverse squared distance from it to position
                    /* Might want to put an upper bound on the distance scalar so it can't get too large */
                    posValue += obsValue * InvSquareDistance(predictedPos, obstacle.transform.position);
                }

                // Update best positions if better position is found
                if (posValue > bestValue)
                {
                    bestValue = posValue;
                    bestPos = predictedPos;
                    bestDir = dir;
                    bestTime = time;
                }
            }

            // If best position is elsewhere, move to it
            if (bestTime > Mathf.Epsilon)
            {
                _targetPos = bestPos;
                enemyPattern.AddPosition(bestPos, gizmoObj);
                // Debug.Log("Moving to position " + bestPos);
                _turning = true;
                StartCoroutine(TurnSequence(bestDir));
            }
        }
    }

    // Turn, then straighten out
    private IEnumerator TurnSequence(float dir)
    {
        float initialDiffSign = Mathf.Sign(_targetPos.x - transform.position.x);
        
        /* This will need to be refactored to account for turns */
        // Keep turning until you're at the new position
        while (Mathf.Sign(_targetPos.x - transform.position.x) - initialDiffSign < Mathf.Epsilon)
        {
            yield return new WaitForFixedUpdate();
            
            Turn(dir * turnForce);
        }

        Vector3 lastForwardVelocity = Vector3.Project(_myBody.velocity,
            (_parentInvRot * _myParent.forward).normalized);
        
        // Straighten out once you're done turning
        StartCoroutine(Straighten(lastForwardVelocity));
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

        _turning = false;
    }
    
    #region Helper Functions

    // Calculate a future position based on time and constant acceleration
    private Vector3 PredictPosition(float time, in Vector3 turnAccel)
    {
        // p = 0.5a(t^2) + v0*t + p0
        
        Vector3 pos = (turnAccel * (0.5f * Mathf.Pow(time, 2))) + (_myBody.velocity * time) + (transform.position);

        return pos;
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
    
    #endregion
}
