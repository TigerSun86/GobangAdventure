using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    private int callerCount = 0;

    public void Pause()
    {
        Time.timeScale = 0;
        callerCount++;
    }

    public void Unpause()
    {
        callerCount--;
        if (callerCount == 0)
        {
            Time.timeScale = 1f;
        }
    }
}
