using UnityEngine;

public class SpeedPlane : MonoBehaviour
{
    public float speedMultiplier = 2f;
    // private Rigidbody _rigid;
    private PlayerMovement _playerMove;
    private AudioSource _audioSource;
 
    private void Start()
    {
        _playerMove = PlayerMovement.Instance;

        if (!_playerMove) Debug.LogError("SpeedPlane Error: No PlayerMovement found");

        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter( Collider collision )
    {
        // _rigid.AddForce(Vector3.forward * speedMultiplier, ForceMode.Impulse);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Balls") && collision.gameObject.CompareTag("Player"))
        {
            // Slowdown should be a separate object / script, adding this if statement for now
            
            // Speed up
            if (speedMultiplier > Mathf.Epsilon)
            {
                // I think the forward is pointing backwards by default?
                _playerMove.Accelerate(speedMultiplier * transform.forward * -1f, false);
            }
            // Slow down
            else
            {
                _playerMove.Accelerate(speedMultiplier, false);
            }
            
            if (_audioSource) _audioSource.Play();
        }
        else if (collision.gameObject.CompareTag("Enemy Ball"))
        {
            // Code for enemy boosting here
        }


        // Debug.Log("BOOOOOOOOST");
        // Debug.Log("Speed of Ball: " + speedOfBall.speed.ToString("F2"));
    }


  //  private bool checkBall(){

 //       if (Physics.Raycast(transform.position, Vector3.up, 2) == true ){
          //  return true;
//} else{
   //         return false;
       // }
        
   // }
}
