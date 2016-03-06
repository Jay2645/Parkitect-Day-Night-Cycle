using UnityEngine;

class Lamp : MonoBehaviour
{
	public float lightIntensity = 1.0f;
	public float fadeInSpeed = 1.0f;
	public float daytimeAmount = 0.1f;
	public float nighttimeAmount = 1.0f;
	public float blinkInChance = 0.01f;

	private Light lamp;
	private float dayTimeModifier = 0.0f;
	private float targetIntensity = 0.0f;

	private void Start()
	{
		GameObject lampGO = new GameObject("Lamp Light");
		lampGO.transform.parent = transform;
		lampGO.transform.localPosition = Vector3.up;
		lamp = lampGO.AddComponent<Light>();
		lamp.type = LightType.Point;
		targetIntensity = 0.0f;
		lamp.intensity = targetIntensity;
		lamp.range = 25.0f;
		lamp.color = Color.Lerp(Color.yellow, Color.white, 0.5f);
	}

	public void Update()
	{
		if (targetIntensity < lightIntensity)
		{
			targetIntensity += Time.deltaTime / fadeInSpeed;
			targetIntensity = Mathf.Clamp01(targetIntensity);
		}
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

	public void CleanUp()
	{
		if (lamp != null)
		{
			Destroy(lamp.gameObject);
		}
	}
}
