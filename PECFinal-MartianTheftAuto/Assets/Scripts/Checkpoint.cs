using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public AudioClip CheckpointClip;
    private bool _passed;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_passed)
        {
            _passed = true;
            //SoundManager.instance.PlayOnce(CheckpointClip);
            other.SendMessage("ChangeRespawnPoint", transform);
        }
    }
}
