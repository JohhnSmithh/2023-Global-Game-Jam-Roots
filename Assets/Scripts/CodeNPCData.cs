using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeNPCData : MonoBehaviour
{
    [SerializeField] private int clueIndex;
    [SerializeField] private string clueMessage;
    public bool hasSpoken;

    private void Start()
    {
        hasSpoken = false; // should replace later with whether they have spoken as stored in the GameManager with clue structure
    }

    public int getIndex()
    {
        return clueIndex;
    }

    public string getMessage()
    {
        return clueMessage;
    }

}
