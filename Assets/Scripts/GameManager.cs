using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // instance
    public static GameManager instance;

    [SerializeField]
    private List<GameObject> informantSpawnList;

    private float time;

    // save data
    [System.Serializable]
    private class SaveData
    {
        public string sceneName;
        public Vector2 spawnPoint;
        public string notebookData;
        public int[] correctCode;
        public int[] enteredCode;

        public string[] livingClues;
        public string[] deadClues;
        public string[] informantClueTypes;
        public string[] informantClues;
        public GameObject[] informantLocations;
        public int[] characterLocations;
        public int[] hitList;
        public int[] deathTimes;
        public bool[] livingList;

        public int nextHit;
    }
    private SaveData data;

    #region UNITY FUNCTIONS

    public void Awake() // called each time a scene is laoded/reloaded
    {
        // set up GameManager singleton class
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            data = new SaveData();
        }
        else
        {
            Destroy(gameObject); // destroy duplicate instance of singleton object
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
        time += Time.deltaTime;

        //Restart timer if on Menu
        if (SceneManager.GetActiveScene().name == "MenuScene")
            time = 0;
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

    public List<GameObject> GetInformantSpawnPoints()
    {
        return informantSpawnList;
    }

    // get time in game (used to determine time of setting)
    public float GetGameTime()
    {
        return time;
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
        // initialize save data to default values at start of game
        data.sceneName = "Hub";
        data.spawnPoint = new Vector2(0, 0);
        data.notebookData = "Notes:\n";
        data.correctCode = new int[4] { 6, 9, 6, 9 };
        data.enteredCode = new int[4] { 0, 0, 0, 0 };

        //Start the timer 

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
