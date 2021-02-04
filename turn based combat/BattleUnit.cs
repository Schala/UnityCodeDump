using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[System.Flags]
public enum BattleUnitStatus : byte
{
	Normal = 0,
	Dead = 1
}

[RequireComponent(typeof(AIMovement))]
public class BattleUnit : MonoBehaviour
{
	public bool ActedThisTurn { get; set; } = false;
	public BattleUnitStatus Status { get; private set; } = BattleUnitStatus.Normal;
	public float meleeRange = 1f;
	[SerializeField] string displayName = "Unnamed";
	AIMovement movement = null;
	TMP_Text nameTag = null;
	NavMeshObstacle obstacle = null;
	NavMeshAgent agent = null;

	private void Awake()
	{
		movement = GetComponent<AIMovement>();
		nameTag = GetComponentInChildren<TMP_Text>();
		obstacle = GetComponent<NavMeshObstacle>();
		agent = GetComponent<NavMeshAgent>();
		Name = displayName;
	}

	public void SetAsObstacle(bool b)
	{
		// To avoid Unity screaming at us, reverse the enable/disable order
		if (b)
		{
			agent.enabled = false;
			obstacle.enabled = true;
		}
		else
		{
			obstacle.enabled = false;
			agent.enabled = true;
		}
	}

	public IEnumerator Attack(BattleUnit target)
	{
		SetAsObstacle(false);
		target.SetAsObstacle(false);
		movement.Move(target.transform.position);
		yield return new WaitForMeleeRange(movement, target);
		var strikes = Random.Range(1, 4);
		for (int i = 0; i < strikes; i++)
		{
			print($"{Name} booped {target.Name} on the snoot!");
			yield return new WaitForSeconds(1f);
		}

		SetAsObstacle(true);
		target.SetAsObstacle(true);
		ActedThisTurn = true;
	}

	public string Name
	{
		get { return displayName; }
		set
		{
			displayName = value;
			nameTag.text = value;
		}
	}
}
