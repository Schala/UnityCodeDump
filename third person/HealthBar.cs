﻿/*
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
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Gradient gradient;
	public Slider slider;
	public Image fill;
	int _maxValue;
	int _current;

	public int current
	{
		get
		{
			return _current;
		}

		set
		{
			if (value < 0) return;

			_current = value;
			slider.value = value;
			fill.color = gradient.Evaluate(slider.normalizedValue);
		}
	}

	public int maxValue
	{
		get
		{
			return _maxValue;
		}

		set
		{
			if (value < _current) return;

			_maxValue = value;
			slider.maxValue = value;
			fill.color = gradient.Evaluate(1f);
		}
	}
}
