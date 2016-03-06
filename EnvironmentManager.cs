using System.Collections.ObjectModel;
using System.Reflection;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
	/// <summary>
	/// How much colder it is at night.
	/// </summary>
	public float nightTemperatureChangeAmount = 4.0f;
	/// <summary>
	/// How much more tired we should make guests at night.
	/// </summary>
	public float nightTirednessAddAmount = 0.06f;
	/// <summary>
	/// How much less tired we should make guests when it becomes day
	/// </summary>
	public float dayTirednessSubtractAmount = 0.05f;

	private float[] dayTemperatureMax;
	private float[] nightTemperatureMax;

	private void Start()
	{
		FieldInfo tempField = GetTemperatureMaxField();
		dayTemperatureMax = (float[])tempField.GetValue(WeatherController.Instance);
		nightTemperatureMax = new float[dayTemperatureMax.Length];
		for (int i = 0; i < dayTemperatureMax.Length; i++)
		{
			nightTemperatureMax[i] = dayTemperatureMax[i] - nightTemperatureChangeAmount;
		}
		DayNight.Instance.OnSunsetStart += ChangeToNightTemps;
		DayNight.Instance.OnSunriseStart += ChangeToDayTemps;
		DayNight.Instance.OnDayStart += WakeGuestsUp;
		DayNight.Instance.OnNightStart += MakeGuestsTired;
	}

	private void MakeGuestsTired()
	{
		ReadOnlyCollection<Guest> guests = GameController.Instance.park.getGuests();
		foreach (Guest guest in guests)
		{
			guest.Tiredness += nightTirednessAddAmount;
		}
	}

	private void WakeGuestsUp()
	{
		ReadOnlyCollection<Guest> guests = GameController.Instance.park.getGuests();
		foreach (Guest guest in guests)
		{
			guest.Tiredness -= dayTirednessSubtractAmount;
		}
	}

	private void ChangeToNightTemps()
	{
		FieldInfo tempField = GetTemperatureMaxField();
		tempField.SetValue(WeatherController.Instance, nightTemperatureMax);
		// Force-update temperature info
		FieldInfo monthField = GetLastWeatherCalculationMonth();
		monthField.SetValue(WeatherController.Instance, -1);
		WeatherController.Instance.getTemperature();
	}

	private void ChangeToDayTemps()
	{
		FieldInfo tempField = GetTemperatureMaxField();
		tempField.SetValue(WeatherController.Instance, dayTemperatureMax);
		// Force-update temperature info
		FieldInfo monthField = GetLastWeatherCalculationMonth();
		monthField.SetValue(WeatherController.Instance, -1);
		WeatherController.Instance.getTemperature();
	}

	private FieldInfo GetTemperatureMaxField()
	{
		BindingFlags flags = BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic;
		FieldInfo tempField = typeof(WeatherController).GetField("temperatureMax", flags);
		if (tempField == null)
		{
			throw new System.NullReferenceException("Temperature field was null!");
		}
		return tempField;
	}

	private FieldInfo GetLastWeatherCalculationMonth()
	{
		BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
		FieldInfo monthField = typeof(WeatherController).GetField("lastWeatherCalculationMonth", flags);
		if (monthField == null)
		{
			throw new System.NullReferenceException("Last weather calculation month was null!");
		}
		return monthField;
	}
}
