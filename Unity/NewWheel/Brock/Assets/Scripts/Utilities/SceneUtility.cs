using UnityEngine.SceneManagement;

public static class SceneUtility
{
    public static void LoadShopScene()
    {
        SceneManager.LoadScene("Shop");
    }

    public static void LoadWaveScene()
    {
        SceneManager.LoadScene("Wave");
    }
}