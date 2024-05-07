using System;
using UnityEngine;

// Manager repository that contains prefabs to use as a references (instantiate, etc.)
public class PrefabManager : MonoBehaviour
{
    private static PrefabManager _instance;
    public static PrefabManager instance
    {
        get
        {
            if (_instance == null)
            {
                // Search for existing instance in the scene
                _instance = FindObjectOfType<PrefabManager>();

                if (_instance == null)
                {
                    // Create the instance in-game
                    _instance = CreateInstance();
                }
            }
            return _instance;
        }
    }

    // Prefabs
    public GameObject arrowForceVisualizerManager;

    private void Awake()
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

    public static PrefabManager CreateInstance()
    {
        throw new NotImplementedException();
    }
}