using UnityEngine;

[RequireComponent(typeof(Camera))]

public class PreserveASpectCamera : MonoBehaviour
{
    [Header("Camera Z Positions")]
    [SerializeField] private float iphoneZ = -10f;
    [SerializeField] private float ipadZ = -16f;
    [SerializeField] private GameObject bgScale; 

    // iPad landscape is ~1.33, iPhone is >= ~1.77
    private const float iPadAspectThreshold = 1.6f;

    private void Awake()
    {
        ApplyAspect();
    }

    private void ApplyAspect()
    {
        float aspect = (float)Screen.width / Screen.height;
        Vector3 pos = transform.position;

        // Wider screen → iPhone
        if (aspect >= iPadAspectThreshold)
        {
            pos.z = iphoneZ;
            bgScale.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        // Narrower → iPad
        else
        {
            pos.z = ipadZ;
            bgScale.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        transform.position = pos;
    }
}


    


