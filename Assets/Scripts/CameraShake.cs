using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [SerializeField] float duration;
    [SerializeField] float magnitude;

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
    }

    private IEnumerator Shake()
    {
        Vector3 originalPos = transform.localPosition;

        float elapsedTime = 0;

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
