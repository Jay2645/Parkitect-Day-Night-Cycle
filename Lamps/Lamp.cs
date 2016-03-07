using UnityEngine;

class Lamp : MonoBehaviour
{
	/// <summary>
	/// The maximum intensity of our lamp light.
	/// </summary>
	public float lightIntensity = 1.0f;
	/// <summary>
	/// How quickly the lamp "fades in" when placed at night.
	/// </summary>
	public float fadeInSpeed = 1.0f;
	/// <summary>
	/// How bright the lamp is in the daytime.
	/// </summary>
	public float daytimeAmount = 0.1f;
	/// <summary>
	/// How bright the lamp is at night.
	/// </summary>
	public float nighttimeAmount = 1.0f;
	/// <summary>
	/// The lamp's chance to turn on early at sunrise/sunset.
	/// Lower values means the lights will turn on one at a time.
	/// </summary>
	public float blinkInChance = 0.01f;
	/// <summary>
	/// How far away our lamp casts light.
	/// </summary>
	public float range = 25.0f;

	private Light lamp;
	private float dayTimeModifier = 0.0f;
	private float targetIntensity = 0.0f;

	private void Start()
	{
		// Create a child GameObject to hold our light
		GameObject lampGO = new GameObject("Lamp Light");
		lampGO.transform.parent = transform;
		// Our light is offset slightly from the ground to cast a light "downwards"
		lampGO.transform.localPosition = Vector3.up;

		// Create our actual light
		lamp = lampGO.AddComponent<Light>();
		lamp.type = LightType.Point;
		targetIntensity = 0.0f;
		lamp.intensity = targetIntensity;
		lamp.range = range;
		lamp.shadows = LightShadows.None;
		// Initialize it to a whitish-yellow color
		lamp.color = Color.Lerp(Color.yellow, Color.white, 0.5f);
	}

	private void Update()
	{
		// Initialize low and slowly go to maximum
		if (targetIntensity < lightIntensity)
		{
			targetIntensity += Time.deltaTime / fadeInSpeed;
			targetIntensity = Mathf.Clamp01(targetIntensity);
		}

		// Check what time of day it is to see how bright our light should be
		DayStatus dayStatus = DayNight.dayStatus;
		if (dayStatus == DayStatus.Daytime)
		{
			if (dayTimeModifier > daytimeAmount)
			{
				dayTimeModifier = daytimeAmount;
			}
		}
		else if (dayStatus == DayStatus.Nighttime)
		{
			if (dayTimeModifier < nighttimeAmount)
			{
				dayTimeModifier = nighttimeAmount;
			}
		}
		else if (dayStatus == DayStatus.Sunrise)
		{
			if (dayTimeModifier > daytimeAmount && Random.value <= blinkInChance)
			{
				dayTimeModifier = daytimeAmount;
			}
		}
		else if (dayStatus == DayStatus.Sunset)
		{
			if (dayTimeModifier < nighttimeAmount && Random.value <= blinkInChance)
			{
				dayTimeModifier = nighttimeAmount;
			}
		}

		lamp.intensity = dayTimeModifier * targetIntensity;
	}

	/// <summary>
	/// Cleans up the lamp light.
	/// </summary>
	public void CleanUp()
	{
		if (lamp != null)
		{
			Destroy(lamp.gameObject);
		}
	}
}
