using UnityEngine;
[CreateAssetMenu(fileName = "TimeSettings", menuName = "Settings/Time Settings")]
public class TimeSettings : ScriptableObject
{
	public float timeMultiplier = 2000.0f;
	public float startHour = 12.0f;
	public float sunriseHour = 6.0f;
	public float sunsetHour = 18.0f;

}
