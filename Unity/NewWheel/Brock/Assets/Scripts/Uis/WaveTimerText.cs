using UnityEngine;

public class WaveTimerText : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;

    void Update()
    {
        text.text = WaveManager.Instance.currentWaveTime.ToString();
        if (WaveManager.Instance.isEarlyComplete)
        {
            text.text += " (Boss Defeated)";
        }

        if (WaveManager.Instance.currentWaveTime <= 10)
        {
            text.color = Color.red;
        }
        else if (WaveManager.Instance.currentWaveTime <= 30)
        {
            text.color = Color.yellow;
        }
        else
        {
            text.color = Color.white;
        }
    }
}
