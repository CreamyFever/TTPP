using UnityEngine;
using SingletonPattern;

public class CSoundManager : CSingletonPattern<CSoundManager>
{
    public AudioClip[] bgmSounds;
    public AudioClip[] efxSounds;

    AudioSource audioSrc;

    private void Awake()
    {
        audioSrc = GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            ChangeBgm(0);
        }
    }

    public void PlayEffectSound(int index)
    {
        audioSrc.PlayOneShot(efxSounds[index], 1.0f);
    }

    public void ChangeBgm(int index)
    {
        audioSrc.clip = bgmSounds[index];
        audioSrc.loop = true;
        audioSrc.Play();
    }

    public void StopBgm()
    {
        audioSrc.Stop();
    }
}