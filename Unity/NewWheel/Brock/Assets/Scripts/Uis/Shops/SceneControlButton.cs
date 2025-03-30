using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneControlButton : MonoBehaviour
{
    enum TargetScene
    {
        Wave,
        Shop,
    }

    [SerializeField] TargetScene targetScene;
    Button button;

    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        switch (targetScene)
        {
            case TargetScene.Wave:
                button.onClick.AddListener(() => SceneUtility.LoadWaveScene());
                break;
            case TargetScene.Shop:
                button.onClick.AddListener(() => SceneUtility.LoadShopScene());
                break;
            default:
                throw new System.Exception("Invalid target scene");
        }
    }
}