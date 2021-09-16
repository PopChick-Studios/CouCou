using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private GameManager gameManager;
    private AudioSource backgroundMusic;
    public AudioClip openWorldMusic;
    public AudioClip battleMusicIntro;
    public AudioClip battleMusicLoop;

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
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            backgroundMusic.clip = battleMusicIntro;
        }
        specialEffectAudio = gameObject.AddComponent<AudioSource>();
    }

    public IEnumerator LoopBattleMusic()
    {
        yield return new WaitForSeconds(5.8f);
        backgroundMusic.clip = battleMusicLoop;
        backgroundMusic.loop = true;
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
