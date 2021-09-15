using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class InTownTrigger : MonoBehaviour
{
    private AudioManager audioManager;
    public AudioClip triggerClip;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        audioManager.ChangeBackgroundMusic(triggerClip);
    }

    private void OnTriggerExit(Collider other)
    {
        audioManager.ChangeBackgroundMusic(null);
    }
}
