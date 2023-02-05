using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{ 
    // instance
    public static SpawnManager instance;

    [SerializeField]
    private List<GameObject> mainCast;
    [SerializeField]
    private List<GameObject> informants;
    [SerializeField]
    private List<GameObject> mainCastSpawnList;
    [SerializeField]
    private List<GameObject> informantSpawnList;

    private List<GameObject> currentInformants;
    private GameObject currentMC;

    [SerializeField]
    private List<Vector3> charOffsets;

    #region Unity Functions

    public void Awake() // called each time a scene is laoded/reloaded
    {
        // set up GameManager singleton class
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject); // destroy duplicate instance of singleton object
        }

        currentInformants = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Spawning Functions

    public void SpawnInformants()
    {
        for (int i = 0; i < informants.Count; i++)
        {
            //Create them
            GameObject obj = Instantiate(informants[i]);
            obj.transform.SetPositionAndRotation(GameManager.instance.GetInformantLocation(i).transform.position + charOffsets[12 + i], GameManager.instance.GetInformantLocation(i).transform.rotation);

            //Give them their respective clues
            obj.GetComponent<CodeNPCData>().SetMessage(GameManager.instance.GetInformantClue(i), "");

            //Add to the current informants list
            currentInformants.Add(obj);
        }
    }

    public void SpawnMainCast()
    {
        int i = 0;
        switch (SceneManager.GetActiveScene().name)
        {
            case "Building1":
                i = 0;
                break;
            case "Building2":
                i = 1;
                break;
            case "Building3":
                i = 2;
                break;
            case "Building4":
                i = 3;
                break;
            case "Building5":
                i = 4;
                break;
            case "Building6":
                i = 5;
                break;
            case "Building7":
                i = 6;
                break;
            case "Building8":
                i = 7;
                break;
            case "Building9":
                i = 8;
                break;
            case "Building10":
                i = 9;
                break;
            case "Building11":
                i = 10;
                break;
            case "Building12":
                i = 11;
                break;
            case "Building13":
                i = 12;
                break;
        }
        //Create them
        GameObject obj = Instantiate(mainCast[i]);
        obj.transform.SetPositionAndRotation(GameManager.instance.GetCharacterLocation(i) + charOffsets[i], GameManager.instance.GetCharacterLocation(i));

        //Give them their respective clues
        obj.GetComponent<CodeNPCData>().SetMessage(GameManager.instance.GetInformantClue(i), "");

        //Add to the current informants list
        currentInformants.Add(obj);
    }

    #endregion

    #region Getters

    public List<GameObject> GetInformants()
    {
        return informants;
    }

    public List<GameObject> GetInformantSpawnList()
    {
        return informantSpawnList;
    }

    #endregion

    public void RefreshInformatants()
    {
        for (int i = 0; i < 5; i++) 
            currentInformants[i].GetComponent<CodeNPCData>().hasSpoken = false;
        Debug.Log("Refreshed");
    }
}
