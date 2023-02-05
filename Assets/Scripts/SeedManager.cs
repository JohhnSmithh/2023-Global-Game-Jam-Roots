using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    //Code
    private static int[] code;

    //List of live/dead clues, each character corresponds to an index
    private static string[] livingClueList;
    private static string[] deadClueList;

    //Informant stuff
    private static string[] informantClueTypeList;

    //Character locations
    private static int[] characterLocationList;

    //Death order
    private static int[] hitList;
    private static int[] deathTimes;

    //Generates the code and subsequent seeded info
    public static void GenerateSeed()
    {
        //Code
        code = new int[4];
        for(int i = 0; i < code.Length; i++)
            code[i] = UnityEngine.Random.Range(0, 10);

        //Clues
        livingClueList = new string[12];
        deadClueList = new string[12];
        informantClueTypeList = new string[5];
        string[] indexStrs = new string[4];
        indexStrs[0] = "1st";
        indexStrs[1] = "2nd";
        indexStrs[2] = "3rd";
        indexStrs[3] = "4th";

        //===================== Living Clues =====================\\

        //Average
        int avgI = UnityEngine.Random.Range(0, 12);
        livingClueList[avgI] = "The average of the code's digits is " + (code[0] + code[1] + code[2] + code[3]) / 4.0f;

        //Correct numbers
        int numI1 = UnityEngine.Random.Range(0, 12);
        int numI2 = UnityEngine.Random.Range(0, 12);
        while (numI1 == avgI)
            numI1 = UnityEngine.Random.Range(0, 12);
        while (numI2 == avgI || numI2 == numI1)
            numI2 = UnityEngine.Random.Range(0, 12);

        int codeI1 = UnityEngine.Random.Range(0, 4);
        int codeI2 = UnityEngine.Random.Range(0, 4);
        while(codeI2 == codeI1)
            codeI2 = UnityEngine.Random.Range(0, 4);

        //Make the messages 
        livingClueList[numI1] = "The " + indexStrs[codeI1] + " digit of the code is " + code[codeI1];
        livingClueList[numI2] = "The " + indexStrs[codeI2] + " digit of the code is " + code[codeI2];

        //All others
        int[] livingTypeCounts = new int[4];
        List<string> livingClueTypes = new()
        {
            "Range",
            "Sum",
            "Contains",
            "Compare"
        };

        int indx = 0;

        while(indx < 12)
        {
            //Break if we already got this one
            if (indx == avgI || indx == numI1 || indx == numI2)
            {
                indx++;
                continue;
            }

            int r = UnityEngine.Random.Range(0, livingClueTypes.Count);

            //Range clue type
            if (livingClueTypes[r] == "Range")
            {
                //Generate code index and the range
                int codeI = UnityEngine.Random.Range(0, 4);
                int rangeLow = code[codeI] - UnityEngine.Random.Range(0, 3);
                if (rangeLow < 0)
                    rangeLow = 0;
                if (rangeLow + 2 > 9)
                    rangeLow = 7;

                //Make the message
                livingClueList[indx] = "The " + indexStrs[codeI] + " digit of the code is a number from " + rangeLow + " to " + (rangeLow + 2);

                //Increase the type count
                livingTypeCounts[0]++;
            }

            //Sum of two
            else if (livingClueTypes[r] == "Sum")
            {
                //Generate two distinct code indices
                codeI1 = UnityEngine.Random.Range(0, 4);
                codeI2 = UnityEngine.Random.Range(0, 4);
                while (codeI2 == codeI1)
                    codeI2 = UnityEngine.Random.Range(0, 4);

                //Make the message
                livingClueList[indx] = "The sum of the " + indexStrs[codeI1] + " and " + indexStrs[codeI2] + " digits is " + (code[codeI1] + code[codeI2]);

                //Increase the type count
                livingTypeCounts[1]++;
            }

            //Contains a number
            else if (livingClueTypes[r] == "Contains")
            {
                //Generate code index
                int codeI = UnityEngine.Random.Range(0, 4);

                //Make the message
                livingClueList[indx] = "The code contains the number " + code[codeI];

                livingTypeCounts[2]++;
            }

            //Compare two indices
            else if (livingClueTypes[r] == "Compare")
            {
                //Generate two distinct code indices
                codeI1 = UnityEngine.Random.Range(0, 4);
                codeI2 = UnityEngine.Random.Range(0, 4);
                while (codeI2 == codeI1)
                    codeI2 = UnityEngine.Random.Range(0, 4);

                //Make the message
                if (code[codeI1] < code[codeI2])
                    livingClueList[indx] = "The " + indexStrs[codeI2] + " digit of the code is " + (code[codeI2] - code[codeI1]) + " greater than the " + indexStrs[codeI1] + " digit of the code";
                else if (code[codeI2] < code[codeI1])
                    livingClueList[indx] = "The " + indexStrs[codeI1] + " digit of the code is " + (code[codeI1] - code[codeI2]) + " greater than the " + indexStrs[codeI2] + " digit of the code";
                else
                    livingClueList[indx] = "The " + indexStrs[codeI1] + " digit of the code is the same as the " + indexStrs[codeI2] + " digit of the code";

                //Increase the type count
                livingTypeCounts[3]++;
            }

            //Wut
            else
            {
                livingClueList[indx] = "Go Away";
            }

            //Prevent too many of one type
            if (livingTypeCounts[0] >= 4)
                livingClueTypes.Remove("Range");
            if (livingTypeCounts[1] >= 4)
                livingClueTypes.Remove("Sum");
            if (livingTypeCounts[2] >= 4)
                livingClueTypes.Remove("Contains");
            if (livingTypeCounts[3] >= 4)
                livingClueTypes.Remove("Compare");

            //Increase the index
            indx++;
        }

        //===================== Dead Clues =====================\\
        
        int[] deadTypeCounts = new int[6];
        List<string> deadClueTypes = new()
        {
            "Range",
            "EvenOdd",
            "NotContains",
            "Compare",
            "NotNumber",
            "Nothing"
        };

        indx = 0;

        while (indx < 12)
        {

            int r = UnityEngine.Random.Range(0, deadClueTypes.Count);

            //Range clue type
            if (deadClueTypes[r] == "Range")
            {
                //Generate code index and the range
                int codeI = UnityEngine.Random.Range(0, 4);
                int rangeLow = code[codeI] - UnityEngine.Random.Range(0, 6);
                if (rangeLow < 0)
                    rangeLow = 0;
                if (rangeLow + 5 > 9)
                    rangeLow = 4;

                //Make the message
                deadClueList[indx] = "The " + indexStrs[codeI] + " digit of the code is a number from " + rangeLow + " to " + (rangeLow + 5);

                //Increase the type count
                deadTypeCounts[0]++;
            }

            //Even or odd
            else if (deadClueTypes[r] == "EvenOdd")
            {
                //Generate code index
                int codeI = UnityEngine.Random.Range(0, 4);

                //Make the message
                if (code[codeI] % 2 == 0)
                    deadClueList[indx] = "The " + indexStrs[codeI] + " digit of the code is even";
                else
                    deadClueList[indx] = "The " + indexStrs[codeI] + " digit of the code is odd";

                //Increase the type count
                deadTypeCounts[1]++;
            }

            //Contains a number
            else if (deadClueTypes[r] == "NotContains")
            {
                //Create int list
                List<int> digitList = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                //Remove the code digits from the digit list
                foreach (int digit in code)
                    digitList.Remove(digit);

                //Random remaining digit
                r = UnityEngine.Random.Range(0, digitList.Count);

                //Make the message
                deadClueList[indx] = "The code does not contain the number " + r;

                deadTypeCounts[2]++;
            }

            //Compare two indices
            else if (deadClueTypes[r] == "Compare")
            {
                //Generate two distinct code indices
                codeI1 = UnityEngine.Random.Range(0, 4);
                codeI2 = UnityEngine.Random.Range(0, 4);
                while (codeI2 == codeI1)
                    codeI2 = UnityEngine.Random.Range(0, 4);

                //Make the message
                if (code[codeI1] < code[codeI2])
                    deadClueList[indx] = "The " + indexStrs[codeI2] + " digit of the code is greater than or equal to the " + indexStrs[codeI1] + " digit of the code";
                else if (code[codeI1] > code[codeI2])
                    deadClueList[indx] = "The " + indexStrs[codeI2] + " digit of the code is less than or equal to the " + indexStrs[codeI1] + " digit of the code";
                else
                {
                    r = UnityEngine.Random.Range(0, 2);
                    if (r == 0)
                        deadClueList[indx] = "The " + indexStrs[codeI2] + " digit of the code is greater than or equal to the " + indexStrs[codeI1] + " digit of the code";
                    else
                        deadClueList[indx] = "The " + indexStrs[codeI2] + " digit of the code is less than or equal to the " + indexStrs[codeI1] + " digit of the code";
                }

                //Increase the type count
                deadTypeCounts[3]++;
            }

            //Index is not a number
            else if (deadClueTypes[r] == "NotNumber")
            {
                //Create int list
                List<int> digitList = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                //Generate code index and remove digit from digitList
                int codeI = UnityEngine.Random.Range(0, 4);
                digitList.Remove(code[codeI]);

                //Random remaining digit
                r = UnityEngine.Random.Range(0, digitList.Count);

                //Make the message
                deadClueList[indx] = "The " + indexStrs[codeI] + " does not contain the number " + r;

                deadTypeCounts[4]++;
            }

            //Nothing
            else if (deadClueTypes[r] == "Nothing")
            {
                //Random nothing message
                r = UnityEngine.Random.Range(0, 3);

                //Make the message
                if (r == 0)
                    deadClueList[indx] = "My dying wish is to aid you on your mission. The code... has 4 digits!";
                else if (r == 1)
                    deadClueList[indx] = "Why are you wasting your time running around and letting innocent citizens die to unlock a coded lock? Go buy a crowbar you fool!";
                else
                    deadClueList[indx] = "I hope that this information will be able to reach you past my death. The code's digits... are 0 through 9!";

                deadTypeCounts[5]++;
            }

            //Prevent too many of one type
            if (deadTypeCounts[0] >= 4)
                deadClueTypes.Remove("Range");
            if (deadTypeCounts[1] >= 4)
                deadClueTypes.Remove("EvenOdd");
            if (deadTypeCounts[2] >= 4)
                deadClueTypes.Remove("NotContains");
            if (deadTypeCounts[3] >= 4)
                deadClueTypes.Remove("Compare");
            if (deadTypeCounts[4] >= 4)
                deadClueTypes.Remove("NotNumber");
            if (deadTypeCounts[5] >= 4)
                deadClueTypes.Remove("Nothing");

            //Increase the index
            indx++;
        }

        //===================== Informant Clue Types =====================\\

        int[] informantTypeCounts = new int[3];
        List<string> informantClueTypes = new()
        {
            "Time",
            "Address",
            "Location"
        };

        indx = 0;

        while (indx < 5)
        {

            int r = UnityEngine.Random.Range(0, informantClueTypes.Count);

            //Add the type to the list
            informantClueTypeList[indx] = informantClueTypes[r];

            switch (informantClueTypes[r])
            {
                case "Time":
                    informantTypeCounts[0]++;
                    break;
                case "Address":
                    informantTypeCounts[1]++;
                    break;
                case "Location":
                    informantTypeCounts[2]++;
                    break;
            }

            //Prevent too many of one type
            if (informantTypeCounts[0] >= 3)
                informantClueTypes.Remove("Time");
            if (informantTypeCounts[1] >= 3)
                informantClueTypes.Remove("Address");
            if (informantTypeCounts[2] >= 3)
                informantClueTypes.Remove("Location");

            //Increase the index
            indx++;
        }

        //===================== Generate character placement =====================\\

        characterLocationList = new int[13];
        List<int> mainCharacterList = new() { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

        indx = 0;

        while (indx < characterLocationList.Length)
        {
            //Generate random number
            int r = UnityEngine.Random.Range(0, mainCharacterList.Count);

            //Add the character
            characterLocationList[indx] = mainCharacterList[r];

            //Remove character from char list
            mainCharacterList.Remove(mainCharacterList[r]);

            //Increase the index
            indx++;
        }

        //===================== Generate death order =====================\\

        hitList = new int[17];
        List<int> allCharacterList = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        indx = 0;

        while (indx < hitList.Length)
        {
            //Generate random number
            int r = UnityEngine.Random.Range(0, allCharacterList.Count);

            //Add the character
            hitList[indx] = allCharacterList[r];

            //Remove character from char list
            allCharacterList.Remove(allCharacterList[r]);

            //Increase the index
            indx++;
        }

        //===================== Generate death times =====================\\

        deathTimes = new int[17];

        //Give leeway on first death
        deathTimes[0] = UnityEngine.Random.Range(10, 30);

        indx = 1;

        while (indx < deathTimes.Length)
        {
            //Generate random time
            int r = UnityEngine.Random.Range(0, 30);
            r += indx * 30;

            //Add the death time to da time array
            deathTimes[indx] = r;

            //Increase the index
            indx++;
        }

        //Send data to Game Manager
        GameManager.instance.SetLivingClues(livingClueList);
        GameManager.instance.SetDeadClues(deadClueList);
        GameManager.instance.SetInformantClueTypes(informantClueTypeList);
        GameManager.instance.SetCharacterLocations(characterLocationList);
        GameManager.instance.SetHitList(hitList);
        GameManager.instance.SetDeathTimes(deathTimes);
    }
}
