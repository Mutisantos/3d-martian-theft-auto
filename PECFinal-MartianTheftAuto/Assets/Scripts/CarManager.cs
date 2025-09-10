using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityStandardAssets.Vehicles.Car;

public class CarManager : MonoBehaviour
{

    public CarController Controller;
    public CarAIControl AIControl;
    public CarUserControl UserControl;
    public PlayerInput CarInput;
    public GameObject TargetFollow;
    public GameObject MountIndicator;
    public GameObject DropDownReference;
    public bool CanPlayerDrive;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && CanPlayerDrive)
        {
            if (AIControl != null)
            {
                AIControl.enabled = false;
            }
            UserControl.enabled = true;
            CarInput.enabled = true;
            PlayerHelper.instance.PlayerReference.SendMessage("NotifyDriveChange", false);
            PlayerHelper.instance.CarInput = CarInput;
            PlayerHelper.instance.ShooterInput.enabled = false;
            PlayerHelper.instance.CarInput.enabled = true;
            PlayerHelper.instance.MainFollowCamera.Follow = TargetFollow.transform;
            PlayerHelper.instance.PlayerReference.SetActive(false);
            PlayerHelper.instance.PlayerReference.transform.SetParent(transform);
            PlayerHelper.instance.CarInput.enabled = true;
            MountIndicator.SetActive(false);
            CanPlayerDrive = false;
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Q) && !PlayerHelper.instance.PlayerReference.activeSelf)
        {
            PlayerHelper.instance.MainFollowCamera.Follow = PlayerHelper.instance.PlayerFollowTarget.transform;
            PlayerHelper.instance.PlayerReference.SetActive(true);
            PlayerHelper.instance.CarInput.enabled = false;
            PlayerHelper.instance.ShooterInput.enabled = true;
            PlayerHelper.instance.PlayerReference.transform.SetParent(null);
            MountIndicator.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanPlayerDrive = true;
            MountIndicator.SetActive(true);
            PlayerHelper.instance.PlayerReference.SendMessage("NotifyDriveChange", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanPlayerDrive = false;
            MountIndicator.SetActive(false);
            PlayerHelper.instance.PlayerReference.SendMessage("NotifyDriveChange", false);
        }
    }
}
