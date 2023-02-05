using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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