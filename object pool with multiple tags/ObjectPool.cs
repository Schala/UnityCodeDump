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

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolItem
{
    public int amount;
    public GameObject prefab = null;
    public bool expandable;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; } = null;

    [SerializeField] List<ObjectPoolItem> items = null;
    List<GameObject> pool = null;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;

        pool = new List<GameObject>();
    }

    private void Start()
    {
        for (int i = 0; i < items.Count; i++)
        {
            for (int j = 0; j < items[i].amount; j++)
            {
                var obj = Instantiate(items[i].prefab);
                obj.name = $"{obj.name} {(char)(j + 65)}";
                obj.SetActive(false);
                pool.Add(obj);
            }
        }
    }

    public GameObject Get(string tag)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy && pool[i].TryGetComponent(out TagCollection tags))
            {
                if (tags.tags.Contains(tag))
                    return pool[i];
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].prefab.TryGetComponent(out TagCollection tags))
            {
                if (items[i].expandable && tags.tags.Contains(tag))
                {
                    var obj = Instantiate(items[i].prefab);
                    obj.SetActive(false);
                    pool.Add(obj);
                    return obj;
                }
            }
        }

        return null;
    }

	private void OnDestroy()
	{
        for (int i = 0; i < pool.Count; i++)
            Destroy(pool[i]);
	}
}
