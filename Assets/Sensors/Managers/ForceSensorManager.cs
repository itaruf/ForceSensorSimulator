using UnityEngine;

public class ForceSensorManager : MonoBehaviour
{
    private static ForceSensorManager _instance;
    public static ForceSensorManager Instance { get => _instance; set => _instance = value; }

    public float updateDelay = 1f;

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
}