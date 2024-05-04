using UnityEngine;

[CreateAssetMenu(fileName = "Force Sensor Data", menuName = "Sensor Data/Force Sensor Data", order = 0)]
public class ForceSensorData : ScriptableObject
{
    public string sensorName;
    public Vector3 force;
    public Vector3 position;
    public Quaternion orientation;
}