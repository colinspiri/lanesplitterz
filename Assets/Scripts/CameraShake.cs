using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour {
    public static CameraShake Instance;

    public KeyCode cameraShakeTestKey;
    
    private float duration = 0.25f;
    [SerializeField] private float defaultMagnitude = 0.4f;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cameraShakeTestKey)) {
            StartCoroutine(ShakeCoroutine());
        }
    }

    public void Shake(float magnitude = 0) {
        StartCoroutine(ShakeCoroutine(magnitude));
    }

    private IEnumerator ShakeCoroutine(float magnitude = 0) {
        if (magnitude == 0) magnitude = defaultMagnitude;
        
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;
            
            transform.localPosition = new Vector3(x, y, z);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}