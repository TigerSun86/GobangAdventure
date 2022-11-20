using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenuManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    private GamePause gamePause;

    // Start is called before the first frame update
    void Start()
    {
        gamePause = GetComponent<GamePause>();
        Level playerLevel = Manager.instance.PlayerLevel;
        playerLevel.OnLevelUp.AddListener((level) => OpenMenu());
    }

    public void OpenMenu()
    {
        gamePause.Pause();
        panel.SetActive(true);
    }

    public void CloseMenu()
    {
        gamePause.Unpause();
        panel.SetActive(false);
    }
}
