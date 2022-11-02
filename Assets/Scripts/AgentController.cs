using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent {
    public float mouseInputX, mouseInputY, xInput, zInput, tagTimer, distance = 0f;
    public bool crouchInput, jumpInput, printDebug;
    private int id, playerId;
    private Debugger debugger;
    [SerializeField] private GameManager gameState;
    [SerializeField] private Transform targetTransform;

    public void Start() {
        debugger = transform.root.GetComponentInChildren<Debugger>();

        id = 0;
        int.TryParse(Regex.Match(transform.parent.name, @"\d+").Value, out id);
        id += 1;

        int.TryParse(Regex.Match(transform.name, @"\d+").Value, out playerId);
        playerId -= 1;
    }

    public void FixedUpdate() {
        distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        bool isTagged = gameState.tagged == transform.tag;

        switch (playerId) {
            case 0:
                if (debugger != null) debugger.meanBank1.Add(GetCumulativeReward());
                break;
            case 1:
                if (debugger != null) debugger.meanBank2.Add(GetCumulativeReward());
                break;
            default:
                break;
        }

        // DEBUGGER 
        if (debugger != null && printDebug) {
            debugger.update($"{(playerId + 1).ToString("D2")}", $"{transform.name} Rewards: {GetCumulativeReward()}");
            debugger.update($"{(playerId + 3).ToString("D2")}", $"{transform.name} = {isTagged}");
            debugger.update("10", $"{distance}");
        }

        // Reset sides and give head start
        if (gameState.serveReward[playerId]) {
            tagTimer = 0f;
            float rewardReset = -GetCumulativeReward();
            float rewardSet = !isTagged ? +gameState.rewardAmt : -gameState.punishAmt;
            AddReward(rewardReset + rewardSet);
            gameState.ServedReward(playerId);
            // gameState.debug[2] = $"Spec Reward : {rewardReset} + {rewardSet}";
        } else {
            // Reward/Punishment distribution
            // Distance from each agent
            if (gameState.timeTick[playerId]) {
                if (gameState.rDistance) {
                    AddReward(distance * (isTagged ? -1f : +1f) * gameState.distanceMod);
                }
                // Time from tagged
                if (gameState.rTime) {
                    AddReward(gameState.timeVal * gameState.timeMod * (isTagged ? -1f : +1f));
                }
                gameState.ServedTick(playerId);
            }
            // gameState.debug[1] = $"Reward : {(distance * (isTagged ? -1f : +1f) * gameState.distanceMod)}";
        }

        if (gameState.endByReward) {
            if ((gameState.rewardThreshold != 0f) && (GetCumulativeReward() > gameState.rewardThreshold)) {
                EndEpisode();
            };
        }

        if (gameState.endByTime) {
            if (gameState.tagTimer == gameState.timeLimit) {
                AddReward(isTagged ? 0f : gameState.winReward);
                EndEpisode();
            }
        }

        if (gameState.endByTotalTime) {
            if (gameState.gameTimer == gameState.gameTimeLimit) {
                EndEpisode();
            }
        }
    }

    public override void OnEpisodeBegin() {
        SetReward(0f);
        gameState.RestartGame();
    }

    public override void CollectObservations(VectorSensor sensor) {
        // Player positions
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(targetTransform.localRotation);

        // Tag State
        sensor.AddObservation(gameState.tagged == transform.tag ? 1f : 0f);
        sensor.AddObservation(gameState.players[playerId].GetComponent<PlayerMovement>().frozen ? 1f : 0f);

        // REMOVED: Uncomment to include player distance to be included in observations
        // // Player Distance
        // sensor.AddObservation(distance);

        // REMOVED: Uncomment to include timer data to be included in observations
        // // Timer
        // sensor.AddObservation(gameState.tagTimer);
        // sensor.AddObservation(gameState.timeLimit);
        // sensor.AddObservation(gameState.gameTimer);
        // sensor.AddObservation(gameState.gameTimeLimit);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        mouseInputX = actions.ContinuousActions[0];
        zInput = actions.ContinuousActions[1];
        xInput = actions.ContinuousActions[2];
        // REMOVED: Uncomment to enable Y-axis look, crouching, or jumping
        // mouseInputY = actions.ContinuousActions[3] * Time.deltaTime;
        // crouchInput = actions.DiscreteActions[0] == 1;
        // jumpInput = actions.DiscreteActions[1] == 1;
    }

    public void OnControllerColliderHit(ControllerColliderHit other) {
        if (other.transform.tag == "Wall" && gameState.wallDeath) {
            gameState.SwitchTag(transform.tag);
            float rewardReset = -GetCumulativeReward();
            AddReward(rewardReset + (-gameState.punishAmt * gameState.wallMultiplier));
            EndEpisode();
        }
    }

    // Heuristics testing
    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        // REMOVED: Uncomment to initialize discrete actions
        // ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        continuousActions[0] = Input.GetAxis("Mouse X");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        continuousActions[2] = Input.GetAxisRaw("Horizontal");

        // REMOVED: Uncomment to enable Y-axis look, crouching, or jumping
        // continuousActions[3] = Input.GetAxis("Mouse Y");
        // discreteActions[0] = Input.GetButton("Crouch") ? 1 : 0;
        // discreteActions[1] = Input.GetButton("Jump") ? 1 : 0;
    }
}
