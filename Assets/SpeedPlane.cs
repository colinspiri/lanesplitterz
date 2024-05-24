using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPlane : MonoBehaviour
{
    public float speedMultiplier = 2f;
    public GameObject ball;
    private BallMove speedOfBall;
    private Rigidbody rigid;
 
    private void Start()
    {
        rigid = ball.GetComponent<Rigidbody>();
    }

    void Update()
    {
        speedOfBall = ball.GetComponent<BallMove>();
    }

    private void OnTriggerEnter( Collider collision )
    {
        rigid.AddForce(Vector3.forward * speedMultiplier);

        Debug.Log("BOOOOOOOOST");
        Debug.Log("Speed of Ball: " + speedOfBall.speed.ToString("F2"));
    }


  //  private bool checkBall(){

 //       if (Physics.Raycast(transform.position, Vector3.up, 2) == true ){
          //  return true;
//} else{
   //         return false;
       // }
        
   // }
}
