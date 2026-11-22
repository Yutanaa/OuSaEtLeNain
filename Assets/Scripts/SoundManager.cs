using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    #region SFX

    [Serializable]
    public class Sound
    {
        [Tooltip("Nom utilisé pour jouer ce son depuis un script.")]
        public string id;

        [Tooltip("Clip audio à jouer.")]
        public AudioClip clip;

        [Range(0f, 1f)]
        [Tooltip("Volume de ce son.")]
        public float volume = 1f;

        [Range(0.1f, 3f)]
        [Tooltip("Vitesse de lecture du son.")]
        public float pitch = 1f;
    }

    #endregion

    #region Settings

    [Header("SFX")]
    [Tooltip("Liste des musiques disponibles.")]
    [SerializeField] private List<Sound> musicSounds = new List<Sound>();
    [SerializeField] private List<Sound> sfxSounds = new List<Sound>();

    [Header("Audio Sources")]

    [Tooltip("Nombre de sons pouvant être joués simultanément.")]
    [SerializeField, Min(1)] private int audioSourceCount = 10;
    private AudioSource musicSource;

    [Header("Volume")]

    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;

    #endregion

    #region Runtime

    private readonly List<AudioSource> audioSources =
        new List<AudioSource>();

    private readonly Dictionary<string, Sound> musicDictionary =
        new Dictionary<string, Sound>();

    private readonly Dictionary<string, Sound> soundDictionary =
        new Dictionary<string, Sound>();

    private int currentSourceIndex;

    #endregion

    #region Unity

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeAudioSources();

        soundDictionary.Clear();
        musicDictionary.Clear();

        InitializeSounds(sfxSounds);
        InitializeMusic();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Crée les AudioSources utilisées pour les SFX
    /// et la musique.
    /// </summary>
    private void InitializeAudioSources()
    {
        for (int i = 0; i < audioSourceCount; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();

            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;

            audioSources.Add(source);
        }

        // Source dédiée à la musique.
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;
    }

    /// <summary>
    /// Enregistre les sons dans le dictionnaire des SFX.
    /// </summary>
    private void InitializeSounds(List<Sound> soundsToInitialize)
    {
        foreach (Sound sound in soundsToInitialize)
        {
            if (sound == null ||
                string.IsNullOrWhiteSpace(sound.id) ||
                sound.clip == null)
            {
                Debug.LogWarning(
                    "[SoundManager] Son invalide dans la liste.",
                    this);

                continue;
            }

            if (soundDictionary.ContainsKey(sound.id))
            {
                Debug.LogWarning(
                    $"[SoundManager] ID de SFX en double : {sound.id}",
                    this);

                continue;
            }

            soundDictionary.Add(sound.id, sound);
        }
    }

    /// <summary>
    /// Enregistre les musiques disponibles.
    /// </summary>
    private void InitializeMusic()
    {
        foreach (Sound music in musicSounds)
        {
            if (music == null ||
                string.IsNullOrWhiteSpace(music.id) ||
                music.clip == null)
            {
                Debug.LogWarning(
                    "[SoundManager] Musique invalide dans la liste.",
                    this);

                continue;
            }

            if (musicDictionary.ContainsKey(music.id))
            {
                Debug.LogWarning(
                    $"[SoundManager] ID de musique en double : {music.id}",
                    this);

                continue;
            }

            musicDictionary.Add(music.id, music);
        }
    }

    #endregion

    #region SFX

    /// <summary>
    /// Joue un effet sonore à partir de son ID.
    /// </summary>
    public void PlaySFX(string id)
    {
        if (!soundDictionary.TryGetValue(id, out Sound sound))
        {
            Debug.LogWarning(
                $"[SoundManager] Son introuvable : {id}",
                this);

            return;
        }

        AudioSource source = GetNextAudioSource();

        source.pitch = 1;
        source.PlayOneShot(
            sound.clip,
            sound.volume * masterVolume
        );
    }

    #endregion

    #region Music

    /// <summary>
    /// Joue une musique à partir de son ID.
    /// Si une musique joue déjà, elle est remplacée.
    /// </summary>
    public void PlayMusic(string id)
    {
        if (!musicDictionary.TryGetValue(id, out Sound music))
        {
            Debug.LogWarning(
                $"[SoundManager] Musique introuvable : {id}",
                this);

            return;
        }

        // Évite de redémarrer la même musique.
        if (musicSource.isPlaying &&
            musicSource.clip == music.clip)
        {
            return;
        }

        musicSource.Stop();
        musicSource.clip = music.clip;
        musicSource.pitch = 1;
        musicSource.volume = music.volume * musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    /// <summary>
    /// Arrête la musique actuelle.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    /// <summary>
    /// Met la musique en pause.
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    /// <summary>
    /// Reprend la musique après une pause.
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource != null &&
            musicSource.clip != null)
        {
            musicSource.UnPause();
        }
    }

    /// <summary>
    /// Modifie le volume de la musique.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null && musicSource.clip != null)
        {
            Sound music = null;

            foreach (Sound sound in musicDictionary.Values)
            {
                if (sound.clip == musicSource.clip)
                {
                    music = sound;
                    break;
                }
            }

            float clipVolume = music != null ? music.volume : 1f;

            musicSource.volume = clipVolume * musicVolume;
        }
    }

    #endregion

    #region Volume

    /// <summary>
    /// Modifie le volume général des SFX.
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }

    #endregion

    #region Audio Sources

    /// <summary>
    /// Récupère la prochaine AudioSource pour les SFX.
    /// </summary>
    private AudioSource GetNextAudioSource()
    {
        AudioSource source = audioSources[currentSourceIndex];

        currentSourceIndex++;

        if (currentSourceIndex >= audioSources.Count)
            currentSourceIndex = 0;

        return source;
    }

    #endregion
}