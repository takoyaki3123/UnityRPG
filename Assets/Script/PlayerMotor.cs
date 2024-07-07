using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{

    Transform target; // target to follow
    NavMeshAgent agent; // reference to our agent

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            FaceTarget();
        }

    }

    // Update is called once per frame
    public void MoveToPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }

    public void FollowTarget(Interactable newTarget)
    {
        //Targetとどれくらいの距離で止まるか
        agent.stoppingDistance = newTarget.radius * .8f;
        agent.updateRotation = false;

        target = newTarget.interactionTransform;
    }

    public void StopFollowingTarget()
    {
        agent.stoppingDistance = 0f;
        agent.updateRotation = true;
        target = null;
    }

    void FaceTarget ()
    {
        //normalized : return 1 or 0(when vector too samll)
        //重要なのは方向だから、大きさは大事じゃない
        Vector3 direction = (target.position - transform.position).normalized;
        //Rotation was handled by Quaternion class
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0f, direction.z));
        // use slerp to trun smoothly
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
