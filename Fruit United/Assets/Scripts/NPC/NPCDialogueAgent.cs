using UnityEngine;
using TMPro;

public class NPCDialogueAgent
{
    private GameObject textBox;
    private NPCDialogueData data;
    private int dialogueIndex;

    public NPCDialogueAgent(GameObject textBox, NPCDialogueData data)
    {
        this.textBox = textBox;
        this.data = data;

        dialogueIndex = 0;
    }

    public void ShowDialogue()
    {
        GameObject textObject = textBox.transform.Find("DialogueText").gameObject;
        textObject.GetComponent<TMP_Text>().text = data.npcDialogue[dialogueIndex++];
    }
}