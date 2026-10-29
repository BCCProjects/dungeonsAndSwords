using System;
using UnityEngine;

// Toca a música da cena e os efeitos sonoros do jogo. Fica vivo entre cenas (como o GameManager).
// Qualquer script chama AudioManager.Play(Sfx.Jump); se não houver AudioManager na cena, nada acontece.
public class AudioManager : MonoBehaviour
{
    public enum Sfx
    {
        Jump, Attack, Shoot, Hit, PlayerHurt, EnemyAttack, EnemyDeath,
        ItemPickup, LevelUp, LevelComplete, Victory, GameOver,
        BossCharge, BossBeam, BossSword, BossDeath
    }

    [Serializable]
    private class SfxEntry
    {
        public Sfx id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.8f;
    }

    [SerializeField] private SfxEntry[] effects;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.45f;

    private static AudioManager instance;
    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public static void Play(Sfx id)
    {
        if (instance == null) return;
        foreach (SfxEntry e in instance.effects)
            if (e.id == id && e.clip != null)
            {
                instance.sfxSource.PlayOneShot(e.clip, e.volume);
                return;
            }
    }

    // Chamado pelo MusicTrack de cada cena. A mesma música não reinicia ao trocar de cena.
    public static void PlayMusic(AudioClip clip)
    {
        if (instance == null || instance.musicSource.clip == clip) return;
        instance.musicSource.clip = clip;
        if (clip != null) instance.musicSource.Play();
        else instance.musicSource.Stop();
    }
}
