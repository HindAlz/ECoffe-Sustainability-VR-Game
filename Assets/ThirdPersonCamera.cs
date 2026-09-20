using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public float distance = 5f;  // Distance from the player
    public float height = 2f;  // Height of the camera
    public float rotationSpeed = 20f;  // Camera rotation speed

    private float currentRotation = 0f;

    void LateUpdate()
    {
        // Check for arrow key input to rotate the camera
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentRotation -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentRotation += rotationSpeed * Time.deltaTime;
        }

        // Calculate the desired camera position
        Vector3 offset = new Vector3(0, height, -distance);
        Quaternion rotation = Quaternion.Euler(0, currentRotation, 0);

        transform.position = player.position + rotation * offset;

        // Always look at the player
        transform.LookAt(player);
    }
}
