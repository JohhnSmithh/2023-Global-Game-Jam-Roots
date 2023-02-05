using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // instance
    public static GameManager instance;

    // save data
    [System.Serializable]
    private class SaveData
    {
        public string sceneName;
        public Vector2 spawnPoint;
        public string notebookData;
        public int[] correctCode;
        public int[] enteredCode;
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

            InitializeSaveData();
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

    // get time in game (used to determine time of setting)
    public float GetGameTime()
    {
        return Time.time;
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

    #endregion

    #region SCENE MANAGEMENT
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void StartGame()
    {
        InitializeSaveData();

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
