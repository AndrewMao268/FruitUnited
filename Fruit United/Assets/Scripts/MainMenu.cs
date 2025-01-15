using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject settingsMenu; // (By the way, CHATGPT is awesome..but still very dumb - Brandon)
    public Button settingsButton; 
    public Button closeButton;

    void Start()
    {
        // Ensure the settings menu is hidden when the game starts
        settingsMenu.SetActive(false);

        // Add a listener to the button to call ShowSettingsMenu when clicked
        settingsButton.onClick.AddListener(ShowSettingsMenu);
        closeButton.onClick.AddListener(CloseSettingsMenu);
    }
    void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
    }
    void ShowSettingsMenu()
    {
        // Toggle the visibility of the settings menu
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
   public void StartGame()
   {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
   }

   public void QuitGame()
    {
        Application.Quit();
    }
   
}
