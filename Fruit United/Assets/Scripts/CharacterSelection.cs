using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    private void OnEnable() {
        UIDocument uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Q("MangoButton").RegisterCallback<ClickEvent>(ChangeScene);
        uiDocument.rootVisualElement.Q("BananaButton").RegisterCallback<ClickEvent>(ChangeScene);
        uiDocument.rootVisualElement.Q("PomegranateButton").RegisterCallback<ClickEvent>(ChangeScene);
        uiDocument.rootVisualElement.Q("WatermelonButton").RegisterCallback<ClickEvent>(ChangeScene);
        uiDocument.rootVisualElement.Q("GrapeButton").RegisterCallback<ClickEvent>(ChangeScene);
    }

    private void ChangeScene(ClickEvent evt) {
        SceneManager.LoadScene("Level001");
    }
}