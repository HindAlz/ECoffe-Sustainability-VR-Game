using UnityEngine;
using UnityEngine.SceneManagement;

public class resetToast : MonoBehaviour
{
    public void reset()
    {
        SceneManager.LoadScene("short");
    }

    public void full()
    {
        SceneManager.LoadScene("Game");
    }
    public void view()
    {
        SceneManager.LoadScene("view");

    }
}
