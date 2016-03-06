using UnityEngine;
public enum DayStatus
{
	Sunrise,
	Daytime,
	Sunset,
	Nighttime
}

public class DayNight : MonoBehaviour
{
	public float dayCycleInMinutes = 5f;
	[Range(0, 12)]
	public int sunriseStartHour = 5;
	[Range(0, 12)]
	public int sunriseEndHour = 8;
	[Range(12, 24)]
	public int sunsetStartHour = 16;
	[Range(12, 24)]
	public int sunsetEndHour = 18;

	private float sunriseDiff = 0.0f;
	private float sunsetDiff = 0.0f;
	private float startIntensity = 1.0f;
	private Vector3 localRotation = Vector3.zero;
	private Quaternion initialSunAngle;
	private Light sun;

	public int hour = 0;
	public int minute = 0;
	public float second = 0.0f;
	private float sunShaftIntensity;

	public static DayStatus dayStatus
	{
		get;
		private set;
	}
	public static float dayPercent
	{
		get;
		private set;
	}


	private void Start()
	{
		sun = gameObject.GetComponent<Light>();
		initialSunAngle = transform.rotation;
		startIntensity = sun.intensity;
		localRotation = transform.localRotation.eulerAngles;
		sunriseDiff = sunriseEndHour - sunriseStartHour;
		sunsetDiff = sunsetEndHour - sunsetStartHour;
	}

	// Update is called once per frame
	private void Update()
	{
		RotateSun();
		DetermineDayPercent();
		DetermineTime();
		CauseSunriseSunset();
	}

	private void RotateSun()
	{
		// Rotate around local X axis
		localRotation.x += Time.deltaTime / (dayCycleInMinutes * 60.0f) * 360.0f;
		if (localRotation.x >= 360.0f)
		{
			localRotation.x -= 360.0f;
		}
		transform.localRotation = Quaternion.Euler(localRotation);
	}

	private void DetermineDayPercent()
	{
		float xRotation = localRotation.x + 90.0f;
		if (xRotation > 360.0f)
		{
			xRotation -= 360.0f;
		}
		dayPercent = xRotation / 360.0f;
	}

	private void DetermineTime()
	{
		float hourAmount = 24.0f * dayPercent;
		hour = Mathf.FloorToInt(hourAmount);
		float minuteAmount = (hourAmount - hour) * 60.0f;
		minute = Mathf.FloorToInt(minuteAmount);
		second = (minuteAmount - (float)minute) * 60.0f;
	}

	private void CauseSunriseSunset()
	{
		float time = (float)hour + ((float)minute / 60.0f);
		if (hour >= sunriseStartHour && hour < sunriseEndHour)
		{
			// Sun is rising
			dayStatus = DayStatus.Sunrise;

			float adjustedTime = time - sunriseStartHour;
			float sunrisePercent = adjustedTime / sunriseDiff;
			sun.intensity = Mathf.Lerp(0.0f, startIntensity, sunrisePercent);
		}
		else
		{
			if (hour >= sunsetStartHour && hour < sunsetEndHour)
			{
				// Sun is setting
				dayStatus = DayStatus.Sunset;

				float adjustedTime = time - sunsetStartHour;
				float sunsetPercent = adjustedTime / sunsetDiff;
				sun.intensity = Mathf.Lerp(startIntensity, 0.0f, sunsetPercent);
			}
			else if (hour >= sunriseEndHour && hour < sunsetStartHour)
			{
				// Is daytime
				dayStatus = DayStatus.Daytime;
				sun.intensity = startIntensity;

			}
			else
			{
				// Is nighttime
				dayStatus = DayStatus.Nighttime;
				sun.intensity = 0.0f;
			}
		}
	}

	public void Reset()
	{
		transform.rotation = initialSunAngle;
	}
}
