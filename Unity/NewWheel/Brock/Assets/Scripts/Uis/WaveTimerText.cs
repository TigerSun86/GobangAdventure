using UnityEngine;

public class WaveTimerText : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;

    void Update()
    {
        text.text = WaveManager.Instance.currentWaveTime.ToString();
    }
}
