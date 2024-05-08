using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class YamlReader : MonoBehaviour
{
    public class ForceSensorSettings
    {
        public string id;
        public string addressable_key;
        public string parent;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string material_key;
    }

    // Save the color data
    public class ColorSettings
    {
        // attributes must match the attributes from the yaml file
        public ColorData color_low_magnitude { get; set; }
        public ColorData color_high_magnitude { get; set; }
    }

    public class ForceSettings
    {
        public float force_threshold { get; set; }
    }

    public class SceneConfiguration
    {
        public List<ForceSensorSettings> force_sensor_game_objects_settings;
        public ColorSettings force_sensor_color_settings;
        public ForceSettings force_sensor_force_settings;
    }

    /*private ColorSettings colors;*/

    // Custom actions to trigger from the inspector
    [CustomEditor(typeof(YamlReader))]
    public class ArrowColorReaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            YamlReader arrowColorReader = (YamlReader)target;

            if (GUILayout.Button("Load Arrow Data from YAML File"))
            {
                arrowColorReader.LoadData();
            }

            if (GUILayout.Button("Save Arrow Data to YAML File"))
            {
                arrowColorReader.SaveData(null/*arrowColorReader.colors*/);
            }
        }
    }

    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(gameObject);
    }

    private void Initialize()
    {
        /*colors = new ColorSettings();*/
    }

    [ExecuteInEditMode]
    private void LoadData()
    {
        string yamlPath = Path.Combine(Application.streamingAssetsPath, "scene-config.yaml");

        if (File.Exists(yamlPath))
        {
            try
            {
                Debug.Log($"Load file: {yamlPath}");

                // Get all the content within the file
                string yamlContent = File.ReadAllText(yamlPath);

                // Retrieve the data within
                var deserializer = new DeserializerBuilder().Build();
                SceneConfiguration config = deserializer.Deserialize<SceneConfiguration>(yamlContent);

                // Apply the data to the game objects, etc.
                ApplyConfiguration(config);
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
    private void SaveData(ColorSettings colors)
    {
        colors ??= new ColorSettings();

        if (ArrowForceVisualizerManager.instance)
        {
            // Retrieve the necessary arrow color data to save
            ColorData colorLowMagnitude = ScriptableObject.CreateInstance<ColorData>();

            colorLowMagnitude.name = ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.ToString();
            colorLowMagnitude.r = ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.r;
            colorLowMagnitude.g = ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.g;
            colorLowMagnitude.b = ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.b;
            colorLowMagnitude.a = ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.a;

            ColorData colorHighMagnitude = ScriptableObject.CreateInstance<ColorData>();

            colorHighMagnitude.name = ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.ToString();
            colorHighMagnitude.r = ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.r;
            colorHighMagnitude.g = ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.g;
            colorHighMagnitude.b = ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.b;
            colorHighMagnitude.a = ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.a;

            colors.color_low_magnitude = ScriptableObject.CreateInstance<ColorData>();
            colors.color_high_magnitude = ScriptableObject.CreateInstance<ColorData>();

            colors.color_high_magnitude = colorLowMagnitude;
            colors.color_high_magnitude = colorHighMagnitude;

            // Build the save package with the new data
            string yamlPath = Path.Combine(Application.streamingAssetsPath, "scene-config.yaml");

            try
            {
                var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
                string yamlContent = serializer.Serialize(colors);

                File.WriteAllText(yamlPath, yamlContent);

                Debug.Log("Arrow settings saved successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving Arrow settings: {ex.Message}");
            }
        }
    }

    private void ApplyConfiguration(SceneConfiguration config)
    {
        // Force Sensor game objects specific settings
        foreach (ForceSensorSettings settings in config.force_sensor_game_objects_settings)
        {
            GameObject game_object = GameObject.Find(settings.id);
            if (game_object == null)
            {
                Addressables.LoadAssetAsync<GameObject>(settings.addressable_key).Completed += (asyncOperationHandle) =>
                {
                    if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        game_object = Instantiate(asyncOperationHandle.Result);

                        GameObject parent = GameObject.Find(settings.parent);
                        if (parent != null)
                            game_object.transform.parent = parent.transform;

                        // Apply the new params
                        game_object.name = settings.id;
                        game_object.transform.position = settings.position;
                        game_object.transform.rotation = Quaternion.Euler(settings.rotation);
                        game_object.transform.localScale = settings.scale;
                    }
                    else
                    {
                        Debug.LogError($"Error loading {settings.id}");
                    }
                };
            }

            else
            {
                // Apply the new params
                game_object.name = settings.id;
                game_object.transform.position = settings.position;
                game_object.transform.rotation = Quaternion.Euler(settings.rotation);
                game_object.transform.localScale = settings.scale;

            }
        }

        // Force Sensor global settings
        if (ArrowForceVisualizerManager.instance)
        {
            // color settings
            Color colorLowMagnitude = new
            (config.force_sensor_color_settings.color_low_magnitude.r,
            config.force_sensor_color_settings.color_low_magnitude.g,
            config.force_sensor_color_settings.color_low_magnitude.b,
            config.force_sensor_color_settings.color_low_magnitude.a);

            Color colorHighMagnitude = new
                (config.force_sensor_color_settings.color_high_magnitude.r,
                config.force_sensor_color_settings.color_high_magnitude.g,
                config.force_sensor_color_settings.color_high_magnitude.b,
                config.force_sensor_color_settings.color_high_magnitude.a);

            ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude = colorLowMagnitude;
            ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude = colorHighMagnitude;

            ArrowForceVisualizerManager.instance.eDI_ArrowLowMagnitudeColor.TriggerEvent(colorLowMagnitude);
            ArrowForceVisualizerManager.instance.eDI_ArrowHighMagnitudeColor.TriggerEvent(colorHighMagnitude);

            // force settings
            ArrowForceVisualizerManager.instance.ArrowMagnitudeThreshold = config.force_sensor_force_settings.force_threshold;

            ArrowForceVisualizerManager.instance.eDI_ArrowMagnitudeThreshold.TriggerEvent(config.force_sensor_force_settings.force_threshold.ToString());

           EditorUtility.SetDirty(ArrowForceVisualizerManager.instance);
        }
    }
}