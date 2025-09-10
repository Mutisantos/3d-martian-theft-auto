using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GameManager : MonoBehaviour
{

    private Dictionary<string, int> KeyInventory = new Dictionary<string, int>();
    public List<Checkpoint> AllCheckpoints;
    public Checkpoint LastCheckpoint;
    public PlayerShooter PlayerReference;
    public Light SceneLight;
    public static GameManager instance = null;


    void Awake()
    {
        PlayerReference.ChangeRespawnPoint(LastCheckpoint.transform);
        KeyInventory = new Dictionary<string, int>();
        MakeSingleton();
    }

    private void Start()
    {
        var lightData = SceneLight.GetComponent<HDAdditionalLightData>();
        lightData.intensity = PlayerPrefs.GetFloat("Brightness");
        SceneLight.intensity = PlayerPrefs.GetFloat("Brightness");
        SceneLight.colorTemperature = PlayerPrefs.GetFloat("Brightness");
        Debug.Log($"{SceneLight.intensity}");
    }

    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CleanInventory()
    {
        KeyInventory = new Dictionary<string, int>();
    }

    public void AddItemToInventory(string keyName, int amount)
    {
        KeyInventory.Add(keyName.ToUpper(), amount);
    }

    public bool HasItemOnInventory(string keyName)
    {
        return KeyInventory.ContainsKey(keyName.ToUpper());
    }


    public void RespawnPlayerToLastCheckpoint()
    {
        PlayerReference.Respawn();
    }
}
