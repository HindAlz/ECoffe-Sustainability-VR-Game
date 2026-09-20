using UnityEngine;

public class FirstPersonPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public Transform cameraTransform; // Assign your camera here

    private float pitch = 0f; // Camera up/down

    void Update()
    {
        MovePlayerWASD();
        RotateWithArrowKeys();
    }

    void MovePlayerWASD()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }

    void RotateWithArrowKeys()
    {
        float yaw = 0f;
        float pitchDelta = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) yaw = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) yaw = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) pitchDelta = -1f;
        if (Input.GetKey(KeyCode.DownArrow)) pitchDelta = 1f;

        // Rotate player (Y axis)
        transform.Rotate(0f, yaw * rotationSpeed * Time.deltaTime, 0f);

        // Rotate camera (X axis)
        pitch += pitchDelta * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}
