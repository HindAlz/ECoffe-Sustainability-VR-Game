using UnityEngine;

public class BinManager : MonoBehaviour
{
    public float scoreCooldown = 2f; // Delay in seconds
    private bool canScore = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canScore) return;

        if (other.CompareTag("Plastic") || other.CompareTag("Glass") ||
            other.CompareTag("Cans") || other.CompareTag("Paper"))
        {
            Trashmanager manager = FindObjectOfType<Trashmanager>();
            if (manager == null)
            {
                Debug.LogWarning("No TrashManager found in scene.");
                return;
            }

            bool isCorrect = other.tag == this.tag;

            if (isCorrect)
            {
                manager.AddScore(5);
            }

           
            Destroy(other.gameObject);

            StartCoroutine(ScoreCooldown());
        }
    }

    private System.Collections.IEnumerator ScoreCooldown()
    {
        canScore = false;
        yield return new WaitForSeconds(scoreCooldown);
        canScore = true;
    }
}
