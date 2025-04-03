using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    public float shakeIntensity = 0.3f;
    public float shakeDecay = 0.02f;
    public float maxShakeTime = 1.0f;

    private Vector3 originalPos;
    private float currentShakeTime = 0f;
    private bool isShaking = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        originalPos = transform.localPosition;
    }

    private void Update()
    {
        if (isShaking)
        {
            if (currentShakeTime > 0)
            {
                transform.localPosition = originalPos + Random.insideUnitSphere * shakeIntensity;
                currentShakeTime -= Time.deltaTime;
            }
            else
            {
                isShaking = false;
                transform.localPosition = originalPos;
            }
        }
    }

    public void ShakeCamera(float intensity = -1, float time = -1)
    {
        originalPos = transform.localPosition;
        isShaking = true;
        currentShakeTime = time > 0 ? time : maxShakeTime;
        shakeIntensity = intensity > 0 ? intensity : shakeIntensity;
    }

    public void StopShaking()
    {
        isShaking = false;
        transform.localPosition = originalPos;
    }
}
