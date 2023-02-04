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
        digits = GameManager.instance.GetEnteredCode();
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
            // update current entered code in GameManager
            GameManager.instance.SetEnteredCode(digits);

            // return to hub
            GameManager.instance.LoadScene("Hub");
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.instance.SetEnteredCode(digits);

            // check for correct code and then show either victory or apply punishment
            bool isCorrect = true;
            for (int i = 0; i < 4; i++)
                if (digits[i] != GameManager.instance.GetCorrectCode()[i])
                    isCorrect = false;
            if(isCorrect)
            {
                GameManager.instance.LoadScene("VictoryScene");
            }
            else // incorrect guess
            {
                // change spawn point and send player back to hub
                GameManager.instance.SetSpawnPoint(new Vector2(0, 0)); // may want to randomize where this puts you (or predetermined in a crunch)
                GameManager.instance.LoadScene("Hub");
                
                // also increment time by some amount here as a penalty (time system not implemented yet) <------ UNFINISHED UNFINISHED UNFINISHED
            }
        }
    }
}
