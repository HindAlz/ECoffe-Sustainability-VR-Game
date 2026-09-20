using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    // Reference to the CustomerSystem that handles the customer logic
    public CustomerSystem customerSystem;

    // A flag to check if the ChessBoard is active
    private bool isChessBoardActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure that the CustomerSystem reference is set
        if (customerSystem == null)
        {
            Debug.LogWarning("CustomerSystem reference is not assigned!");
        }
        else
        {
            // Initially, check if the ChessBoard is in the scene and active
            isChessBoardActive = gameObject.activeSelf;
            UpdateCustomerPatience();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Continuously check if the ChessBoard is active in the scene
        if (gameObject.activeSelf != isChessBoardActive)
        {
            isChessBoardActive = gameObject.activeSelf;
            UpdateCustomerPatience();
        }
    }

    // Update the customer patience time based on ChessBoard's active status
    void UpdateCustomerPatience()
    {
        if (isChessBoardActive)
        {
            // Directly change the patienceTimer in CustomerSystem to 120 seconds if ChessBoard is active
            if (customerSystem != null)
            {
                customerSystem.patienceTimer = 130f; // Directly modifying the patienceTimer
            }
        }
        else
        {
            // Optionally, revert the patienceTimer to the default (e.g., 60 seconds) if ChessBoard is not active
            if (customerSystem != null)
            {
                customerSystem.patienceTimer = 100f; // Default patience
            }
        }
    }
}
