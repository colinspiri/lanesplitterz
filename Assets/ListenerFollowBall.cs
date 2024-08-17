using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ListenerFollowBall : MonoBehaviour
{

    [SerializeField] GameObject Ball;
    [SerializeField] GameObject Camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Ball)
        {
            /*Vector3 position = Camera.transform.position + (Ball.transform.position - Camera.transform.position)/2;
            this.transform.SetPositionAndRotation(position, Camera.transform.rotation);*/    
            this.transform.SetPositionAndRotation(Ball.transform.position, Ball.transform.rotation);    
        }
        else
        {
            this.transform.SetPositionAndRotation(Camera.transform.position, Camera.transform.rotation);
        }
    }
}
