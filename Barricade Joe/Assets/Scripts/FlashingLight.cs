using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    public float Delay = .1f;

    private void OnEnable()
    {
        Timer.Register(Delay, () => gameObject.SetActive(false));
    }
}
