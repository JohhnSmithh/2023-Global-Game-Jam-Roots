using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBackdrop;

    // Start is called before the first frame update
    void Start()
    {
        // deactivate all text/image obejcts for dialogue by default
        speakerText.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        dialogueBackdrop.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // display current time in seconds rounded to two decimal places
        timeText.SetText("Time: " + Mathf.Round(100 * GameManager.instance.getGameTime()) / 100);
    }

    public void displayDialogue(string speakerName, string dialogue)
    {
        // update all text
        speakerText.SetText(speakerName);
        dialogueText.SetText(dialogue);

        // activate all text/image obejcts for dialogue
        speakerText.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        dialogueBackdrop.gameObject.SetActive(true);
    }

    public void exitDialogue()
    {
        // deactivate all text/image obejcts for dialogue
        speakerText.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        dialogueBackdrop.gameObject.SetActive(false);
    }
}
