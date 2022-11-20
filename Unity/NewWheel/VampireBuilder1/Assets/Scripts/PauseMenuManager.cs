using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    private GamePause gamePause;

    // Start is called before the first frame update
    void Start()
    {
        gamePause = GetComponent<GamePause>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panel.activeInHierarchy)
            {
                gamePause.Unpause();
                panel.SetActive(false);
            }
            else
            {
                gamePause.Pause();
                panel.SetActive(true);
            }
        }
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
