using UnityEngine;

public class BasilController : MonoBehaviour
{
    public GameObject basil; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Toast"))
        {
            basil.SetActive(false);
            StartCoroutine(ReactivateBasil());
        }
    }

    private System.Collections.IEnumerator ReactivateBasil()
    {
        yield return new WaitForSeconds(5f);
        basil.SetActive(true);
    }
}
