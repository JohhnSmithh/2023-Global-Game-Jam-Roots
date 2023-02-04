using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeedManager : MonoBehaviour
{

    //Code
    private int[] code;

    //List of live/dead clues, each character corresponds to an index
    private List<Tuple<string, string>> clueList = new List<Tuple<string, string>>();

    //Generates the code and subsequent seeded info
    public void GenerateSeed()
    {
        //Code
        code = new int[4];
        for(int i = 0; i < code.Length; i++)
            code[i] = UnityEngine.Random.Range(0, 10);

        //Clues
        
    }

    //Gets the code
    public int[] GetCode()
    {
        return code;
    }

    //Debugs the code
    public void PrintCode()
    {
        Debug.Log(code[0] + " " + code[1] + " " + code[2] + " " + code[3]);
    }

    //Gets the clue for the person corresponding to the specified index
    public string GetClue(int indx, bool alive)
    {
        if (alive)
            return clueList[indx].Item1;
        return clueList[indx].Item2;
    }
}
