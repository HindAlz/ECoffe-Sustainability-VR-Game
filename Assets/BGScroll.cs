using UnityEngine;
using UnityEngine.UI;

public class BGScroll : MonoBehaviour
{
    public RawImage backgroundImage;  // Assign your panel's RawImage here
    public Vector2 scrollSpeed = new Vector2(0.1f, -0.1f);  // X and Y scroll speeds

    private Vector2 currentOffset = Vector2.zero;

    void Update()
    {
        if (backgroundImage != null)
        {
            currentOffset += scrollSpeed * Time.deltaTime;
            backgroundImage.uvRect = new Rect(currentOffset, backgroundImage.uvRect.size);
        }
    }
}
