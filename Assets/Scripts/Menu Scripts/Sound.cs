using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{

    AudioSource fxSound; // Emitir sons
    public AudioClip backMusic; // Som de fundo

                                // Use this for initialization
    void Start()
    {
        // Audio Source responsavel por emitir os sons
        fxSound = GetComponent<AudioSource>();
        fxSound.Play();
    }
}