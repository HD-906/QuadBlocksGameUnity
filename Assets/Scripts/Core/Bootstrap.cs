using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap I 
    {  
        get; 
        private set; 
    }
    public GameConfig config;

    void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }
}
