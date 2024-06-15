using UnityEngine;

public class ResetCameraOrientation : MonoBehaviour
{
    public bool isVisible = true;
    
    private CameraFollowObject _playerCam;
    private PlayerMovement _playerBall;
    
    private void Start()
    {
        _playerCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<CameraFollowObject>();
        _playerBall = PlayerMovement.Instance.GetComponent<PlayerMovement>();
        if (!isVisible) GetComponent<MeshRenderer>().enabled = false;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Balls") && collision.gameObject.CompareTag("Player"))
        {
            _playerBall.Steer(transform.forward);
            _playerCam.ResetForward(transform.forward);
        }
    }
}
