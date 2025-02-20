using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{   
    public float minPitch = 0.1f;
    private float pitchFromPlane;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = minPitch;
        
    }

    // Update is called once per frame
    void Update()
    {
        pitchFromPlane = Input.GetAxis("Acceleration") * 0.6f + minPitch;
        Debug.Log("Pitch from Plane: " + pitchFromPlane + " minPitch: " + minPitch);
        if(pitchFromPlane < minPitch)
            audioSource.pitch = minPitch;
        else
            audioSource.pitch = pitchFromPlane;
    }
}
