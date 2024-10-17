using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public Material foggedMaterial; // Assign this in the Unity Editor
    public float fogSpeed = 1.0f; // Adjust this to control the speed of fog movement

    void Update()
    {
        if (foggedMaterial != null)
        {
            float offset = Time.time * fogSpeed;
            foggedMaterial.SetFloat("_FogOffset", offset);
        }
    }

}
