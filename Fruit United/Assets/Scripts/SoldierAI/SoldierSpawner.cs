using UnityEngine;

public class SoldierSpawner : MonoBehaviour {

    public GameObject soldiersFolder;
    public GameObject soldier;
    void Start() {

        for (int i = 0; i < 9; i++) {
            GameObject newSoldier = Instantiate(soldier, soldiersFolder.transform);
        }
    }
}