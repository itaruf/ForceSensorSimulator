using UnityEngine;

public class ForceSensorManager : MonoBehaviour
{
    public static ForceSensorManager instance;

    public float updateDelay = 1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}