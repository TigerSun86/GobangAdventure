using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    GamePause gamePause;

    // Start is called before the first frame update
    void Start()
    {
        gamePause = GetComponent<GamePause>();
    }

    public void OpenMenu()
    {
        panel.SetActive(value: true);
        gamePause.Pause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        gamePause.Unpause();
    }
}
