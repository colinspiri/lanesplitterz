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


    void Update()
    {
        speed = Vector3.Distance(oldPosition, transform.position) * 100f;
        oldPosition = transform.position;

        if (Input.GetAxis("Horizontal") > 0)
        {
            rigid.AddForce(Vector3.right * speed);
        }
        else if (Input.GetAxis("Horizontal") < 0 )
        {
            rigid.AddForce(-Vector3.right * speed);
        }

        Debug.Log("Speed: " + speed.ToString("F2"));
    }


}
