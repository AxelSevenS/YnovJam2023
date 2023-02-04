using UnityEngine;

public class EnemyMover : MonoBehaviour {
    public float wanderRadius = 5.0f;
    public float speed = 3.0f;
    private Vector3 targetPos;

    void Start(){
        targetPos = ChooseNewTargetPos();
    }

    void Update(){
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            targetPos = ChooseNewTargetPos();
        }
    }

    Vector3 ChooseNewTargetPos(){
        Vector3 newTargetPos = transform.position + new Vector3(Random.Range(-wanderRadius, wanderRadius), Random.Range(-wanderRadius, wanderRadius), 0);
        return newTargetPos;
    }
}