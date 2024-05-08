using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Define classes that will match the structure of the YAML data file
public class YamlReader
{
    // Settings specific to force sensor game objects
    public class ForceSensorGOSettings
    {
        public string id;
        public string addressable_key;
        public string parent;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string material_key;
    }

    // Settings specific to arrows' color based on magnitude
    public class ColorSettings
    {
        public ColorData color_low_magnitude { get; set; }
        public ColorData color_high_magnitude { get; set; }
    }

    // Setting specific to force 
    public class ForceSettings
    {
        public float force_threshold { get; set; }
    }

    // Main configuration container that includes all individual settings groups
    public class SceneConfiguration
    {
        public List<ForceSensorGOSettings> force_sensor_game_objects_settings;
        public ColorSettings force_sensor_color_settings;
        public ForceSettings force_sensor_force_settings;
    }

    [ExecuteInEditMode]
    public static void LoadData()
    {
        string yamlPath = Path.Combine(Application.streamingAssetsPath, "scene-config.yaml");

        if (File.Exists(yamlPath))
        {
            try
            {
                // Get all the content within the file
                string yamlContent = File.ReadAllText(yamlPath);

                // Retrieve the data within
                var deserializer = new DeserializerBuilder().Build();
                SceneConfiguration config = deserializer.Deserialize<SceneConfiguration>(yamlContent);

                // Parse the deserialized data and apply the data to game objects, etc.
                Parse(config);
            }
            catch (IOException e)
            {
                Debug.LogError("An I/O error occurred while reading the file: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError("An error occurred while reading the file: " + e.Message);
            }
        }
    }

    [ExecuteInEditMode]
    public static void SaveData()
    {
        SceneConfiguration package = new SceneConfiguration();

        if (ArrowForceVisualizerManager.Instance)
        {
            // Save game objects
            var forceSensors = GameObject.FindObjectsOfType<ForceSensor>();

            // Save color settings
            ColorData colorLowMagnitude = ScriptableObject.CreateInstance<ColorData>();

            colorLowMagnitude.name = ArrowForceVisualizerManager.Instance.ArrowColorLowMagnitude.ToString();
            colorLowMagnitude.r = ArrowForceVisualizerManager.Instance.ArrowColorLowMagnitude.r;
            colorLowMagnitude.g = ArrowForceVisualizerManager.Instance.ArrowColorLowMagnitude.g;
            colorLowMagnitude.b = ArrowForceVisualizerManager.Instance.ArrowColorLowMagnitude.b;
            colorLowMagnitude.a = ArrowForceVisualizerManager.Instance.ArrowColorLowMagnitude.a;

            ColorData colorHighMagnitude = ScriptableObject.CreateInstance<ColorData>();

            colorHighMagnitude.name = ArrowForceVisualizerManager.Instance.ArrowColorHighMagnitude.ToString();
            colorHighMagnitude.r = ArrowForceVisualizerManager.Instance.ArrowColorHighMagnitude.r;
            colorHighMagnitude.g = ArrowForceVisualizerManager.Instance.ArrowColorHighMagnitude.g;
            colorHighMagnitude.b = ArrowForceVisualizerManager.Instance.ArrowColorHighMagnitude.b;
            colorHighMagnitude.a = ArrowForceVisualizerManager.Instance.ArrowColorHighMagnitude.a;

            package.force_sensor_color_settings.color_low_magnitude = ScriptableObject.CreateInstance<ColorData>();
            package.force_sensor_color_settings.color_high_magnitude = ScriptableObject.CreateInstance<ColorData>();

            package.force_sensor_color_settings.color_high_magnitude = colorLowMagnitude;
            package.force_sensor_color_settings.color_high_magnitude = colorHighMagnitude;

            // Save force settings
            package.force_sensor_force_settings.force_threshold = ArrowForceVisualizerManager.Instance.ArrowMagnitudeThreshold;

            // Build the save package with the new data
            string yamlPath = Path.Combine(Application.streamingAssetsPath, "scene-config.yaml");

            try
            {
                var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
                string yamlContent = serializer.Serialize(package);

                File.WriteAllText(yamlPath, yamlContent);

                Debug.Log("Arrow settings saved successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving Arrow settings: {ex.Message}");
            }
        }
    }

    // Apply parsed configuration settings to respective game objects, etc.
    private static void Parse(SceneConfiguration config)
    {
        // Force Sensor game objects specific settings
        foreach (ForceSensorGOSettings settings in config.force_sensor_game_objects_settings)
        {
            // Search for the corresponding game object using a key identifier
            GameObject gameObject = GameObject.Find(settings.id);
            if (gameObject == null)
            {
                // Asynchronously load the game object if not already present in the scene
                Addressables.LoadAssetAsync<GameObject>(settings.addressable_key).Completed += (asyncOperationHandle) =>
                {
                    if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        gameObject = GameObject.Instantiate(asyncOperationHandle.Result);
                        ApplySettingsToGameObject(gameObject, settings);

                    }
                    else
                        Debug.LogError($"Error loading {settings.id}");
                };
            }
            else
                ApplySettingsToGameObject(gameObject, settings);
        }

        // Force Sensor global settings
        if (ArrowForceVisualizerManager.Instance == null)
        {
            Addressables.LoadAssetAsync<GameObject>("arrow_manager").Completed += (asyncOperationHandle) =>
            {
                if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject gameObject = GameObject.Instantiate(asyncOperationHandle.Result);
                    if (gameObject.TryGetComponent(out ArrowForceVisualizerManager arrowManager))
                    {
                        ArrowForceVisualizerManager.Instance = arrowManager;

                        // Hack: remove the "clone" word at the end 
                        gameObject.name = asyncOperationHandle.Result.name;

                        // Apply color settings + force settings
                        ApplyColorSettings(ArrowForceVisualizerManager.Instance, config.force_sensor_color_settings);
                        ApplyForceSettings(ArrowForceVisualizerManager.Instance, config.force_sensor_force_settings);

                        if (SceneSettingsManager.Instance)
                            SceneSettingsManager.Instance.OnArrowManagerStart?.Invoke();
                    }
                }
            };
        }

        else
        {
            // color settings + force settings
            ApplyColorSettings(ArrowForceVisualizerManager.Instance, config.force_sensor_color_settings);
            ApplyForceSettings(ArrowForceVisualizerManager.Instance, config.force_sensor_force_settings);
        }
    }

    // Apply the new params to the targeted game object
    private static void ApplySettingsToGameObject(GameObject gameObject, ForceSensorGOSettings settings)
    {
        GameObject parent = GameObject.Find(settings.parent);
        if (parent != null)
            gameObject.transform.SetParent(parent.transform);

        gameObject.name = settings.id;
        gameObject.transform.position = settings.position;
        gameObject.transform.rotation = Quaternion.Euler(settings.rotation);
        gameObject.transform.localScale = settings.scale;
    }

    // Apply the color data to the arrow manager
    private static void ApplyColorSettings(ArrowForceVisualizerManager manager, ColorSettings settings)
    {
        Color colorLowMagnitude = new
            (settings.color_low_magnitude.r,
            settings.color_low_magnitude.g,
            settings.color_low_magnitude.b,
            settings.color_low_magnitude.a);

        Color colorHighMagnitude = new
            (settings.color_high_magnitude.r,
            settings.color_high_magnitude.g,
            settings.color_high_magnitude.b,
            settings.color_high_magnitude.a);

        manager.ArrowColorLowMagnitude = colorLowMagnitude;
        manager.ArrowColorHighMagnitude = colorHighMagnitude;

        manager.eDI_ArrowLowMagnitudeColor.TriggerEvent(colorLowMagnitude);
        manager.eDI_ArrowHighMagnitudeColor.TriggerEvent(colorHighMagnitude);
    }

    // Apply the force data to the arrow manager
    private static void ApplyForceSettings(ArrowForceVisualizerManager manager, ForceSettings settings)
    {
        manager.ArrowMagnitudeThreshold = settings.force_threshold;
        manager.eDI_ArrowMagnitudeThreshold.TriggerEvent(settings.force_threshold.ToString());
    }
}