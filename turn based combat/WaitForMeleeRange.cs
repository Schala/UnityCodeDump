using UnityEngine;

public class WaitForMeleeRange : CustomYieldInstruction
{
	readonly AIMovement movement;
	readonly BattleUnit target;

	public WaitForMeleeRange(AIMovement movement, BattleUnit target)
	{
		this.movement = movement;
		this.target = target;
	}

	public override bool keepWaiting => !movement.IsInRange(target);
}
