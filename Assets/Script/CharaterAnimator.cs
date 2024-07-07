using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharaterAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    // get charater speed
    NavMeshAgent agent;
    Animator animator;

    const float locomotionAnimationSmoothTime = .1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);
    }
}