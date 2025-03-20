using UnityEngine;

public class PendulumAxe2 : MonoBehaviour
{
    [Header("Pendulum Settings")]
    [Tooltip("Maximum swing angle (in degrees) to each side.")]
    public float maxAngle = 45f;

    [Tooltip("How fast the pendulum swings.")]
    public float swingSpeed = 2f;

    // Optional: a starting offset if you don't want it to begin exactly centered
    [Tooltip("Offset to shift the starting angle of the swing.")]
    public float angleOffset = 0f;

    void Update()
    {
        // Calculate an angle between -maxAngle and +maxAngle using a sine wave
        float angle = maxAngle * Mathf.Sin(Time.time * swingSpeed + angleOffset);

        // Rotate this pivot (and thus all its children) around the Z axis
        // If your pendulum should swing in a different axis, change (0,0,angle) accordingly
        transform.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }
}
