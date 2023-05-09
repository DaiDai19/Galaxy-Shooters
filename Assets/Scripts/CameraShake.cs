using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [SerializeField] float duration;
    [SerializeField] float magnitude;

    private bool infiniteShake;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());

        IEnumerator Shake()
        {
            Vector3 originalPos = transform.localPosition;

            float elapsedTime = 0;
            duration = 0.1f;

            while (elapsedTime < duration)
            {
                float x = Random.Range(-0.25f, 0.25f) * magnitude;
                float y = Random.Range(-0.25f, 0.25f) * magnitude;

                transform.localPosition = new Vector3(x, y, originalPos.z);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }

    public void CameraShakeFinish()
    {
        StartCoroutine(FinishShake());

        IEnumerator FinishShake()
        {
            Vector3 originalPos = transform.localPosition;

            float elapsedTime = 0;
            duration = 6f;

            while (elapsedTime < duration)
            {
                float x = Random.Range(-0.25f, 0.25f) * magnitude;
                float y = Random.Range(-0.25f, 0.25f) * magnitude;

                transform.localPosition = new Vector3(x, y, originalPos.z);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }
}
