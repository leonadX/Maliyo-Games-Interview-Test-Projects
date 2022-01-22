using System.Collections;
using UnityEngine;

public class SetChildPostionAfterStart : MonoBehaviour
{
    [SerializeField] Vector3 pos;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        transform.localPosition = pos;
    }

}
