using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource src;   
    public AudioSource scr2;
    public AudioSource Main;

    public AudioClip sfx1;    
    public AudioClip sfx2;
    public AudioClip backgroundmusic;

    void Start()
    {
        if (Main != null && backgroundmusic != null)
        {
            Main.clip = backgroundmusic;
            Main.loop = true;
            Main.Play();    
        }
    }

    public void Button1()
    {
        if (sfx1 != null && src != null)
        {
            src.PlayOneShot(sfx1);
        }
    }

    public void PlaySpinSound()
    {
        if (sfx2 != null && scr2 != null)
        {
            scr2.clip = sfx2;
            scr2.Play();
        }
    }

    public void StopSpinSound()
    {
        if (scr2 != null && scr2.isPlaying)
        {
            scr2.Stop();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (Main != null && backgroundmusic != null)
        {
            Main.clip = backgroundmusic;
            Main.Play();
        }
    }
}

