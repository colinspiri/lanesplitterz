using UnityEngine;

public class SpeedPlane : MonoBehaviour
{
    public float speedMultiplier = 2f;
    // private Rigidbody _rigid;
    private PlayerMovement _playerMove;
    private AudioSource _audioSource;
 
    private void Awake()
    {
        _playerMove = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();

        if (!_playerMove) Debug.LogError("SpeedPlane Error: No PlayerMovement found");

        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter( Collider collision )
    {
        // _rigid.AddForce(Vector3.forward * speedMultiplier, ForceMode.Impulse);
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerMove.Accelerate(speedMultiplier);
            _audioSource.Play();
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
