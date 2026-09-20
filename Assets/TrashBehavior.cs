using UnityEngine;

public class TrashBehavior : MonoBehaviour
{
    public string binType; // What bin type this trash belongs to
    private Vector3 offset;
    private float fixedZ;

    private Collider[] allBins;

    void Start()
    {
        fixedZ = transform.position.z;
        // Find all bins at start (assume bins are tagged "Bin")
        allBins = FindObjectsOfType<Collider>();
    }
    void Update()
    {
        if (transform.position.y > -0.33) Destroy(gameObject);
    }

    private Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xyPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZ));
        float distance;
        if (xyPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            return new Vector3(hitPoint.x, hitPoint.y, fixedZ);
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
        Collider closestBin = null;
        float closestDistance = float.MaxValue;

        // Find the closest bin that the trash is overlapping
        foreach (Collider bin in allBins)
        {
            if (bin.bounds.Contains(transform.position))
            {
                float dist = Vector3.Distance(transform.position, bin.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestBin = bin;
                }
            }
        }

        bool correctPlacement = false;
        bool isNoneBin = false;

        if (closestBin != null)
        {
            if (closestBin.CompareTag("None"))
            {
                isNoneBin = true;
                correctPlacement = true;
            }
            else if (closestBin.CompareTag(binType))
            {
                correctPlacement = true;
            }
        }
        else
        {
            // No bin was found under the trash
            // If no bin of our binType is visible, allow None
            bool correctBinVisible = false;
            foreach (Collider bin in allBins)
            {
                if (bin.CompareTag(binType))
                {
                    correctBinVisible = true;
                    break;
                }
            }

            if (!correctBinVisible)
            {
                isNoneBin = true;
                correctPlacement = true;
            }
        }

        // Handle the scoring
        HandleTrashPlacement(this.gameObject, correctPlacement, isNoneBin);


    }

    public void HandleTrashPlacement(GameObject trash, bool correctPlacement, bool isNoneBin)
    {
        if (correctPlacement)
        {
            if (isNoneBin)
            {
                Debug.Log($"{trash.name} placed in None bin correctly!");
                GameController.Instance.AddScore(2);
            }
            else
            {
                Debug.Log($"{trash.name} placed correctly!");
                GameController.Instance.AddScore(10);
            }
        }
        else
        {
            Debug.Log($"{trash.name} placed incorrectly!");
            GameController.Instance.DeductScore(5);
        }
    }
}
