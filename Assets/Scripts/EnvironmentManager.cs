using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public Transform[] obstacles;
    public Transform agentTransform;

    public void ResetEnvironment()
    {
        // 随机化障碍物位置
        foreach (Transform ob in obstacles)
        {
            ob.localPosition = new Vector3(Random.Range(-3.5f,3.5f), ob.localPosition.y, Random.Range(-3.5f,3.5f));
        }
        // Agent 的 OnEpisodeBegin 会重置自己
    }
}