using UnityEngine;

public class PlayAudioOnSpawn : MonoBehaviour, IPooledObject
{
    [SerializeField] Audio Sound;
    // Start is called before the first frame update
    public void OnObjectActive()
    {
        AudioManager.instance.PlayAudio(Sound);
    }

    public void OnObjectSpawn()
    {

    }
}
