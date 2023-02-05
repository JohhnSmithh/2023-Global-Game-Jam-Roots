using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // instance
    public static GameManager instance;

    //Game time stuff
    private float gameTime;

    // save data
    [System.Serializable]
    private class SaveData
    {
        public string sceneName;
        public Vector2 spawnPoint;
        public string notebookData;
        public int[] correctCode;
        public int[] enteredCode;

        public string[] livingClues = new string[12];
        public string[] deadClues = new string[12];
        public string[] informantClueTypes = new string[5];
        public string[] informantClues = new string[5];
        public GameObject[] informantLocations = new GameObject[5];
        public int[] characterLocations = new int[13];
        public int[] hitList = new int[17];
        public int[] deathTimes = new int[17];
        public bool[] livingList = new bool[17];

        public int nextHit = 0;
    }
    private SaveData data;

    //Generates locations of informatants
    public void ChangeInformantLocations()
    {
        //Make a copy of spawn list to remove stuff from - prevents two characters on one spawn
        List<GameObject> spawnList = new List<GameObject>(SpawnManager.instance.GetInformantSpawnList());

        //Check for ded bodies
        for (int i = 0; i < 5; i++)
            if (!IsAlive(i))
                spawnList.Remove(data.informantLocations[i]);

        for (int i = 0; i < 5; i++)
        {
            if (IsAlive(12 + i))
            {
                int r = UnityEngine.Random.Range(0, spawnList.Count);

                //Set da location
                data.informantLocations[i] = spawnList[r];

                //Remove spawn from spawn list
                spawnList.Remove(spawnList[r]);
            }
        }
    }

    //Generates informant clues
    public void ChangeInformantClues()
    {
        int hour, minute;
        string z, timePostfix;

        for (int i = 0; i < 5; i++)
        {
            switch (GetInformantClueType(i))
            {
                case "Time":

                    //Time conversion
                    hour = 6 + (int)(GetDeathTimes()[GetNextHitIndex()] / 60);
                    minute = (int)(GetDeathTimes()[GetNextHitIndex()] % 60);
                    z = "";
                    timePostfix = "PM";

                    //Auxiliary text
                    if (minute < 10)
                        z = "0";
                    if (hour > 12)
                    {
                        hour -= 12;
                        timePostfix = "AM";
                    }

                    //Make the message
                    data.informantClues[i] = "I hear voices in the dumpsters. They said that the next gig is going down at " + hour + ":" + z + minute + " " + timePostfix;
                    break;


                case "Address":
                    
                    //If a garlic is next to die
                    if (data.hitList[data.nextHit] >= 12)
                    {
                        //Make da message
                        data.informantClues[i] = "The voices! They're getting louder! One of my kind is next...";
                        break;
                    }

                    //Make da message
                    data.informantClues[i] = "The rats talk at night. They told me that a gig is happening soon at [Address]";
                    break;

                case "Location":

                    //If a garlic is next to die
                    if (data.hitList[data.nextHit] >= 12)
                    {
                        //Make da message
                        data.informantClues[i] = "The voices! They're getting louder! One of my kind is next...";
                        break;
                    }

                    //Make da message
                    data.informantClues[i] = "A little weevil told me that something's going down in the [X] part of town";
                    break;
            }
        }
    }

    #region UNITY FUNCTIONS

    public void Awake() // called each time a scene is laoded/reloaded
    {
        // set up GameManager singleton class
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            data = new SaveData();

            InitializeSaveData();
        }
        else
        {
            Destroy(gameObject); // destroy duplicate instance of singleton object
        }

        //Check if hub
        if(SceneManager.GetActiveScene().name == "Hub")
        {
            SpawnManager.instance.SpawnInformants();
        }
    }

    // Start is called before the first frame update
    void Start() // called only once at program boot-up (since duplicate GameManagers are destroyed in Awake())
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Timer time
        gameTime += Time.deltaTime;

        //Restart timer if on Menu
        if (SceneManager.GetActiveScene().name == "MenuScene")
            gameTime = 0;

        //Check for death
        else if(gameTime >= GetDeathTimes()[GetNextHitIndex()])
        {
            Murder(GetHitList()[GetNextHitIndex()]);
            data.nextHit++; 
        }
    }

    #endregion

    #region DATA FUNCTIONS

    private void InitializeSaveData()
    {
        // initialize save data to default values
        data.sceneName = "Hub";
        data.spawnPoint = new Vector2(0, 0);
        data.notebookData = "Notes:\n";
        data.correctCode = new int[4] { 6, 9, 6, 9 };
        data.enteredCode = new int[4] { 0, 0, 0, 0 };
        data.livingList = new bool[17] {true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        data.nextHit = 0;
}

    public string GetSceneName()
    {
        return data.sceneName;
    }

    public Vector2 GetSpawnPoint()
    {
        return data.spawnPoint;
    }

    public string GetNotebookText()
    {
        return data.notebookData;
    }

    public int[] GetCorrectCode()
    {
        return data.correctCode;
    }

    public int[] GetEnteredCode()
    {
        return data.enteredCode;
    }

    public string GetClue(bool alive, int indx)
    {
        return alive ? data.livingClues[indx] : data.deadClues[indx];
    }

    public string GetInformantClueType(int indx)
    {
        return data.informantClueTypes[indx];
    }

    public string GetInformantClue(int indx)
    {
        return data.informantClues[indx];
    }

    public GameObject GetInformantLocation(int indx)
    {
        return data.informantLocations[indx];
    }

    public int GetCharacterLocation(int indx)
    {
        return data.characterLocations[indx];
    }

    public int[] GetHitList()
    {
        return data.hitList;
    }

    public int[] GetDeathTimes()
    {
        return data.deathTimes;
    }

    public int GetNextHitIndex()
    {
        return data.nextHit;
    }

    public bool IsAlive(int indx)
    {
        return data.livingList[indx];
    }

    // get time in game (used to determine time of setting)
    public float GetGameTime()
    {
        return gameTime;
    }

    public void SetSceneName(string sceneName)
    {
        data.sceneName = sceneName;
    }

    public void SetSpawnPoint(Vector2 spawnPoint)
    {
        data.spawnPoint = spawnPoint;
    }

    public void SetNotebookText(string notebookText)
    {
        data.notebookData = notebookText;
    }

    public void SetCorrectCode(int[] correctCode)
    {
        data.correctCode = correctCode;
    }

    public void SetEnteredCode(int[] newCode)
    {
        data.enteredCode = newCode;
    }

    public void SetLivingClues(string[] livingClues)
    {
        data.livingClues = livingClues;
    }

    public void SetDeadClues(string[] deadClues)
    {
        data.deadClues = deadClues;
    }

    public void SetInformantClueTypes(string[] informantClueTypes)
    {
        data.informantClueTypes = informantClueTypes;
    }

    public void SetInformantClues(string[] informantClues)
    {
        data.informantClues = informantClues;
    }

    public void SetInformantLocations(GameObject[] informantLocations)
    {
        data.informantLocations = informantLocations;
    }

    public void SetCharacterLocations(int[] characterLocations)
    {
        data.characterLocations = characterLocations;
    }

    public void SetHitList(int[] hitList)
    {
        data.hitList = hitList;
    }

    public void SetDeathTimes(int[] deathTimes)
    {
        data.deathTimes = deathTimes;
    }

    public void Murder(int charI)
    {
        data.livingList[charI] = false;
    }

    public void Revive(int charI)
    {
        data.livingList[charI] = true;
    }

    #endregion

    #region SCENE MANAGEMENT
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void StartGame()
    {
        InitializeSaveData();

        //Generate the seed
        SeedManager.GenerateSeed();

        //Randomize starting locations and the clues for informants
        ChangeInformantClues();
        ChangeInformantLocations();

        LoadScene("Hub"); // may want to change to a 'tutorial scene with a sign for controls and walking to town
    }

    public void LoadInfoScene()
    {
        LoadScene("InfoScene");
    }

    public void MenuScene()
    {
        LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void TransitionScene(TransitionData transitionData)
    {
        data.sceneName = transitionData.sceneName;
        data.spawnPoint = transitionData.spawnPoint;

        LoadScene(data.sceneName);
    }

    #endregion

}
