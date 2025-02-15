using UnityEngine;

public class SoldierSpawner : MonoBehaviour {
    GameObject soldier;
    void Start() {
        soldier = GameObject.Find("TheSpawnOfEvil");

        for (int i = 0; i < 99; i++) {
            Instantiate(soldier);
        }
    }
}