using System.Collections.Generic;
using UnityEngine;

class LampManager : MonoBehaviour
{
	public float lampCheckTime = 2.0f;
	public List<Lamp> allLamps = new List<Lamp>();
	private float currentTime = 0.0f;

	private void Update()
	{
		currentTime += Time.deltaTime;
		if (currentTime >= lampCheckTime)
		{
			FindLamps();
			currentTime = 0.0f;
		}
	}


	private void FindLamps()
	{
		PathAttachment[] scenery = FindObjectsOfType<PathAttachment>();
		foreach (PathAttachment deco in scenery)
		{
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