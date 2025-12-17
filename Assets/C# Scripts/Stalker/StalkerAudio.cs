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

    [Header("Chase:")]
    [SerializeField] private AudioClip chaseClip;
    [SerializeField] private MinMaxFloat randomChasePitch;
    private AudioSource chaseSource;

    private AudioSource randomScreamSource;
    private float nextScreamGlobalTime;


    public void Init(Component stalkerRef)
    {
        randomScreamSource = stalkerRef.AddComponent<AudioSource>();
        randomScreamSource.loop = false;

        chaseSource = stalkerRef.AddComponent<AudioSource>();
        chaseSource.loop = true;

        nextScreamGlobalTime = Time.time + EzRandom.Range(randomScreamCooldown);
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