using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboardMovement : MonoBehaviour
{
    private PlayerMovement playerMove;
    public GameObject billboard;
    
    [SerializeField] private AudioSource destroySound;
    [SerializeField] public float moveDuration = 5;

    //This is the stupidest solution to this problem I could have found
    //MoveBoolean: If true, move right, if false, move left
    //SwitchBoolean: If true, switch MoveBoolean to false, if false, switch MoveBooelan to true
    private bool MoveBoolean = true;
    private bool SwitchBoolean = false;
    private bool playSound = true;

 
    [Range(0.01f, 1.0f)]
    public float fuelSub = 0.1f;

    private void Start()
    {
        playerMove = PlayerMovement.Instance;
    }

    private void Update()
    {
        if (MoveBoolean)
        {
            transform.Translate(Vector3.right * moveDuration * Time.deltaTime);
        }
        else 
        {
            transform.Translate(Vector3.left * moveDuration * Time.deltaTime);
        }
        
    }

    //Checks if the layer is a Balls or Ground. On collider hit, either slows down Ball by a float and deactivates the object, 
    //or reverses movement of the billboard 
    public void OnCollisionEnter( Collision billboardHit )
    {
        if (billboardHit.gameObject.layer == LayerMask.NameToLayer("Balls"))
        {
            billboard.gameObject.SetActive(false);
            
            

            playerMove.ReduceFuel(fuelSub);
            if (billboardHit.gameObject.CompareTag("Player"))
            {
                destroySound.spatialBlend = 0.0f;
            }
            else if (billboardHit.gameObject.CompareTag("Enemy Ball"))
            {
                destroySound.spatialBlend = 1.0f;
            }

            if (playSound)
            {
                destroySound.Play();
                playSound = false;
            }
        }

        if (billboardHit.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //this feels like an afront to god
            if(!SwitchBoolean)
            {
                MoveBoolean = false;
                SwitchBoolean = true;

            }
            else 
            {
                MoveBoolean = true;
                SwitchBoolean = false;

            }
            
            
        }

        //                Debug.Log("Triggered1");
    }

}
