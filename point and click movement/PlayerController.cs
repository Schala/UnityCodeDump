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

using StudioJamNov2020.Battle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StudioJamNov2020.Entities.Player
{
	public enum PlayerState : byte
	{
		Idle = 0,
		Moving,
		MovingWithinRange,
		Attacking
	}

	public class PlayerController : MonoBehaviour
	{
		UnitController m_Unit;
		Combatant m_Combatant;
		PlayerState m_State = PlayerState.Idle;
		RaycastHit m_LastHit;
		bool m_Exiting = false;

		private void Awake()
		{
			m_Unit = GetComponent<UnitController>();
			m_Combatant = GetComponent<Combatant>();
		}

		void FixedUpdate()
		{
			if (m_Unit.m_NavMeshAgent.isStopped) return;

			switch (m_State)
			{
				case PlayerState.Idle:
					CheckNewAction();
					break;
				case PlayerState.Moving:
					m_Unit.Move(m_LastHit.point);
					CheckNewAction();
					if (m_Unit.HasArrived()) m_State = PlayerState.Idle;
					break;
				case PlayerState.MovingWithinRange:
					m_Unit.Move(m_LastHit.point);
					CheckNewAction();
					if (m_Combatant.IsInRange()) m_State = PlayerState.Attacking;
					break;
				case PlayerState.Attacking:
					m_Unit.Stop();
					StartCoroutine(m_Combatant.Attack());
					break;
			}
		}

		private void Update()
		{
			if (m_Combatant.m_Flags.HasFlag(CombatFlags.Dead)) Destroy(gameObject);

			if (Mouse.current.leftButton.wasReleasedThisFrame)
			{
				m_Combatant.Disengage();
				m_Unit.Continue();
				CheckNewAction();
				m_State = PlayerState.Idle;
			}

			if (m_State == PlayerState.Attacking)
				if (m_Combatant.m_Target != null) transform.LookAt(m_Combatant.m_Target.transform, Vector3.up);
		}

		Ray GetMouseRay() => Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

		bool IsMousePressed() => Mouse.current.leftButton.isPressed;

		bool CheckTarget()
		{
			var cursorPoint = GetMouseRay();
			var hits = Physics.RaycastAll(cursorPoint);

			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].collider.CompareTag("Enemy") || hits[i].collider.CompareTag("Destructible"))
				{
					m_LastHit = hits[i];
					m_Combatant.m_Target = hits[i].collider.gameObject;

					if (m_Combatant.IsInRange())
					{
						if (m_Combatant.m_Target != null) m_State = PlayerState.Attacking;
						return true;
					}

					m_State = PlayerState.MovingWithinRange;
					return true;
				}
			}

			// No target found in ray hit, so let's clear it. The player probably clicked away from the target.
			m_Combatant.m_Target = null;
			return false;
		}

		bool CheckMove()
		{
			var cursorPoint = GetMouseRay();

			if (Physics.Raycast(cursorPoint, out m_LastHit, Mathf.Infinity, LayerMask.GetMask("Level")))
			{
				m_State = PlayerState.Moving;
				return true;
			}

			return false;
		}

		void CheckNewAction()
		{
			if (!IsMousePressed()) return;
			if (CheckTarget()) return;
			CheckMove();
		}

		private void OnDestroy()
		{
			if (m_Exiting) return;

			var gameManager = FindObjectOfType<GameManager>();
			var audio = gameManager.GetComponent<AudioSource>();
			gameManager.m_GameOverText.SetActive(true);
			audio.Stop();
			audio.clip = gameManager.m_GameOverBGM;
			audio.volume = 1f;
			audio.Play();
		}

		private void OnApplicationQuit() => m_Exiting = true;
	}
}
