using UnityEngine;
using Unity.Simulation.Sensors;
using System.Collections;

// Class that represents a force sensor mimicking human senses
public class ForceSensor : MonoBehaviour
{
    // Reference to the external library simulating force information
    private ForceSensorSimulator _forceSensorSimulator;

    // Data structure to store the force information provided
    private ForceSensorData _forceSensorData;

    // Update old data with the new force information provided
    private Coroutine _updateSensorWithForceDataCoroutine;

    // Property accessors
    public ForceSensorSimulator ForceSensorSimulator { get => _forceSensorSimulator; }
    public ForceSensorData ForceSensorData { get => _forceSensorData; }

    private void Start()
    {
        _forceSensorSimulator = new ForceSensorSimulator(gameObject.name);

        _forceSensorData = new ForceSensorData();
        _forceSensorData.sensorName = name;

        _updateSensorWithForceDataCoroutine = StartCoroutine(UpdateSensorWithForceData());
    }

    private IEnumerator UpdateSensorWithForceData()
    {
        while (true)
        {
            ForceSensorSimulator.ForceSensorDataSimulation();

            Vector3 simulatedPosition = new Vector3
                (ForceSensorSimulator.Position.X, ForceSensorSimulator.Position.Y, ForceSensorSimulator.Position.Z);

            Vector3 simulatedForce = new Vector3(ForceSensorSimulator.Force.X, ForceSensorSimulator.Force.Y, ForceSensorSimulator.Force.Z);

            Quaternion simulatedOrientation = new Quaternion(ForceSensorSimulator.Orientation.X, ForceSensorSimulator.Orientation.Y, ForceSensorSimulator.Orientation.Z,
           ForceSensorSimulator.Orientation.W);
      
            _forceSensorData.force = simulatedForce;
            _forceSensorData.position = simulatedPosition;
            _forceSensorData.orientation = simulatedOrientation;

            yield return new WaitForSeconds(ForceSensorManager.instance.updateDelay);
        }
    }
}