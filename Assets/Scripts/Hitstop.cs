using System.Collections;
using UnityEngine;

public class Hitstop : MonoBehaviour
{
    public static Hitstop Instance;

    [SerializeField] private float hitstopTimeScale = 0f;
    [SerializeField] private float defaultHitstopTime = 1f;
    [SerializeField] private float hitstopShakeStrength;

    private void Awake()
    {
        Instance = this;
    }

    public void DoHitstop(float hitstopTime = 0f)
    {
        CameraShake.Instance.Shake(hitstopShakeStrength);
        StartCoroutine(HitstopCoroutine(hitstopTime));
    }

    private IEnumerator HitstopCoroutine(float hitstopTime = 0f)
    {
        if (hitstopTime == 0) hitstopTime = defaultHitstopTime;
        
        TimeManager.Instance.SetTimeScale(hitstopTimeScale);

        var timer = 0f;
        while (timer < hitstopTime) {
            // do not countdown hitstop if game is stopped for some reason
            if(!GameManager.Instance.GamePaused && !TimeManager.Instance.pausedForTutorial) timer += Time.unscaledDeltaTime;
            yield return null;
        }
        
        TimeManager.Instance.ResumeTime();
    }
}