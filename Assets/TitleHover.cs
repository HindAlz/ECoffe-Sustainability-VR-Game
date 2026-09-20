using UnityEngine;

public class TitleHover : MonoBehaviour
{
    public float amplitude = 10f;    // How far up and down it moves (in units, like pixels)
    public float speed = 2f;         // How fast it moves

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
