using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeNPCData : MonoBehaviour
{
    [SerializeField] private int clueIndex;
    [SerializeField] private string npcName;
    [SerializeField] private string livingClueMessage;
    [SerializeField] private string deadClueMessage;
    [SerializeField] public bool hasSpoken;
    [SerializeField] private bool alive;

    Animator animator;

    private void Start()
    {
        hasSpoken = false; // should replace later with whether they have spoken as stored in the GameManager with clue structure
        alive = true;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        alive = GameManager.instance.IsAlive(clueIndex);
        animator.SetBool("isDead", !alive);
    }

    public int GetIndex()
    {
        return clueIndex;
    }

    public string GetName()
    {
        return npcName;
    }

    public string GetMessage()
    {
        return alive ? livingClueMessage : deadClueMessage;
    }

    public void SetMessage(string livingMsg, string deadMsg)
    {
        livingClueMessage = livingMsg;
        deadClueMessage = deadMsg;
    }

}
