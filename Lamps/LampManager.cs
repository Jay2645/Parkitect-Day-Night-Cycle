using System.Collections.Generic;
using UnityEngine;

namespace DayNightCycle
{
	public class LampManager : MonoBehaviour
	{
		/// <summary>
		/// How often we should try to find lamps. Lower values cause a larger performance hit.
		/// </summary>
		public float lampCheckTime = 2.0f;
		/// <summary>
		/// A list of all lamps we've created, for easy reference and cleanup.
		/// </summary>
		public List<Lamp> allLamps = new List<Lamp>();
		private float currentTime = 0.0f;

		private void FixedUpdate()
		{
			currentTime += Time.fixedDeltaTime;
			if (currentTime >= lampCheckTime)
			{
				FindLamps();
				currentTime = 0.0f;
			}
		}

		/// <summary>
		/// Finds all lamps in the game.
		/// VERY SLOW AND HACKY.
		/// </summary>
		private void FindLamps()
		{
			// Get every PathAttachment in the game
			PathAttachment[] scenery = FindObjectsOfType<PathAttachment>();
			foreach (PathAttachment deco in scenery)
			{
				// If it doesn't already have a Lamp component and it has "lamp" in the name, it's a lamp
				Lamp lamp = deco.GetComponent<Lamp>();
				if (lamp != null)
				{
					continue;
				}
				if (deco.getName().ToLower().Contains("lamp"))
				{
					allLamps.Add(deco.gameObject.AddComponent<Lamp>());
				}
			}
		}

		/// <summary>
		/// Destroys all lamps we've created.
		/// </summary>
		public void CleanUp()
		{
			Lamp[] lampArray = allLamps.ToArray();
			foreach (Lamp lamp in lampArray)
			{
				lamp.CleanUp();
				Destroy(lamp);
			}
		}
	}
}