using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = (T)this;
            DontDestroyOnLoad(this);
        }
    }
}