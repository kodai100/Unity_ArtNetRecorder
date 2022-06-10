using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
        
    protected static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
                
            _instance = (T) FindObjectOfType(typeof(T));

            if (_instance == null)
            {
                var go = new GameObject ($"__{nameof(T)}");
                _instance = go.AddComponent<T> ();
                DontDestroyOnLoad (go);
                // go.hideFlags = HideFlags.HideInHierarchy;
            }

            return _instance;
        }
    }

}