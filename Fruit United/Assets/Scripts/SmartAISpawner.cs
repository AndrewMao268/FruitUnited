using UnityEngine;

public class SmartAISpawner : MonoBehaviour
{
    public GameObject smartAI;
    void Start()
    {
        for (int i = 0; i < 99; i++)
        {
            Instantiate(smartAI);
        }
    }
}