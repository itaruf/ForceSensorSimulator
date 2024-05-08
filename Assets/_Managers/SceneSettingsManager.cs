using System.Drawing.Printing;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using System;

public class SceneSettingsManager : MonoBehaviour
{
    private static SceneSettingsManager _instance;
    public static SceneSettingsManager Instance { get => _instance; set => _instance = value; }

    public bool loadYamlOnStart = true;

    // Events
    public Action OnArrowManagerStart;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Custom actions to trigger from the inspector
    [CustomEditor(typeof(SceneSettingsManager))]
    public class SceneSettingsManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Load Scene Data from YAML File"))
            {
                YamlReader.LoadData();
            }

            /*if (GUILayout.Button("Save Scene Data to YAML File"))
            {
                YamlReader.SaveData();
            }*/
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loadYamlOnStart)
            YamlReader.LoadData();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}