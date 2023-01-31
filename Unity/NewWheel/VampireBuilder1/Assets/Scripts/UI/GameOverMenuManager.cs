using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        if (panel.activeInHierarchy)
        {
            return;
        }

        panel.SetActive(value: true);
        Button button = panel.GetComponentInChildren<Button>();
        EventSystem.current.SetSelectedGameObject(button.gameObject);

        gamePause.Pause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        gamePause.Unpause();
    }
}
