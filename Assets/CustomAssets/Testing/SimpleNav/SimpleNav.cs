using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNav : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Space]
    [SerializeField] Transform wayPointRoot;
    [SerializeField] private Transform currentGoal;

    [Space]
    [SerializeField] private float goalOffset = 0.0f;
    [SerializeField] private float idleTime = 0.0f;
    [SerializeField] private float maxSpeed = 0.0f;

    private bool arrivedGoalPos = true;

    void Start()
    {
        //agent.stoppingDistance = 0.5f;

        if (arrivedGoalPos && wayPointRoot.childCount > 0)
        {
            currentGoal = wayPointRoot.GetChild(Random.Range(0, wayPointRoot.childCount - 1));
            agent.SetDestination(currentGoal.position);
            arrivedGoalPos = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckDist();
        SetAnimation();
    }

    void SetAnimation()
    {
        animator.SetFloat("Speed", agent.speed);
    }

    void CheckDist()
    {
        if (Vector3.Distance(this.transform.position, currentGoal.position) <= goalOffset)
        {
            agent.speed = 0f;
            arrivedGoalPos = true;
            StartCoroutine(RandomNav());
        }
    }

    IEnumerator RandomNav()
    {
        yield return new WaitForSeconds(Random.Range(0f, idleTime));

        if (arrivedGoalPos && wayPointRoot.childCount > 0)
        {
            currentGoal = wayPointRoot.GetChild(Random.Range(0, wayPointRoot.childCount - 1));
            agent.SetDestination(currentGoal.position);
            agent.speed = maxSpeed;
            arrivedGoalPos = false;
        }
    }
}
