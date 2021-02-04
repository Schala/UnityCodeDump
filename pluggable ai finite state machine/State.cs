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

namespace StudioJamNov2020.AI
{
	[CreateAssetMenu(menuName = "AI/State")]
	public class State : ScriptableObject
	{
		[SerializeField] Action[] m_Actions;
		[SerializeField] Transition[] m_Transitions;
		public Color m_SceneGizmoColor = Color.gray;

		public void UpdateState(StateController controller)
		{
			DoActions(controller);
			CheckTransitions(controller);
		}

		void DoActions(StateController controller)
		{
			for (int i = 0; i < m_Actions.Length; i++)
				m_Actions[i].Act(controller);
		}

		void CheckTransitions(StateController controller)
		{
			for (int i = 0; i < m_Transitions.Length; i++)
			{
				bool decisionSuccess = m_Transitions[i].decision.Decide(controller);
				controller.TransitionToState(decisionSuccess ? m_Transitions[i].trueState : m_Transitions[i].falseState);
			}
		}
	}
}
