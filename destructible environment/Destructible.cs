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
	public class Destructible : MonoBehaviour
	{
		[SerializeField] GameObject m_FracturedPrefab;
		[SerializeField] float m_VelocityDilution = 4f;

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag("Weapon"))
			{
				var fractured = Instantiate(m_FracturedPrefab, transform.position, transform.rotation);
				var debris = fractured.GetComponentsInChildren<Debris>();

				for (int i = 0; i < debris.Length; i++)
					debris[i].m_Force = collision.gameObject.GetComponent<Rigidbody>().velocity / m_VelocityDilution;

				Destroy(gameObject);
			}
		}
	}
}
