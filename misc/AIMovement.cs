using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
	[SerializeField] float moveSpeed = 1f;
	[SerializeField] float turnSpeed = 10f;
    NavMeshAgent navMeshAgent = null;

	private void Awake()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.speed = moveSpeed;
		navMeshAgent.angularSpeed = turnSpeed;
		navMeshAgent.acceleration = turnSpeed * 0.05f;
	}

	public void Move(Vector3 position)
	{
		Continue();
		navMeshAgent.SetDestination(position);
	}

	public bool HasArrived() => navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending;

	public void Stop() => navMeshAgent.isStopped = true;

	public void Continue() => navMeshAgent.isStopped = false;

	public bool IsInRange(BattleUnit target)
	{
		if (target == null) return true; // crash mitigation
		return Vector3.Distance(transform.position, target.transform.position) < target.meleeRange;
	}
}
