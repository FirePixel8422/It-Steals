using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;



[System.Serializable]
public class PlayerAudio
{
    [SerializeField] private AudioClip heartBeatClip;
    [SerializeField] private MinMaxFloat heartBeatPitch;

    [SerializeField] private AudioClip dangerClip;
    [SerializeField] private MinMaxFloat dangerPitch;
    [SerializeField] private MinMaxFloat dangerVolume;

    public float DangerMaxDist = 20f;

    private AudioSource heartBeatSource;
    private AudioSource dangerSource;


    public void Init(Component stalkerRef)
    {
        heartBeatSource = stalkerRef.AddComponent<AudioSource>();
        heartBeatSource.loop = true;

        dangerSource = stalkerRef.AddComponent<AudioSource>();
        dangerSource.loop = true;
    }

    public void OnUpdate()
    {
        
    }

    public void StartHeartBeat()
    {
        heartBeatSource.PlayClipWithPitch(heartBeatClip, 1);
    }
    public void UpdateHeartBeat(float percentage01)
    {
        heartBeatSource.pitch = math.lerp(heartBeatPitch.min, heartBeatPitch.max, percentage01);
    }
    public void StopHeartBeat()
    {
        heartBeatSource.Stop();
    }

    public void StartDangerSFX()
    {
        dangerSource.PlayClipWithPitch(dangerClip, 1);
    }
    public void UpdateDangerSFX(float percentage01)
    {
        dangerSource.volume = math.lerp(dangerVolume.min, dangerVolume.max, percentage01);
        dangerSource.pitch = math.lerp(dangerPitch.min, dangerPitch.max, percentage01);
    }
    public void StopDangerSFX()
    {
        dangerSource.Stop();
    }
}