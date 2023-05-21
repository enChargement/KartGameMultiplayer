using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    Rigidbody rb;

    public float _gear1Divider = 4f;
    public float _gear2Divider = 8f;
    public float _speedMinGear2 = 7.5f;
    public float volumeGear1 = .2f;

    [SerializeField]
    AudioSource EngineSound;
    [SerializeField]
    AudioSource DriftAudio;
    [SerializeField]
    AudioSource TurboSound;

    CarManager carManager;

    int audioIndex = 0;

    [SerializeField]
    AudioClip idle;

    [SerializeField]
    AudioClip Gear1;

    [SerializeField]
    AudioClip Gear2;

    void Start()
    {
        carManager = GetComponent<CarManager>();
        rb = carManager.rb;
        EngineSound.clip = idle;
        EngineSound.Play();
    }


    void Update()
    {

        float speed = rb.velocity.magnitude;
        if(speed < 0.3)
        {
            if(audioIndex != 0)
            {
                EngineSound.pitch = 1;
                EngineSound.clip = idle;
                EngineSound.Play();
                audioIndex = 0;
            }
        }
        else if (0.3 <= speed && speed < _speedMinGear2)
        {
            if(audioIndex != 1)
            {
                EngineSound.clip = Gear1;
                EngineSound.Play();
                audioIndex = 1;
                EngineSound.volume = volumeGear1;
            }
            EngineSound.pitch = speed / _gear1Divider;
        }
        else if (_speedMinGear2 <= speed)
        {
            if (audioIndex != 2)
            {
                EngineSound.clip = Gear2;
                EngineSound.Play();
                audioIndex = 2;
                EngineSound.volume = .1f;
            }
            EngineSound.pitch = speed / _gear2Divider;
            if (EngineSound.pitch > 1.6) EngineSound.pitch = 1.6f;
        }

        if (carManager.IsDrifting())
        {
            if (!DriftAudio.isPlaying)
                DriftAudio.Play();
        }
        else
        {
            if (DriftAudio.isPlaying)
                DriftAudio.Stop();
        }

        if (carManager.IsTurbo())
        {
            if (!TurboSound.isPlaying)
                TurboSound.Play();
        }
        else
        {
            if (TurboSound.isPlaying)
                TurboSound.Stop();
        }
    }
}
