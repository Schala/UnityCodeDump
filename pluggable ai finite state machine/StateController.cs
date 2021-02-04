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
using StudioJamNov2020.Entities;
using System.Collections;
using UnityEngine;

namespace StudioJamNov2020.AI
{
    public class StateController : MonoBehaviour
    {
        public Transform[] m_Waypoints;
        public bool m_AIActive;

        [Header("States")]
        public State m_CurrentState;
        public State m_RemainState;

        [Header("Looking")]
        public float m_LookRadius = 1f;

        [Header("Searching")]
        public float m_SearchDuration = 4f;

        [Header("Audio")]
        public AudioClip[] m_Normal = null;
        public float m_AudioInterval = 2f;

        [HideInInspector] public Combatant m_Combatant;
        [HideInInspector] public UnitController m_Unit;
        [HideInInspector] public int m_NextWaypoint;
        [HideInInspector] public float m_StateTimeElapsed;
        [HideInInspector] public AudioSource m_AudioSource = null;

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_Combatant = GetComponent<Combatant>();
            m_Unit = GetComponent<UnitController>();
            m_AudioSource.pitch = Random.Range(0.75f, 1.25f);
        }

        void Update()
        {
            if (!m_AIActive) return;
            m_CurrentState.UpdateState(this);
        }

        private void Start() => m_Unit.m_NavMeshAgent.enabled = m_AIActive;

        private void OnDrawGizmos()
        {
            if (m_CurrentState == null|| m_Unit == null) return;
            Gizmos.color = m_CurrentState.m_SceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position, m_LookRadius);
        }

        public void TransitionToState(State newState)
        {
            if (newState == m_RemainState) return;
            m_CurrentState = newState;
            OnExitState();
        }

        public bool CheckIfCountDownElapsed(float duration)
        {
            m_StateTimeElapsed += Time.deltaTime;
            return m_StateTimeElapsed >= duration;
        }

        private void OnExitState() => m_StateTimeElapsed = 0f;

        public IEnumerator AudioPlay()
        {
            if (m_AudioSource.isPlaying) yield break;
            m_AudioSource.clip = m_Normal[Random.Range(0, m_Normal.Length - 1)];
            m_AudioSource.Play();
        }
    }
}
