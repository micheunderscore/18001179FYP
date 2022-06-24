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
    public PlayerMovement p1Control, p2Control;
    public Material matOne, matTwo;

    // vvv DEBUG STUFF REMEMBER TO REMOVE vvv
    public string[] debug = new string[10];

    public void OnEnable() {
        playerOne.onTriggerEnter.AddListener(OnTheOtherTriggerEnterMethod);
        playerTwo.onTriggerEnter.AddListener(OnTheOtherTriggerEnterMethod);
    }

    public void Start() {
        players[0].TryGetComponent<PlayerMovement>(out p1Control);
        players[1].TryGetComponent<PlayerMovement>(out p2Control);
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

        // Model rigging is bad so can't do foreach for initialization for some reason. Oof
        players[0].transform.localPosition = new Vector3(Random.Range(-spaceSize, spaceSize), 0.9f, Random.Range(-spaceSize, -1f));
        players[0].transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        if (p1Control != null) p1Control.xRotation = 0f;

        players[1].transform.localPosition = new Vector3(Random.Range(-spaceSize, spaceSize), 0.9f, Random.Range(spaceSize, 1f));
        players[1].transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        if (p2Control != null) p2Control.xRotation = 0f;
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

    // void OnGUI() {
    //     GUI.Label(
    //         new Rect(
    //             5,                       // x, left offset
    //             0,                       // y, bottom offset
    //             300f,                    // width
    //             150f                     // height
    //         ),
    //         string.Join("\n", debug),    // the display text
    //         GUI.skin.textArea            // use a multi-line text area
    //     );
    // }
}
