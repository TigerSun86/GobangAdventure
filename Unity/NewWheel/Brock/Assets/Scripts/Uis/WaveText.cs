using UnityEngine;

public class WaveText : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;

    void Update()
    {
        text.text = "Wave: " + WaveManager.Instance.currentWave;
    }
}
