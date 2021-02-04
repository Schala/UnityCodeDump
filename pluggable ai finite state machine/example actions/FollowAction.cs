/*
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street - Fifth Floor, Boston, MA 02110-1301, USA.
 */

using UnityEngine;

namespace StudioJamNov2020.AI.Actions
{
	[CreateAssetMenu(menuName = "AI/Actions/Follow")]
	public class FollowAction : Action
	{
		public override void Act(StateController controller)
		{
			var combatant = controller.m_Combatant;
			if (combatant.m_Target == null) return;

			controller.m_Unit.Move(combatant.m_Target.transform.position);
			controller.StartCoroutine(controller.AudioPlay());

			if (combatant.IsInRange())
			{
				if (controller.CheckIfCountDownElapsed(combatant.m_Rate))
				{
					controller.m_Unit.Stop();
					controller.transform.LookAt(combatant.m_Target.transform, Vector3.up);
					controller.StartCoroutine(combatant.Attack());
				}
			}
		}
	}
}
