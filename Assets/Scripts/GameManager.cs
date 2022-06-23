using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject[] players;
    public MeshRenderer floor;
    public float arenaSize, rewardAmt, punishAmt, distanceMod;
    public string tagged;
    public bool serveReward = false;
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
        tagged = players[0].tag;
        float spaceSize = arenaSize - 2;

        players[0].transform.localPosition = new Vector3(Random.Range(-spaceSize, spaceSize), 0.9f, Random.Range(-spaceSize, -1f));
        players[0].transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        players[1].transform.localPosition = new Vector3(Random.Range(-spaceSize, spaceSize), 0.9f, Random.Range(spaceSize, 1f));
        players[1].transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    public void OnTheOtherTriggerEnterMethod(Collider other) {
        // Debug.Log($"{other.tag} entered the trigger!");
        if (other.tag != tagged) {
            // TODO: Freeze the recently tagged person for 2 seconds
            tagged = other.tag;
            serveReward = true;
        }
    }

    public void ServedReward() {
        serveReward = false;
    }
}
