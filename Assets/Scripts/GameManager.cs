using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject[] players;
    public MeshRenderer floor;
    public float arenaSize, rewardAmt, punishAmt, distanceMod;
    private Vector3[] startPos = { Vector3.zero, Vector3.zero };
    public string tagged;
    public bool serveReward = false, randomPos = false;
    public OnTriggerEnterEvent playerOne, playerTwo;
    private PlayerMovement p1Control, p2Control;
    public Material matOne, matTwo;

    public void OnEnable() {
        playerOne.onTriggerEnter.AddListener(OnTheOtherTriggerEnterMethod);
        playerTwo.onTriggerEnter.AddListener(OnTheOtherTriggerEnterMethod);
    }

    public void Start() {
        players[0].TryGetComponent<PlayerMovement>(out p1Control);
        players[1].TryGetComponent<PlayerMovement>(out p2Control);

        startPos[0] = players[0].transform.localPosition;
        startPos[1] = players[1].transform.localPosition;
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
        Vector3[] usedPos = { Vector3.zero, Vector3.zero };

        // Model rigging is bad so can't do foreach for initialization for some reason. Oof
        if (randomPos) {
            usedPos[0] = new Vector3(Random.Range(-spaceSize, spaceSize), 0.9f, Random.Range(-spaceSize, -1f));
            players[0].transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            if (p1Control != null) p1Control.xRotation = 0f;

            usedPos[1] = new Vector3(Random.Range(-spaceSize, spaceSize), 0.9f, Random.Range(spaceSize, 1f));
            players[1].transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            if (p2Control != null) p2Control.xRotation = 0f;
        } else {
            usedPos[0] = startPos[0];
            usedPos[1] = startPos[1];
        }

        players[0].transform.localPosition = usedPos[0];
        players[1].transform.localPosition = usedPos[1];
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
