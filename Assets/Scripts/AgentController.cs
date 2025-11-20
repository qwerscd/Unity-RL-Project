using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class RayAgentController : Agent
{
    public Rigidbody rb;
    public float moveSpeed = 3f;
    public float jumpForce = 5f;

    // 安全出生区域
    public Vector3 spawnAreaMin = new Vector3(-1f, 0.5f, -1f);
    public Vector3 spawnAreaMax = new Vector3(1f, 0.5f, 1f);

    // Ray感知参数
    public int rayCount = 8;
    public float rayLength = 3f;
    public LayerMask obstacleMask;

    public override void Initialize()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // 重置速度和角速度
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 安全随机位置
        bool safe = false;
        while (!safe)
        {
            Vector3 pos = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                spawnAreaMin.y,
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            // 检查是否与障碍物重叠
            if (!Physics.CheckSphere(pos, 0.5f, obstacleMask))
            {
                transform.localPosition = pos;
                safe = true;
            }
        }

        transform.localRotation = Quaternion.identity;
        Debug.Log($"Episode Begin: Agent start position = {transform.localPosition}");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1) Agent速度 (3)
        sensor.AddObservation(rb.linearVelocity);

        // 2) Agent相对中心位置 (2)
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        // 3) Ray感知
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * 360f / rayCount;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, rayLength, obstacleMask))
            {
                sensor.AddObservation(hit.distance / rayLength);
            }
            else
            {
                sensor.AddObservation(1f);
            }

            // Debug可视化Ray
            Debug.DrawRay(rayOrigin, dir * rayLength, Color.red, 0.1f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float jump = actions.ContinuousActions[2];

        // 移动
        Vector3 vel = rb.linearVelocity;
        vel.x = moveX * moveSpeed;
        vel.z = moveZ * moveSpeed;
        rb.linearVelocity = vel;

        // 跳跃
        if (jump > 0.5f && Mathf.Abs(transform.localPosition.y - 0.5f) < 0.05f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        // 生存奖励
        AddReward(0.001f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-1f);
            EndEpisode();
            Debug.Log("Hit obstacle, episode ended.");
        }
    }
}