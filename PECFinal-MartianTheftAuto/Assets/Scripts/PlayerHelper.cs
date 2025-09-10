using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHelper : MonoBehaviour
{
    public static PlayerHelper instance;
    public GameObject PlayerReference;
    public GameObject PlayerFollowTarget;
    public GameObject CarReference;
    public PlayerInput ShooterInput;
    public PlayerInput CarInput;
    public CinemachineVirtualCamera MainFollowCamera;

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
    void Awake()
    {
        MakeSingleton();
    }

}
