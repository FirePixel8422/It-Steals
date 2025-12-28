using FirePixel.Utility;
using UnityEngine;


public class FlashlightSway : MonoBehaviour
{
    [SerializeField] private Transform flashlightTransform;
    [SerializeField] private Transform followTransform;
    [SerializeField] private float lerpSpeed;

    private Quaternion prevRot;


    private void OnEnable() => UpdateScheduler.RegisterLateUpdate(OnLateUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterLateUpdate(OnLateUpdate);

    private void OnLateUpdate()
    {
        flashlightTransform.rotation = Quaternion.Lerp(prevRot, followTransform.rotation, lerpSpeed * Time.deltaTime);

        prevRot = flashlightTransform.rotation;
    }
}