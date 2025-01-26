using UnityEngine;
using UnityEngine.SceneManagement; // Required to manage scenes

public class SceneChanger : MonoBehaviour
{
    // Call this method when the button is clicked
    public void LoadNextScene()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the next scene (current index + 1)
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}