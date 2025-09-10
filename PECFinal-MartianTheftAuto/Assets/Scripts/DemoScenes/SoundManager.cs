using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public UnityEngine.Audio.AudioMixer myAudioMixer;
    public float timeToChangesnapshot = 1.0f;

    private UnityEngine.Audio.AudioMixerSnapshot[] snapshotArray;
    private float[] snapshotWeightArray;

    void SetSetFixedVolume()
    {
        myAudioMixer.SetFloat("ingame_FX_volume", -13.0f);
    }

    void SetSnapshot(string snapshotName,
                     float timeTochangeIt,
                     float snapshotWeight = 1.0f)
    {
        UnityEngine.Audio.AudioMixerSnapshot snapshot = myAudioMixer.FindSnapshot(snapshotName);

        snapshotArray = new UnityEngine.Audio.AudioMixerSnapshot[1];
        snapshotArray[0] = snapshot;
        snapshotWeightArray = new float[1];
        snapshotWeightArray[0] = snapshotWeight;

        myAudioMixer.TransitionToSnapshots(snapshotArray, snapshotWeightArray, timeTochangeIt);
    }

    public void PauseGame()
    {
        SetSnapshot("Pause", 1.0f);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PauseGame();
 
    }
}
