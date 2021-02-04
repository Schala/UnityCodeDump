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

namespace StudioJamNov2020.Entities
{
	public class Debris : MonoBehaviour
	{
		public Vector3 m_Force = Vector3.zero;
		[SerializeField] Material m_PieceMaterial = null;
		[SerializeField] float m_Lifetime = 5f;
		[SerializeField] float m_PieceMass = 0.5f;
		Rigidbody[] m_Bodies;

		private void Awake()
		{
			// Programmatically add materials to each piece to save headaches.
			AddMaterial();

			m_Bodies = GetComponentsInChildren<Rigidbody>();
		}

		void AddMaterial()
		{
			var renderers = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < renderers.Length; i++)
				renderers[i].material = m_PieceMaterial;
		}

		void Start()
		{
			for (int i = 0; i < m_Bodies.Length; i++)
			{
				m_Force.y = Random.value;
				m_Bodies[i].AddForce(m_Force, ForceMode.Impulse);
			}

			var entity = GetComponent<Thing>();
			entity.m_DecayTime = m_Lifetime;
			entity.m_IsActive = true;
		}
	}
}
