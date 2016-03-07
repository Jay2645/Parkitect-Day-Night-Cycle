using MiniJSON;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DayNightCycle
{
	public class Main : IMod, IModSettings
	{
		private DayNight dayNight;
		private LampManager lampManager;
		private EnvironmentManager environmentManager;

		private Dictionary<string, object> settingsValueDictionary = new Dictionary<string, object>();

		private const string DAY_CYCLE_SETTINGS_TEXT = "Cycle Length (In Minutes)";
		private const string CHANGE_TEMP_AT_NIGHT_SETTINGS_TEXT = "Change Temperature at Night";
		private const string USE_LAMPS_SETTINGS_TEXT = "Use Lamp Lights at Night";

		#region Start Mod
		public void onEnabled()
		{
			// Find where the sun is
			// The sun is defined by Unity's procedural skybox as the brightest directional light in the sky
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
			lampManager = sun.gameObject.AddComponent<LampManager>();
			environmentManager = sun.gameObject.AddComponent<EnvironmentManager>();
			ReadSettingsFile();
		}

		public void onDisabled()
		{
			// Turn off the day/night cycle
			dayNight.Reset();
			// Destroy all lamps
			lampManager.CleanUp();
			// Clean up our Components
			Object.Destroy(environmentManager);
			Object.Destroy(lampManager);
			Object.Destroy(dayNight);
		}
		#endregion

		#region Settings Panel
		public void onDrawSettingsUI()
		{
			if (dayNight != null)
			{
				dayNight.dayCycleInMinutes = DrawFloatField(DAY_CYCLE_SETTINGS_TEXT, dayNight.dayCycleInMinutes);
			}
			if (environmentManager != null)
			{
				environmentManager.changeTemperatureAtNight = DrawBoolField(CHANGE_TEMP_AT_NIGHT_SETTINGS_TEXT, environmentManager.changeTemperatureAtNight);
			}
			if (lampManager != null)
			{
				bool enableLampManager = DrawBoolField(USE_LAMPS_SETTINGS_TEXT, lampManager.enabled);
				if (!enableLampManager && lampManager.enabled)
				{
					lampManager.CleanUp();
					lampManager.enabled = false;
				}
				else if (enableLampManager && !lampManager.enabled)
				{
					lampManager.enabled = true;
				}
			}
		}

		private bool DrawBoolField(string fieldName, bool initialValue)
		{
			bool value = GUILayout.Toggle(initialValue, fieldName);
			settingsValueDictionary[fieldName] = value;
			return value;
		}

		private float DrawFloatField(string fieldName, float initialValue)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(fieldName);
			string floatString = GUILayout.TextField(initialValue.ToString("F1"));
			float floatValue;
			if (!float.TryParse(floatString, out floatValue))
			{
				floatValue = initialValue;
			}
			else if (floatValue == 0.0f)
			{
				floatValue = initialValue;
			}
			GUILayout.EndHorizontal();
			settingsValueDictionary[fieldName] = floatString;
			return floatValue;
		}

		public void onSettingsOpened()
		{

		}

		public void onSettingsClosed()
		{
			WriteSettingsFile();
		}
		#endregion

		#region Save/Load Settings
		public void WriteSettingsFile()
		{
			string jsonDictionary = Json.Serialize(settingsValueDictionary);
			File.WriteAllText(Path + @"/settings.json", jsonDictionary);
		}

		public void ReadSettingsFile()
		{
			settingsValueDictionary = Json.Deserialize(File.ReadAllText(Path + @"/settings.json")) as Dictionary<string, object>;
			if (dayNight != null)
			{
				float dayCycleLength;
				if (float.TryParse((string)settingsValueDictionary[DAY_CYCLE_SETTINGS_TEXT], out dayCycleLength))
				{
					dayNight.dayCycleInMinutes = dayCycleLength;
				}
			}
			if (environmentManager != null)
			{
				environmentManager.changeTemperatureAtNight = (bool)settingsValueDictionary[CHANGE_TEMP_AT_NIGHT_SETTINGS_TEXT];
			}
			if (lampManager != null)
			{
				bool shouldEnable = (bool)settingsValueDictionary[USE_LAMPS_SETTINGS_TEXT];
				if (!shouldEnable)
				{
					lampManager.CleanUp();
					lampManager.enabled = false;
				}
				else
				{
					lampManager.enabled = true;
				}
			}
		}
		#endregion

		#region Mod Details
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
		#endregion
	}
}