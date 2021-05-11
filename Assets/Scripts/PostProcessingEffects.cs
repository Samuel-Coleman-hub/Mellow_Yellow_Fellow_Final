using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingEffects : MonoBehaviour
{
    [SerializeField] PostProcessProfile profile;

    private ChromaticAberration chromatic;

    // Start is called before the first frame update
    void Start()
    {
        profile.TryGetSettings(out chromatic);

        chromatic.intensity.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
