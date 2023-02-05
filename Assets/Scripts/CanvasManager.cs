using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    // Constants
    private const float NOTE_P_VALUE = -10.0f;
    private readonly float[ , ] NOTE_POSITIONS = { {-210.39f, -34.1f}, {211.19f, -34.1f}, { -210.39f, -400f}, {211.19f, -400f} };
    private const float NOTE_SNAP_THRESHOLD = 0.1f;

    // Canvas components
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBackdrop;
    [SerializeField] private GameObject notepad;
    [SerializeField] private TextMeshProUGUI noteText;

    // variables
    private enum NoteState { LEFT = 0, RIGHT = 1, HIDDEN_LEFT = 2, HIDDEN_RIGHT = 3 }
    private NoteState noteState;
    private float xTrack, yTrack;

    // Start is called before the first frame update
    void Start()
    {
        // deactivate all text/image obejcts for dialogue by default
        speakerText.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        dialogueBackdrop.gameObject.SetActive(false);

        noteState = NoteState.HIDDEN_LEFT;

        noteText.SetText(GameManager.instance.GetNotebookText());
    }

    // Update is called once per frame
    void Update()
    {
        // display current time in seconds rounded to two decimal places
        timeText.SetText("Time: " + Mathf.Round(100 * GameManager.instance.GetGameTime()) / 100);

        // set notepad positions to track to
        xTrack = NOTE_POSITIONS[(int) noteState, 0];
        yTrack = NOTE_POSITIONS[(int) noteState, 1];

        //notepad.transform.localPosition = new Vector3(xTrack, yTrack, 0);
        // proportional velocity control
        if (Mathf.Abs(notepad.transform.localPosition.x - xTrack) >= NOTE_SNAP_THRESHOLD || Mathf.Abs(notepad.transform.localPosition.y - yTrack) >= NOTE_SNAP_THRESHOLD)
            notepad.transform.localPosition += new Vector3((notepad.transform.localPosition.x - xTrack) * NOTE_P_VALUE * Time.deltaTime, (notepad.transform.localPosition.y - yTrack) * NOTE_P_VALUE * Time.deltaTime, 0);
        // snap to exact position
        else
        {
            notepad.transform.localPosition = new Vector3(xTrack, yTrack, 0);
        }
    }

    public void DisplayDialogue(string speakerName, string dialogue)
    {
        // update all text
        speakerText.SetText(speakerName);
        dialogueText.SetText(dialogue);
        // add dialogue to notebook
        GameManager.instance.SetNotebookText(GameManager.instance.GetNotebookText() + "- " + dialogue + "\n");
        noteText.SetText(GameManager.instance.GetNotebookText());

        // activate all text/image obejcts for dialogue
        speakerText.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        dialogueBackdrop.gameObject.SetActive(true);
    }

    public void ExitDialogue()
    {
        // deactivate all text/image obejcts for dialogue
        speakerText.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        dialogueBackdrop.gameObject.SetActive(false);
    }

    public void NoteLeft()
    {
        // if hidden on right, instantly make it hidden on left (ensures note always enters from bottom and not at a diagonal)
        if (noteState == NoteState.HIDDEN_RIGHT)
            notepad.transform.localPosition = new Vector3(NOTE_POSITIONS[(int) NoteState.HIDDEN_LEFT, 0], NOTE_POSITIONS[(int) NoteState.HIDDEN_LEFT, 1], 0);

        noteState = NoteState.LEFT;
    }

    public void NoteRight()
    {
        // if hidden on left, instantly make it hidden on right (ensures note always enters from bottom and not at a diagonal)
        if (noteState == NoteState.HIDDEN_LEFT)
            notepad.transform.localPosition = new Vector3(NOTE_POSITIONS[(int)NoteState.HIDDEN_RIGHT, 0], NOTE_POSITIONS[(int)NoteState.HIDDEN_RIGHT, 1], 0);

        noteState = NoteState.RIGHT;
    }

    public void NoteDown()
    {
        if (noteState == NoteState.LEFT)
            noteState = NoteState.HIDDEN_LEFT;
        else if (noteState == NoteState.RIGHT)
            noteState = NoteState.HIDDEN_RIGHT;
    }
}
