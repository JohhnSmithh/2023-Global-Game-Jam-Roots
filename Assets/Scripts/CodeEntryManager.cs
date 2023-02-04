using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CodeEntryManager : MonoBehaviour
{
    // Canvas elements
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI noteText;
    [SerializeField] private GameObject selectionIndicator;
    [SerializeField] private TextMeshProUGUI[] textDigits;

    // variables
    private int[] digits;
    private int selectedIndex;

    // Start is called before the first frame update
    void Start()
    {
        // set text for notes
        noteText.SetText(GameManager.instance.GetNotebookText());

        // default selection on first digit
        selectionIndicator.transform.position = textDigits[0].transform.position;
        selectedIndex = 0;

        // default digit initialization
        digits = new int[4] {0, 0, 0, 0} ;
    }

    // Update is called once per frame
    void Update()
    {
        // display current time in seconds rounded to two decimal places
        timeText.SetText("Time: " + Mathf.Round(100 * GameManager.instance.GetGameTime()) / 100);

        // display digits to text
        for (int i = 0; i < 4; i++)
            textDigits[i].SetText(digits[i].ToString());

        // display proper location of selection indicator
        selectionIndicator.transform.position = textDigits[selectedIndex].transform.position;

        // cycle selected position
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex++;
            if (selectedIndex >= 4)
                selectedIndex = 0;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex--;
            if (selectedIndex <= -1)
                selectedIndex = 3;
        }

        // cycle number at currently selected position
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            digits[selectedIndex]++;
            if (digits[selectedIndex] >= 10)
                digits[selectedIndex] = 0;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            digits[selectedIndex]--;
            if (digits[selectedIndex] <= -1)
                digits[selectedIndex] = 9;
        }

        // return to hub with escape key
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.LoadScene("Hub");
        }
    }
}
