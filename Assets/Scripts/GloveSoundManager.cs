using UnityEngine;
using System.Collections;

public class GloveSoundManager : MonoBehaviour {

    public AudioClip punch;
    private AudioSource punchSound;


    void Start()
    {
        punchSound = gameObject.AddComponent<AudioSource>();
        punchSound.clip = punch;


    }

    public void PlayPunchSound()
    {

        punchSound.Play();


    }

   

}
