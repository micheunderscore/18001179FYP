using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent {
    public float mouseInputX, mouseInputY, xInput, zInput;
    public bool crouchInput, jumpInput;
    [SerializeField] private GameManager gameState;
    [SerializeField] private Transform targetTransform;

    public void Update() {
        float distance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        // Reset sides and give head start
        if (gameState.serveReward) {
            SetReward(0f);
            AddReward(gameState.tagged != transform.tag ? +gameState.rewardAmt : -gameState.punishAmt);
        }

        // Reward/Punishment distribution
        AddReward(distance * (gameState.tagged != transform.tag ? +1f : -1f) * gameState.distanceMod);

        // Toggle once sides have been changed
        if (gameState.serveReward) {
            gameState.ServedReward();
        }

        // Debug.Log($"{transform.name} Rewards: {GetCumulativeReward()}");
    }

    public override void OnEpisodeBegin() {
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
        mouseInputY = actions.ContinuousActions[1] * Time.deltaTime;
        xInput = actions.ContinuousActions[2];
        zInput = actions.ContinuousActions[3];
        jumpInput = actions.DiscreteActions[0] == 1;
        crouchInput = actions.DiscreteActions[1] == 1;
    }

    public void OnControllerColliderHit(ControllerColliderHit other) {
        // Debug.Log($"{transform.tag} touched {other.transform.tag}");
        if (other.transform.tag == "Wall") {
            AddReward(-gameState.punishAmt);
            EndEpisode();
        }
    }

    // Heuristics testing
    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxis("Mouse X");
        continuousActions[1] = Input.GetAxis("Mouse Y");
        continuousActions[2] = Input.GetAxisRaw("Horizontal");
        continuousActions[3] = Input.GetAxisRaw("Vertical");
        discreteActions[0] = Input.GetButton("Jump") ? 1 : 0;
        discreteActions[1] = Input.GetButton("Crouch") ? 1 : 0;
    }
}
