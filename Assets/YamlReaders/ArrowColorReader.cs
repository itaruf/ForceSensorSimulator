using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class ArrowColorReader : MonoBehaviour
{
    // Save the color data
    public class ColorConfig
    {
        // attributes must match the attributes from the yaml file
        public List<ColorData> color_low_magnitude { get; set; }
        public List<ColorData> color_high_magnitude { get; set; }
    }

    private ColorConfig colors;

    // Custom actions to trigger from the inspector
    [CustomEditor(typeof(ArrowColorReader))]
    public class ArrowColorReaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ArrowColorReader arrowColorReader = (ArrowColorReader) target;

            if (GUILayout.Button("Load Arrow Colors Data from YAML File"))
            {
                arrowColorReader.LoadColors();
            }

            if (GUILayout.Button("Save Arrow Colors Data to YAML File"))
            {
                arrowColorReader.SaveColors(arrowColorReader.colors);
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
        colors = new ColorConfig();
        colors.color_low_magnitude = new List<ColorData>();
        colors.color_high_magnitude = new List<ColorData>();
    }

    [ExecuteInEditMode]
    private void LoadColors()
    {
        string yamlPath = Path.Combine(Application.streamingAssetsPath, "arrow-color-settings.yaml");

        if (File.Exists(yamlPath))
        {
            try
            {
                Debug.Log($"Load file: {yamlPath}");

                // Get all the content within the file
                string yamlContent = File.ReadAllText(yamlPath);

                // Retrieve the data within
                var deserializer = new DeserializerBuilder().Build();
                colors = deserializer.Deserialize<ColorConfig>(yamlContent);

                Color colorLowMagnitude = new Color(colors.color_low_magnitude[0].r, colors.color_low_magnitude[0].g, colors.color_low_magnitude[0].b, colors.color_low_magnitude[0].a);
                Color colorHighMagnitude = new Color(colors.color_high_magnitude[0].r, colors.color_high_magnitude[0].g, colors.color_high_magnitude[0].b, colors.color_low_magnitude[0].a);

                if (ArrowForceVisualizerManager.instance)
                {

                    ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude = colorLowMagnitude;
                    ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude = colorHighMagnitude;

                    ArrowForceVisualizerManager.instance.eDI_ArrowLowMagnitudeColor.TriggerEvent(colorLowMagnitude);
                    ArrowForceVisualizerManager.instance.eDI_ArrowHighMagnitudeColor.TriggerEvent(colorHighMagnitude);

                    EditorUtility.SetDirty(ArrowForceVisualizerManager.instance);
                }
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
    private void SaveColors(ColorConfig colors)
    {
        if (colors == null)
        {
            colors = new ColorConfig();
            colors.color_low_magnitude = new List<ColorData>();
            colors.color_high_magnitude = new List<ColorData>();
        }
        else
        {
            colors.color_low_magnitude.Clear();
            colors.color_high_magnitude.Clear();
        }

        if (ArrowForceVisualizerManager.instance)
        {
            // Retrieve the necessary arrow color data to save
            ColorData colorLowMagnitude = new ColorData(
                 ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.ToString(),
                 ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.r,
                 ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.g,
                 ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.b,
                 ArrowForceVisualizerManager.instance.ArrowColorLowMagnitude.a);

            ColorData colorHighMagnitude = new ColorData(
                 ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.ToString(),
                 ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.r,
                 ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.g,
                 ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.b,
                 ArrowForceVisualizerManager.instance.ArrowColorHighMagnitude.a);

            // Build the save package with the new data
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            string yamlPath = Path.Combine(Application.streamingAssetsPath, "arrow-color-settings.yaml");

            colors.color_low_magnitude.Add(colorLowMagnitude);
            colors.color_high_magnitude.Add(colorHighMagnitude);

            string yamlContent = serializer.Serialize(colors);

            try
            {
                File.WriteAllText(yamlPath, yamlContent);
                Debug.Log("Arrow Colors saved successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving Arrow Colors: {ex.Message}");
            }
        }
    }
}