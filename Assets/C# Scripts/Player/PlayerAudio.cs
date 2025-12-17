using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;



[System.Serializable]
public class PlayerAudio
{
    [SerializeField] private AudioClip heartBeatClip;
    [SerializeField] private MinMaxFloat heartBeatPitch;
    private AudioSource heartBeatSource;


    public void Init(Component stalkerRef)
    {
        heartBeatSource = stalkerRef.AddComponent<AudioSource>();
        heartBeatSource.loop = true;
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
}