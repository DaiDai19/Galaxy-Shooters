using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] float duration;
    [SerializeField] float magnitude;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
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
