using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent {
    public float mouseInputX, mouseInputY, xInput, zInput, rewardThreshold;
    public bool crouchInput, jumpInput;
    [SerializeField] private GameManager gameState;
    [SerializeField] private Transform targetTransform;


    public void FixedUpdate() {
        float distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        bool isTagged = gameState.tagged == transform.tag;

        gameState.debug[0] = $"{transform.name} Rewards: {GetCumulativeReward()}";

        // Debug.Log($"Served Reward? : {gameState.serveReward}");
        // Reset sides and give head start
        if (gameState.serveReward) {
            float rewardReset = -GetCumulativeReward();
            float rewardSet = !isTagged ? +gameState.rewardAmt : -gameState.punishAmt;
            AddReward(rewardReset + rewardSet);
            // gameState.debug[2] = $"Spec Reward : {rewardReset} + {rewardSet}";
            gameState.ServedReward();
        } else {
            // Reward/Punishment distribution
            AddReward(distance * (isTagged ? -1f : +1f) * gameState.distanceMod);
            // gameState.debug[1] = $"Reward : {(distance * (isTagged ? -1f : +1f) * gameState.distanceMod)}";
        }

        if (GetCumulativeReward() > rewardThreshold) EndEpisode();
    }

    public override void OnEpisodeBegin() {
        SetReward(0f);
        gameState.RestartGame();
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(targetTransform.localRotation);
        sensor.AddObservation(gameState.tagged == transform.tag ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        mouseInputX = actions.ContinuousActions[0] * Time.deltaTime;
        zInput = actions.ContinuousActions[1];
        xInput = actions.ContinuousActions[2];
        // mouseInputY = actions.ContinuousActions[3] * Time.deltaTime;
        // crouchInput = actions.DiscreteActions[0] == 1;
        // jumpInput = actions.DiscreteActions[1] == 1;
    }

    public void OnControllerColliderHit(ControllerColliderHit other) {
        // Debug.Log($"{transform.tag} touched {other.transform.tag}");
        if (other.transform.tag == "Wall") {
            SetReward(-gameState.punishAmt);
            gameState.ResetFloor();
            EndEpisode();
        }
    }

    // Heuristics testing
    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        // ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxis("Mouse X");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        continuousActions[2] = Input.GetAxisRaw("Horizontal");
        // continuousActions[3] = Input.GetAxis("Mouse Y");
        // discreteActions[0] = Input.GetButton("Crouch") ? 1 : 0;
        // discreteActions[1] = Input.GetButton("Jump") ? 1 : 0;
    }
}
