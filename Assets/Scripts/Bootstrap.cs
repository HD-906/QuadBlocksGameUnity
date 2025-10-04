using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap I 
    {  
        get; 
        private set; 
    }
    [SerializeField] public GameConfig config;

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
