﻿using UnityEngine;

namespace DayNightCycle
{
	public enum DayStatus
	{
		Sunrise,
		Daytime,
		Sunset,
		Nighttime
	}

	public class DayNight : MonoBehaviour
	{
		/// <summary>
		/// Called whenever the hour changes to a new hour.
		/// </summary>
		/// <param name="hour">The new hour.</param>
		public delegate void OnHourChangedHandler(int hour);
		/// <summary>
		/// Called when the day begins.
		/// </summary>
		public delegate void OnDayStartHandler();
		/// <summary>
		/// Called when sunset begins.
		/// </summary>
		public delegate void OnSunsetStartHandler();
		/// <summary>
		/// Called when night begins.
		/// </summary>
		public delegate void OnNightStartHandler();
		/// <summary>
		/// Called when sunrise begins.
		/// </summary>
		public delegate void OnSunriseStartHandler();

		/// <summary>
		/// How long an entire day/night cycle takes, in minutes
		/// </summary>
		public float dayCycleInMinutes = 5f;
		/// <summary>
		/// What hour does sunrise start?
		/// </summary>
		[Range(0, 12)]
		public int sunriseStartHour = 5;
		/// <summary>
		/// What hour does sunrise become daytime?
		/// </summary>
		[Range(0, 12)]
		public int sunriseEndHour = 8;
		/// <summary>
		/// What hour does sunset start?
		/// </summary>
		[Range(12, 24)]
		public int sunsetStartHour = 16;
		/// <summary>
		/// What hour does sunset become nighttime?
		/// </summary>
		[Range(12, 24)]
		public int sunsetEndHour = 18;

		private float sunriseDiff = 0.0f;
		private float sunsetDiff = 0.0f;
		private float startIntensity = 1.0f;
		private Vector3 localRotation = Vector3.zero;
		private Quaternion initialSunAngle;
		private Light sun;

		/// <summary>
		/// The current Day/Night cycle hour.
		/// </summary>
		public static int hour
		{
			get;
			private set;
		}
		/// <summary>
		/// The current Day/Night cycle minute.
		/// </summary>
		public static int minute
		{
			get;
			private set;
		}
		/// <summary>
		/// The current Day/Night cycle second.
		/// </summary>
		public static float second
		{
			get;
			private set;
		}

		/// <summary>
		/// What's the current status of the day?
		/// </summary>
		public static DayStatus dayStatus
		{
			get;
			private set;
		}
		/// <summary>
		/// How close to midnight is it?
		/// 0.0 is 12:00:01, while 1.0 is 11:59:59.
		/// </summary>
		public static float dayPercent
		{
			get;
			private set;
		}

		public static DayNight Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Called when the day starts.
		/// </summary>
		public event OnDayStartHandler OnDayStart;
		/// <summary>
		/// Called when sunset starts.
		/// </summary>
		public event OnSunsetStartHandler OnSunsetStart;
		/// <summary>
		/// Called when night starts.
		/// </summary>
		public event OnNightStartHandler OnNightStart;
		/// <summary>
		/// Called when sunrise starts.
		/// </summary>
		public event OnSunriseStartHandler OnSunriseStart;

		/// <summary>
		/// Called when the hour changes.
		/// </summary>
		public event OnHourChangedHandler OnHourChanged;

		private void Start()
		{
			Instance = this;
			sun = gameObject.GetComponent<Light>();
			initialSunAngle = transform.rotation;
			startIntensity = sun.intensity;
			localRotation = transform.localRotation.eulerAngles;
			sunriseDiff = sunriseEndHour - sunriseStartHour;
			sunsetDiff = sunsetEndHour - sunsetStartHour;
		}

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
			int lastHour = hour;
			hour = Mathf.FloorToInt(hourAmount);
			float minuteAmount = (hourAmount - hour) * 60.0f;
			minute = Mathf.FloorToInt(minuteAmount);
			second = (minuteAmount - (float)minute) * 60.0f;

			if (lastHour != hour && OnHourChanged != null)
			{
				OnHourChanged(hour);
			}
		}

		private void CauseSunriseSunset()
		{
			float time = (float)hour + ((float)minute / 60.0f);
			// Cache DayStatus so we can check to see if it changed
			DayStatus oldStatus = dayStatus;
			if (hour >= sunriseStartHour && hour < sunriseEndHour)
			{
				// Sun is rising
				dayStatus = DayStatus.Sunrise;

				float adjustedTime = time - sunriseStartHour;
				float sunrisePercent = adjustedTime / sunriseDiff;
				sun.intensity = Mathf.Lerp(0.0f, startIntensity, sunrisePercent);

				if (oldStatus != dayStatus && OnSunriseStart != null)
				{
					OnSunriseStart();
				}
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

					if (oldStatus != dayStatus && OnSunsetStart != null)
					{
						OnSunsetStart();
					}
				}
				else if (hour >= sunriseEndHour && hour < sunsetStartHour)
				{
					// Is daytime
					dayStatus = DayStatus.Daytime;
					sun.intensity = startIntensity;

					if (oldStatus != dayStatus && OnDayStart != null)
					{
						OnDayStart();
					}
				}
				else
				{
					// Is nighttime
					dayStatus = DayStatus.Nighttime;
					sun.intensity = 0.0f;

					if (oldStatus != dayStatus && OnNightStart != null)
					{
						OnNightStart();
					}
				}
			}
		}

		/// <summary>
		/// Turns off the Day/Night cycle and resets the sun back to normal.
		/// </summary>
		public void Reset()
		{
			transform.rotation = initialSunAngle;
		}
	}
}