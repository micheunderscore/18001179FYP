using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject[] players;
    public MeshRenderer floor;
    public float arenaSize;
    public string tagged;
    public OnTriggerEnterEvent playerOne, playerTwo;
    public Material matOne, matTwo;
    public void OnEnable() {
        playerOne.onTriggerEnter.AddListener(OnTheOtherTriggerEnterMethod);
        playerTwo.onTriggerEnter.AddListener(OnTheOtherTriggerEnterMethod);
    }

    public void Update() {
        if (tagged == players[0].tag) {
            floor.material = matOne;
        } else if (tagged == players[1].tag) {
            floor.material = matTwo;
        }
    }

    public void RestartGame() {
        tagged = players[Random.Range(0, 1)].tag;

        float spaceSize = arenaSize - 2;
        foreach (GameObject player in players) {
            player.transform.localPosition = new Vector3(Random.Range(-spaceSize, spaceSize), 0.9f, Random.Range(-spaceSize, -1f));
            player.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
    }

    public void OnTheOtherTriggerEnterMethod(Collider other) {
        // Debug.Log($"{other.tag} entered the trigger!");
        if (other.tag != tagged) {
            // TODO: Freeze the recently tagged person for 2 seconds
            tagged = other.tag;
        }
    }
}
