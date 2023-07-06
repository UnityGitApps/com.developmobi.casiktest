using UnityEngine;

public static class MScreen
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex > 0)
        {
            return;
        }

        Object.Instantiate(Resources.Load("loading-screen"));
    }
}
