using UnityEngine;

// Define a música de uma cena (menu, cada fase, vitória, derrota).
public class MusicTrack : MonoBehaviour
{
    [SerializeField] private AudioClip music;

    private void Start() => AudioManager.PlayMusic(music);
}
