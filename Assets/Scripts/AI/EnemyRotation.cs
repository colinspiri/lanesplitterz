using UnityEngine;

public class EnemyRotation : MonoBehaviour
{
    public void ResetForward(Vector3 newForward)
    {
        Vector3 xzForward = transform.forward;
        xzForward.y = 0f;
        newForward.y = 0f;
        
        float angleDiff = Vector3.SignedAngle(xzForward, newForward, Vector3.up);
        Quaternion diffRot = Quaternion.AngleAxis(angleDiff, Vector3.up);
        transform.rotation *= diffRot;
    }
}
