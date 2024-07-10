using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField]
    List<AudioClip> m_sounds;

    protected AudioSource m_audio;

    private void Start()
    {
        m_audio = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        m_audio.PlayOneShot(clip);
    }

    public void PlaySoundLoop(AudioClip clip)
    {
        m_audio.loop = true;
        if(!m_audio.isPlaying&&m_audio.clip != clip)
        {
            m_audio.Stop();
            m_audio.clip = clip;
            m_audio.Play();
        }
    }

    public void PlaySound(string sound)
    {
        m_audio.PlayOneShot(m_sounds.Find(s =>s.name == sound));
    }
}
