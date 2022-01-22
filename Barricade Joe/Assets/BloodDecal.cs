using DG.Tweening;
using UnityEngine;

public class BloodDecal : MonoBehaviour
{
    [SerializeField] float bloodDelay;
    [SerializeField] GameObject[] smallBlood;
    [SerializeField] GameObject[] bigBlood;
    [SerializeField] GameObject[] hugeBlood;
    Renderer rend;
    private void OnDisable()
    {
        rend.DOKill();
    }
    private void OnDestroy()
    {
        rend.DOKill();
    }
    public void SpawnBlood(BloodType _bloodType, Color _color)
    {
        GameObject r = null;
        switch (_bloodType)
        {
            case BloodType.small:
                r = smallBlood.GetRandom();
                break;

            case BloodType.big:
                r = bigBlood.GetRandom();
                break;

            case BloodType.huge:
                r = hugeBlood.GetRandom();
                break;
        }
        r.SetActive(true);
        rend = r.GetComponent<Renderer>();
        rend.material.SetColor("_BaseColor", _color);
        rend.material.DOFade(1, "_BaseColor", 0);
        rend.material.DOFade(0, "_BaseColor", bloodDelay).OnComplete(() => gameObject.SetActive(false));
    }


}
public enum BloodType { small, big, huge }
