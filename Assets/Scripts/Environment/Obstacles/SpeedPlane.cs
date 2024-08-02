using UnityEngine;

public class SpeedPlane : MonoBehaviour
{
    public float speedMultiplier = 2f;
    public bool updatePlayerForward = true;
    public bool updateCameraForward = true;
    // private Rigidbody _rigid;
    private PlayerMovement _playerMove;
    private AudioSource _audioSource;
    private CameraFollowObject _playerCam;
 
    private void Start()
    {
        _playerMove = PlayerMovement.Instance;

        if (!_playerMove) Debug.LogWarning("SpeedPlane Warning: No PlayerMovement found");

        _audioSource = GetComponent<AudioSource>();
        
        _playerCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<CameraFollowObject>();
    }

    private void OnTriggerEnter( Collider collision )
    {
        // _rigid.AddForce(Vector3.forward * speedMultiplier, ForceMode.Impulse);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Balls"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Slowdown should be a separate object / script, adding this if statement for now
            
                // Speed up
                if (speedMultiplier > Mathf.Epsilon)
                {
                    if (updatePlayerForward)
                    {
                        // I think the forward is pointing backwards by default?
                        _playerMove.Accelerate(speedMultiplier * transform.forward * -1f, false);
                    }
                    else
                    {
                        _playerMove.Accelerate(speedMultiplier, false);
                    }

                    if (updateCameraForward)
                    {
                        _playerCam.ResetForward(transform.forward * -1f);
                    }
                }
                // Slow down
                else
                {
                    _playerMove.Accelerate(speedMultiplier, false);
                }
            }
            else if (collision.gameObject.CompareTag("Enemy Ball"))
            {
                EnemyBall enemyBall = collision.gameObject.GetComponent<EnemyBall>();
                
                // enemyBall.Stun(1f);
                
                // Accelerate and reset ball forward
                if (speedMultiplier > Mathf.Epsilon && updatePlayerForward)
                {
                    // I think the forward is pointing backwards by default?
                    enemyBall.Accelerate(speedMultiplier * transform.forward * -1f, false);
                    enemyBall.ResetParentForward(transform.forward * -1f);
                }
                // Accelerate or decelerate
                else
                {
                    enemyBall.Accelerate(speedMultiplier, false);
                }
            }

            
            if (_audioSource) _audioSource.Play();
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
