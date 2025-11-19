using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class AgentController : Agent
{
    public Rigidbody rb;
    public float moveSpeed = 3f;
    public float jumpForce = 5f;

    public override void Initialize()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // 重置速度和位置（随机化位置略微有利训练）
        rb.linearVelocity = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-2f,2f), 0.5f, Random.Range(-2f,2f));
        transform.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1) agent 速度 (3 values)
        sensor.AddObservation(rb.linearVelocity);

        // 2) agent 相对位置到中心（可选）
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        // 总观测维度：3 + 2 = 5（再加 RayPerception 会自动加观测）
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float jump = actions.ContinuousActions[2];

        Vector3 move = new Vector3(moveX, 0f, moveZ) * moveSpeed;
        // 使移动更稳定：直接设置 velocity 的 x,z
        Vector3 vel = rb.linearVelocity;
        vel.x = move.x;
        vel.z = move.z;
        rb.linearVelocity = vel;

        // Jump（只有当接近地面时才允许跳跃）
        if (jump > 0.5f && Mathf.Abs(transform.localPosition.y - 0.5f) < 0.05f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        // 生存奖励（每个决策步）
        AddReward(0.001f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}