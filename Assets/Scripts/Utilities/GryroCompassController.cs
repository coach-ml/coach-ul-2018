using UnityEngine;

public class GryroCompassController : MonoBehaviour
{
    public Camera arCamera;

    private bool gyroAvailable;
    private const float lowPassFilterFactor = 0.5f;

    public void Start()
    {
        TryEnableGyro();
    }

    protected void Update()
    {
        if (gyroAvailable)
        {
            arCamera.transform.rotation = lowPassFilterQuaternion(arCamera.transform.rotation, ConvertRotation(Input.gyro.attitude), lowPassFilterFactor);
        }
    }

    Quaternion lowPassFilterQuaternion(Quaternion intermediateValue, Quaternion targetValue, float factor)
    {
        intermediateValue = targetValue;

        intermediateValue = Quaternion.Lerp(intermediateValue, targetValue, factor);
        return intermediateValue;
    }

    private static Quaternion ConvertRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    public bool TryEnableGyro(bool enable = true)
    {
        if (enable)
        {
            Input.gyro.enabled = SystemInfo.supportsGyroscope;
            gyroAvailable = Input.gyro.enabled;
        }
        else
        {
            gyroAvailable = Input.gyro.enabled = false;
        }

        return gyroAvailable;
    }
}
