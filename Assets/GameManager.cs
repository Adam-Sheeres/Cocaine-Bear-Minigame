using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void EndGame()
    {
        Debug.Log("GAME OVER");
        Invoke("End", 4.0f);
        
    }

    public void End()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
