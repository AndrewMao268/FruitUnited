using UnityEngine;

public class SoldierSpawner : MonoBehaviour {
    GameObject soldier;
    void Start() {
        soldier = GameObject.Find("Soldier");

        for (int i = 0; i < 19; i++) {
            Instantiate(soldier);
        }
    }
}