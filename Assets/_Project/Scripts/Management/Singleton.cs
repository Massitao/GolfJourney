using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{

    private static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (_instance == this)
        {
            _instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}