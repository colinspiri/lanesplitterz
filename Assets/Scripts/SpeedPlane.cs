using UnityEngine;

public class SpeedPlane : MonoBehaviour
{
    public float speedMultiplier = 2f;
    // private Rigidbody _rigid;
    [SerializeField] private PlayerMovement _playerMove;
    [SerializeField] private AudioSource _audioSource;
 
    // private void Start()
    // {
        // Doesn't work when ball is disabled at start of load
        // _playerMove = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    // }

    private void OnTriggerEnter( Collider collision )
    {
        // _rigid.AddForce(Vector3.forward * speedMultiplier, ForceMode.Impulse);
        _playerMove.Accelerate(speedMultiplier);
        _audioSource.Play();

        Debug.Log("BOOOOOOOOST");
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
