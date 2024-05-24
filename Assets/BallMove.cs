using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    private Rigidbody rigid;
    public float speed;
    Vector3 oldPosition;

    // Update is called once per frame
    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        // speed = Vector3.Distance(oldPosition, transform.position) * 100f;
        speed = rigid.velocity.magnitude;
        // oldPosition = transform.position;

        // if (Input.GetAxis("Horizontal") > Mathf.Epsilon)
        // {
        //     rigid.AddForce(Vector3.right * speed);
        // }
        // else if (Input.GetAxis("Horizontal") < (Mathf.Epsilon * -1))
        // {
        //     rigid.AddForce(-Vector3.right * speed);
        // }

        Debug.Log("Speed: " + speed.ToString("F2"));
    }


}
