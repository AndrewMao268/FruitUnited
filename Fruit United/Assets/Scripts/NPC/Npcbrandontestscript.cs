using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Npcbrandontestscript : MonoBehaviour, IInteractable
{
    public NPCDialogueTest dialogueDataBrandonTest;
    public GameObject dialoguePanelBrandonTest;
    public TMP_Text dialogueTextBrandon, nameTextBrandon;
    public Image portraitImage;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private bool collidedWithPortal;

    private string firstLine = "Hello There!";

    void Start()
    {
        dialogueDataBrandonTest.dialogueLines[0] = firstLine;
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if(dialogueDataBrandonTest == null)
        {
            return;
        }
        if (isDialogueActive)
        {
            NextLine();
        }
        else 
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        nameTextBrandon.SetText(dialogueDataBrandonTest.npcName);
        portraitImage.sprite = dialogueDataBrandonTest.npcPortrait;

        dialoguePanelBrandonTest.SetActive(true);
        

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if(isTyping)
        {
            StopAllCoroutines();
            dialogueTextBrandon.SetText(dialogueDataBrandonTest.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if(++dialogueIndex < dialogueDataBrandonTest.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueTextBrandon.SetText("");

        foreach(char letter in dialogueDataBrandonTest.dialogueLines[dialogueIndex])
        {
            dialogueTextBrandon.text += letter;
            yield return new WaitForSeconds(dialogueDataBrandonTest.typingSpeed);
        }

        isTyping = false;

        if(dialogueDataBrandonTest.autoProgressLines.Length > dialogueIndex && dialogueDataBrandonTest.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueDataBrandonTest.autoProgressDelay);
            if(!collidedWithPortal)
            {
            NextLine();
            }
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueTextBrandon.SetText("");
        dialoguePanelBrandonTest.SetActive(false);
        
    }

    public void ChangeFirstDialogue(string newFirstLine)
    {
        if (!collidedWithPortal)
        {
            dialogueDataBrandonTest.dialogueLines[0] = newFirstLine;
            collidedWithPortal = true;
        }
    }
}
