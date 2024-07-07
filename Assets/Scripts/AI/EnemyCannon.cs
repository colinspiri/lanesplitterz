using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class EnemyCannon : MonoBehaviour
{
    [field: SerializeField] public float MinYaw{get; private set;}
    [field: SerializeField] public float MaxYaw{get; private set;}
    [field: SerializeField] public float MinPitch{get; private set;}
    [field: SerializeField] public float MaxPitch{get; private set;}
    [field: SerializeField] public float MaxLaunchForce{get; private set;}
    [field: SerializeField] public float MinLaunchForce{get; private set;}
    [field: SerializeField] public float DefaultLaunchForce{get; private set;}
    [field: SerializeField] public float DegreesPerSecond{get; private set;}
    
    [SerializeField] private Transform launchPoint;
    [SerializeField] private AudioSource launchSound;
    [SerializeField] private Rigidbody ball;

    public bool Rotating { get; private set; } = false;

    #region Cannon Movement
    
    public void Rotate(float x, float y)
    {
        if (!Rotating)
        {
            Rotating = true;
            
            float clampedX = Mathf.Clamp(RoundAngle(x) * -1, MinPitch, MaxPitch);
            float clampedY = Mathf.Clamp(RoundAngle(y), MinYaw, MaxYaw);
        
            StartCoroutine(RotateGradually(Quaternion.Euler(clampedX, clampedY, 0f)));
        }
    }
    
    // normalize angle to [-180,180]
    private float RoundAngle(float angle)
    {
        angle %= 360;
        return angle > 180 ? angle - 360 : angle < -180 ? angle + 360 : angle;
    }

    private IEnumerator RotateGradually(Quaternion goal)
    {
        goal = transform.rotation * goal;
        
        while (Quaternion.Angle(transform.rotation, goal) > Mathf.Epsilon)
        {
            yield return new WaitForEndOfFrame();
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, DegreesPerSecond * Time.deltaTime);
        }

        Rotating = false;
    }
    
    #endregion
    
    #region Ball Launch
    public void LaunchDefault()
    {
        Launch(DefaultLaunchForce);
    }
    
    public void LaunchInRange(float minForce, float maxForce)
    {
        Launch(Random.Range(minForce, maxForce));
    }
    public void Launch(float launchForce)
    {
        ball.gameObject.SetActive(true);
        ball.transform.position = launchPoint.position;
        ball.velocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        
        launchForce = Mathf.Clamp(launchForce, MinLaunchForce, MaxLaunchForce);
        ball.AddForce(launchForce * transform.forward, ForceMode.Impulse);

        launchSound.Play();
    }
    
    #endregion
}
