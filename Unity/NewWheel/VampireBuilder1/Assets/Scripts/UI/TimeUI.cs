using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimeUI : MonoBehaviour
{
    [SerializeField] FloatVariable timeInSecond;

    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        int minutes = (int)(timeInSecond.value / 60);
        int seconds = (int)(timeInSecond.value % 60);
        text.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
