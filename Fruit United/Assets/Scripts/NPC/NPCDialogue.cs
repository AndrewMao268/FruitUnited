using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    private int nextNPCId;
    private List<NPCDialogueData> npcDatas;
    private List<NPCDialogueAgent> npcAgents;

    private List<GameObject> textBoxes;

    public GameObject textBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextNPCId = 0;
        npcDatas = new List<NPCDialogueData>();
        npcAgents = new List<NPCDialogueAgent>();
        textBoxes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int RegisterNPC(NPCDialogueData npcData)
    {
        int id = nextNPCId++;

//        npcDatas.Add(npcData);
        // TODO: Write instantiate function that resizes text boxes accordingly for
        // simultaneous dialogues
        GameObject newTextBox = Instantiate(textBox, transform);
//        textBoxes.Add(newTextBox);
   //     npcAgents.Add(new NPCDialogueAgent(newTextBox, npcData));

        return id;
    }

    public void ShowDialogue(int npcId)
    {
        npcAgents[npcId].ShowDialogue();
    }
}
