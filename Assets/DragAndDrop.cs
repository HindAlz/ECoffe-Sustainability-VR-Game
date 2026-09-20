using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset; // Offset between the mouse position and the object
    private Collider binCollider; // Collider for the bin
    public string binType; // Name of the bin type this object should go to
    private float fixedZ; // Store the initial Z position

    private TrashBehavior trashBehavior; // Reference to the TrashBehavior component

    void Start()
    {
        // Store the initial Z position to keep it constant
        fixedZ = transform.position.z;

        // Ensure the object has a collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("No collider found on object! Add a Collider to the object.");
            return;
        }

        // Find the bin GameObject by name or tag
        GameObject binObject = GameObject.FindWithTag(binType);
        if (binObject != null)
        {
            binCollider = binObject.GetComponent<Collider>();
        }
        else
        {
            Debug.LogWarning("Bin not found! Ensure that a bin with the correct tag exists in the scene.");
        }

        // Get reference to this object's TrashBehavior
        trashBehavior = GetComponent<TrashBehavior>();
        if (trashBehavior == null)
        {
            Debug.LogError("No TrashBehavior component found on this object!");
        }
    }

    private Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xyPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZ)); // Plane parallel to XY
        float distance;

        if (xyPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            return new Vector3(hitPoint.x, hitPoint.y, fixedZ); // Keep Z fixed
        }
        return transform.position;
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMousePos();
    }

    void OnMouseDrag()
    {
        transform.position = GetMousePos() + offset;
    }

    void OnMouseUp()
    {
        bool correctBinFound = false;
        bool isNoneBin = false;

        // Check if the trash was dropped into the correct bin
        if (binCollider != null && binCollider.bounds.Contains(transform.position))
        {
            correctBinFound = true;
        }

        // If no correct bin was found, check if dropped into the None bin
        if (!correctBinFound && binCollider != null && binCollider.CompareTag("None"))
        {
            correctBinFound = true;
            isNoneBin = true;
        }

        // Use this object's TrashBehavior to handle placement
        if (trashBehavior != null)
        {
            trashBehavior.HandleTrashPlacement(this.gameObject, correctBinFound, isNoneBin);
        }

        // Always destroy the trash once sorted
        Destroy(gameObject);
    }
}
