using System.Collections; using System.Collections.Generic; using UnityEngine;  public class AudioScript : MonoBehaviour {     public AudioClip[] audioclip;     AudioSource audio;     // Start is called before the first frame update     void Start()     {         audio = GetComponent<AudioSource>();
     }

    // Update is called once per frame
    public void playSound (int clipId)     {
        audio.PlayOneShot(audioclip[clipId], 0.7F);

    }  } 