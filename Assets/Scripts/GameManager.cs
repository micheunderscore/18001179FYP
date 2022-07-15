using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public TMP_Text timeText;
    public MeshRenderer floor;
    public Material matOne, matTwo;
    public OnTriggerEnterEvent playerOne, playerTwo;
    private PlayerMovement p1Control, p2Control;
    public GameObject[] players;
    public Transform[] spawns;
    [HideInInspector]
    public bool[] serveReward;
    public bool[] timeTick;
    [HideInInspector]
    public string tagged;
    public int timeLimit = 10, gameTimeLimit = 0;
    public float rewardAmt, punishAmt, distanceMod = 0.1f, timeVal = 0.001f, timeMod = 2f, rewardThreshold, wallMultiplier, winReward = 0f;
    public bool randomPos = false, randomTagged = false, rDistance = false, rTime = false, wallDeath = true, endByTime = false, endByReward = false, endByTotalTime = false;
    [HideInInspector]
    public int tagTimer = 0, gameTimer = 0;
    private Vector3[] startPos = { Vector3.zero, Vector3.zero };
    private bool timerInvoked = false, gameInvoked = false;
    private System.Random rand = new System.Random();
    private Debugger debugger;

    public void OnEnable() {
        playerOne.onTriggerEnter.AddListener(OnTheOtherTriggerEnterMethod);
        playerTwo.onTriggerEnter.AddListener(OnTheOtherTriggerEnterMethod);
    }

    public void Start() {
        debugger = transform.root.GetComponentInChildren<Debugger>();

        players[0].TryGetComponent<PlayerMovement>(out p1Control);
        players[1].TryGetComponent<PlayerMovement>(out p2Control);

        startPos[0] = players[0].transform.localPosition;
        startPos[1] = players[1].transform.localPosition;
    }

    public void Update() {
        timeText.text = tagTimer.ToString("00");
        if (!timerInvoked) {
            timerInvoked = true;
            tagTimer++;
            timeTick = new bool[] { true, true };
            Invoke("InvokeTimer", 1);
        }

        if (!gameInvoked) {
            gameInvoked = true;
            gameTimer++;
            Invoke("InvokeGameTick", 1);
        }

        if (tagged == players[0].tag) {
            floor.material = matOne;
        } else if (tagged == players[1].tag) {
            floor.material = matTwo;
        }
    }

    public void RestartGame() {
        gameTimer = 0;
        tagTimer = 0;
        tagged = players[randomTagged ? rand.Next(2) : 0].tag;
        serveReward = new bool[] { true, true };

        Vector3[] usedPos = { Vector3.zero, Vector3.zero };

        // Model rigging is bad so can't do foreach for initialization for some reason. Oof
        if (randomPos) {
            int spawn = 0;
            int[] selectedSpawns = new int[] { -1, -1 };
            for (int i = 0; i < players.Length; i++) {
                do {
                    spawn = Random.Range(0, spawns.Length);
                } while (selectedSpawns.Contains(spawn));
                usedPos[i] = spawns[spawn].localPosition;
                players[i].transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                selectedSpawns[i] = spawn;
            }

            if (p2Control != null) p2Control.xRotation = 0f;
            if (p1Control != null) p1Control.xRotation = 0f;
        } else {
            usedPos[0] = startPos[0];
            usedPos[1] = startPos[1];
        }

        players[0].transform.localPosition = usedPos[0];
        players[1].transform.localPosition = usedPos[1];
    }

    public void OnTheOtherTriggerEnterMethod(Collider other) {
        if (other.tag != tagged) {
            if (other.TryGetComponent<PlayerMovement>(out PlayerMovement targetScript)) {
                targetScript.freeze();
            }
            SwitchTag(other.tag);
        }
    }

    public void SwitchTag(string tagAgent) {
        tagged = tagAgent;
        tagTimer = 0;
        serveReward = new bool[] { true, true };
    }

    public void InvokeTimer() {
        timerInvoked = false;
    }

    public void InvokeGameTick() {
        gameInvoked = false;
    }

    public void ServedReward(int id) {
        serveReward[id] = false;
    }

    public void ServedTick(int id) {
        timeTick[id] = false;
    }
}
