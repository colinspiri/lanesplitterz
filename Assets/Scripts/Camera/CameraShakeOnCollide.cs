using UnityEngine;

public class CameraShakeOnCollide : ActionOnCollide {
    [SerializeField] private float shakeMagnitude = 0;
    
    protected override void DoAction() {
        CameraShake.Instance.Shake(shakeMagnitude);
    }
}