using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour {
    public static CameraShake Instance;

    public KeyCode cameraShakeTestKey;

    private const float Duration = 0.25f;
    [SerializeField] private float defaultMagnitude = 0.4f;

    private Coroutine _shakeCoroutine;
    private float _currentMagnitude;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cameraShakeTestKey)) {
            Shake(defaultMagnitude);
        }
    }

    public void Shake(float magnitude = -1) {
        if (magnitude == -1) magnitude = defaultMagnitude;

        // if already shaking, reset shake with the greater magnitude
        if (_shakeCoroutine != null) {
            StopCoroutine(_shakeCoroutine);
            magnitude = Mathf.Max(_currentMagnitude, magnitude);
        }
        _shakeCoroutine = StartCoroutine(ShakeCoroutine(magnitude));

        _currentMagnitude = magnitude;
    }

    private IEnumerator ShakeCoroutine(float magnitude) {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < Duration) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;
            
            transform.localPosition = new Vector3(x, y, z);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;

        _shakeCoroutine = null;
    }
}