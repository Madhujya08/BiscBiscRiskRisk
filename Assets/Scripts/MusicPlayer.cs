using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.RenderGraphModule;

[RequireComponent (typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [Header("Assign the Music +1 tracks ")]
    [SerializeField] private AudioClip[] tracks;

    [Header("Route to mixer")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    [SerializeField] private float fadeSeconds = 0.5f;

    AudioSource src;

    private void Awake()
    {
       if(Instance != null && Instance != this) { Destroy(gameObject); return;}
        Instance = this;
        DontDestroyOnLoad(gameObject);

        src = GetComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;
        if (musicMixerGroup) src.outputAudioMixerGroup = musicMixerGroup;
    }

    private void Start()
    {
        if (tracks != null && tracks.Length > 0)
            PlayImmediate(0);                                        //start the first track immediately
    }

    public void PlayImmediate(int index)
    {
        if (index < 0 || index >= tracks.Length) return;
        src.clip = tracks[index];
        src.volume = 1f;
        src.Play();
    }

    public void FadeTo(int index)
    {
        if (index < 0 || index >= tracks.Length) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(index));
    }

    IEnumerator FadeRoutine(int index)
    {
        float t = 0f, start = src.volume;
        while (t < fadeSeconds) { t += Time.unscaledDeltaTime; src.volume = Mathf.Lerp(start, 0f, t / fadeSeconds); yield return null;}
        src.Stop();
        src.clip = tracks[index];
        src.Play();
        t = 0f;
        while (t < fadeSeconds) { t += Time.unscaledDeltaTime; src.volume = Mathf.Lerp(0f, 1f, t / fadeSeconds); yield return null; }
        src.volume = 1f;
    }
}
