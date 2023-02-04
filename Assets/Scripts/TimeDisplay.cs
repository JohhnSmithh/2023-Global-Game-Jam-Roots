using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // display current time in seconds rounded to two decimal places
        text.SetText("Time: " + Mathf.Round(100 * GameManager.instance.getGameTime())/100);
    }
}
