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
        if (ArrowForceVisualizerManager.Instance == null)
            SceneSettingsManager.Instance.OnArrowManagerStart += Initialize;

        else
            Initialize();
    }

    private void Initialize()
    {
        // Instantiate a new force sensor simulator associated to this force sensor
        _forceSensorSimulator = new ForceSensorSimulator(gameObject.name);

        // Instantiate the data structure to store the provided information
        _forceSensorData = ScriptableObject.CreateInstance<ForceSensorData>();
        _forceSensorData.sensorName = name;

        // Start the simulation and keep it active
        _updateSensorWithForceDataCoroutine = StartCoroutine(UpdateSensorWithForceData());
    }

    private IEnumerator UpdateSensorWithForceData()
    {
        while (true)
        {
            // Get the force data from the external library
            ForceSensorSimulator.ForceSensorDataSimulation();

            Vector3 simulatedPosition = new(ForceSensorSimulator.Position.X, ForceSensorSimulator.Position.Y, ForceSensorSimulator.Position.Z);
            Vector3 simulatedForce = new(ForceSensorSimulator.Force.X, ForceSensorSimulator.Force.Y, ForceSensorSimulator.Force.Z);
            Quaternion simulatedOrientation = new(ForceSensorSimulator.Orientation.X, ForceSensorSimulator.Orientation.Y, ForceSensorSimulator.Orientation.Z, ForceSensorSimulator.Orientation.W);
      
            // Store the data
            _forceSensorData.force = simulatedForce;
            _forceSensorData.position = simulatedPosition;
            _forceSensorData.orientation = simulatedOrientation;

            // Update the position and rotation based on those data
            transform.SetPositionAndRotation(simulatedPosition, simulatedOrientation);

            yield return new WaitForSeconds(ArrowForceVisualizerManager.Instance.UpdateDelay);
        }
    }
}