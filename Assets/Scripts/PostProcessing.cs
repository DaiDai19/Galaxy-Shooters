using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessing : MonoBehaviour
{
    [SerializeField] PostProcessVolume postProcessVolume;
    [SerializeField] LensDistortion lensDistortion;
    [SerializeField] float changeSpeed = 10;

    float currentDistortion = 0;

    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out lensDistortion);
        postProcessVolume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100, lensDistortion);
    }

    // Update is called once per frame
    void Update()
    {
        lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, 0, changeSpeed * Time.deltaTime);
    }
}
