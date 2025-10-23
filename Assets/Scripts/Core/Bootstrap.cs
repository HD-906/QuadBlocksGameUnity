using UnityEngine;

public enum MenuPage
{
    Root, SinglePlayer, MultiPlayer, Config, Drilling
}

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap I { get; private set; }
    public MenuPage nextMenuPage = MenuPage.Root;

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
