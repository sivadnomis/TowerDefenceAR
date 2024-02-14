using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class ARLightAmbient : MonoBehaviour
{
    private Light l;
    public ARCameraManager arCameraManager;

#if UNITY_IOS
    void Start ()
    {
        l = GetComponent<Light>();
        arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnCameraFrameReceived (ARCameraFrameEventArgs eventArgs)
    {
        l.intensity = eventArgs.lightEstimation.averageBrightness.Value;
        l.colorTemperature = eventArgs.lightEstimation.averageColorTemperature.Value;
    }

    void OnDisable ()
    {
        arCameraManager.frameReceived -= OnCameraFrameReceived;
    }
#endif
}