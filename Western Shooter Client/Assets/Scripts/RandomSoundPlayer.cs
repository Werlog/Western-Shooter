using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;


    private void Start()
    {
        if (sounds != null)
        {
            AudioSource source = GetComponent<AudioSource>();
            source.clip = sounds[Random.Range(0, sounds.Length)];
            source.Play();
        }
    }
}
