using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private GameManager gameManager;
    private AudioSource backgroundMusic;
    public AudioClip openWorldMusic;

    [Header("Special Effects")]
    public AudioSource specialEffectAudio;
    public AudioClip levelUpEffect;

    private void Awake()
    {
        backgroundMusic = GetComponent<AudioSource>();
        gameManager = GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        specialEffectAudio = gameObject.AddComponent<AudioSource>();
    }

    public void ChangeBackgroundMusic(AudioClip audio)
    {
        if (audio == null)
        {
            backgroundMusic.clip = openWorldMusic;
        }
        else
        {
            backgroundMusic.clip = audio;
        }
    }
}
