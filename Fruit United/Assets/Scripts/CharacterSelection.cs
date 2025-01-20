using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public Button mangoContainer;
    public Button bananaContainer;
    public Button pomegranateContainer;
    public Button watermelonContainer;
    public Button grapeContainer;

    void Start() {
        mangoContainer.onClick.AddListener(ChangeScene);
        bananaContainer.onClick.AddListener(ChangeScene);
        pomegranateContainer.onClick.AddListener(ChangeScene);
        watermelonContainer.onClick.AddListener(ChangeScene);
        grapeContainer.onClick.AddListener(ChangeScene);
    }

    private void OnEnable() {
    }

    private void ChangeScene() {
        SceneManager.LoadScene("Level001");
    }
}