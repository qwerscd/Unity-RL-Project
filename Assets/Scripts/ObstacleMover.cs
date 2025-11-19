using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 dir;
    public float areaLimit = 4.5f;

    void Start()
    {
        dir = new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f,1f)).normalized;
    }

    void Update()
    {
        transform.localPosition += dir * speed * Time.deltaTime;

        // 碰到边界反弹
        Vector3 pos = transform.localPosition;
        if (Mathf.Abs(pos.x) > areaLimit) dir.x *= -1f;
        if (Mathf.Abs(pos.z) > areaLimit) dir.z *= -1f;
    }
}