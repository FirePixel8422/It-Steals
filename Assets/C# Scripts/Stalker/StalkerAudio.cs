using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;



[System.Serializable]
public class StalkerAudio
{
    [Header("Screaming:")]
    [SerializeField] private AudioClip[] randomScreamClips;
    [SerializeField] private MinMaxFloat randomScreamPitch;
    [SerializeField] private MinMaxFloat randomScreamCooldown;
    private AudioSource randomScreamSource;
    private float nextScreamGlobalTime;

    [Header("Chase:")]
    [SerializeField] private AudioClip chaseClip;
    [SerializeField] private MinMaxFloat randomChasePitch;
    private AudioSource chaseSource;

    [Header("Spotted:")]
    [SerializeField] private AudioClip spottedClip;
    [SerializeField] private MinMaxFloat randomSpottedPitch;
    private AudioSource spottedSource;



    public void Init(Component stalkerRef)
    {
        randomScreamSource = stalkerRef.AddComponent<AudioSource>();
        randomScreamSource.loop = false;
        nextScreamGlobalTime = Time.time + EzRandom.Range(randomScreamCooldown);

        chaseSource = stalkerRef.AddComponent<AudioSource>();
        chaseSource.loop = true;

        spottedSource = stalkerRef.AddComponent<AudioSource>();
        spottedSource.loop = false;
    }

    public void OnUpdate()
    {
        if (Time.time > nextScreamGlobalTime)
        {
            nextScreamGlobalTime = Time.time + EzRandom.Range(randomScreamCooldown);
            PlayRandomAudioClip();
        }
    }


    private void PlayRandomAudioClip()
    {
        int r = EzRandom.Range(0, randomScreamClips.Length);
        float pitch = EzRandom.Range(randomScreamPitch);
        randomScreamSource.PlayOneShotClipWithPitch(randomScreamClips[r], pitch);
    }

    public void OnSpotted()
    {
        float pitch = EzRandom.Range(randomSpottedPitch);
        spottedSource.PlayOneShotClipWithPitch(spottedClip, pitch);
    }

    public void StartChase()
    {
        float pitch = EzRandom.Range(randomChasePitch);
        chaseSource.PlayClipWithPitch(chaseClip, pitch);
    }
    public void StopChase()
    {
        chaseSource.Stop();
    }
}