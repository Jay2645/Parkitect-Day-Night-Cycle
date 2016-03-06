using UnityEngine;

public class Main : IMod
{
	private DayNight dayNight;

	public void onEnabled()
	{
		Light[] allLights = Object.FindObjectsOfType<Light>();
		Light sun = null;
		foreach (Light light in allLights)
		{
			if (light.type == LightType.Directional)
			{
				if (sun == null || light.intensity > sun.intensity)
				{
					sun = light;
				}
			}
		}
		dayNight = sun.gameObject.AddComponent<DayNight>();
	}

	public void onDisabled()
	{
		dayNight.Reset();
		Object.Destroy(dayNight);
	}

	/// <summary>
	///     Gets the name of this instance.
	/// </summary>
	public string Name
	{
		get
		{
			return "Day/Night Cycle";
		}
	}

	/// <summary>
	///     Gets the description of this instance.
	/// </summary>
	public string Description
	{
		get
		{
			return "Creates a day/night cycle in Parkitect.";
		}
	}

	private string _identifier;
	/// <summary>
	///     Gets an unique identifier of this mod.
	/// <summary>
	public string Identifier
	{
		get
		{
			return _identifier;
		}
		set
		{
			_identifier = value;
		}
	}

	private string _path;
	public string Path
	{
		get
		{
			return _path;
		}
		set
		{
			_path = value;
		}
	}
}