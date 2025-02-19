using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject player;
    public GameObject interactionText;
    public GameObject npcDialogueObject;

    private bool dialogueSetup;
    private NPCDialogueData dialogueData;
    private NPCDialogue npcDialogue;
    private int npcDialogueId;

    public float interactionDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcDialogue = npcDialogueObject.GetComponent<NPCDialogue>();

        List<string> dialogueList = new List<string>();
        dialogueList.Add("I am an NPC!");

        dialogueData = new NPCDialogueData(dialogueList);

        npcDialogueId = npcDialogue.RegisterNPC(dialogueData);

        dialogueSetup = true;
    }

    // Update is called once per frame
    void Update()
    {
        bool closeEnough = GetDistanceFromPlayer() < interactionDistance;
        interactionText.SetActive(closeEnough);
        if (closeEnough && Input.GetKeyDown(KeyCode.F))
        {
            ShowDialogue();
        }
    }

    private void Checks()
    {
        if (interactionDistance < 0.0f)
        {
            throw new System.Exception("interactionDistance cannot be negative");
        }
    }

    private float GetDistanceFromPlayer()
    {
        float xDistance = player.transform.position.x - transform.position.x;
        float yDistance = player.transform.position.y - transform.position.y;
        return Mathf.Sqrt(Mathf.Pow(xDistance, 2.0f) + Mathf.Pow(yDistance, 2.0f));
    }

    private void ShowDialogue()
    {
        npcDialogue.ShowDialogue(npcDialogueId);
    }
}
