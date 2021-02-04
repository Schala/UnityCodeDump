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

public class Thing : MonoBehaviour
{
    public float m_DecayTime = 2f;
    public float m_FadeTime = 5f;
    [HideInInspector] public bool m_IsActive = false;
    Renderer[] m_Renderers = null;
    float m_FadeDelta = 0f;

    void Awake()
    {
        m_Renderers = GetComponentsInChildren<Renderer>();
        m_FadeDelta = Time.deltaTime / m_FadeTime;
    }

    void Update()
    {
        if (!m_IsActive) return;

        if (m_DecayTime >= 0f)
        {
            m_DecayTime -= Time.deltaTime;
            return;
        }

        for (int i = 0; i < m_Renderers.Length; i++)
        {
            var current = m_Renderers[i].material.color;
            current.a = Mathf.Lerp(current.a, 0f, m_FadeDelta);
            m_Renderers[i].material.color = current;
        }

        m_FadeTime -= Time.deltaTime;
        if (m_FadeTime <= 0f) Destroy(gameObject);
    }
}
