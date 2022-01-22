using UnityEngine;

public class Barricade : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Ai_Script ai = other.GetComponent<Ai_Script>();
            if (ai != null)
                if (ai.m_enemyType == Ai_Script.EnemyType.BombBlaster)
                    ai.ReduceHealth(10);
            Death();
        }
    }
    public void Death()
    {
        ObjectPooler.instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity);
        EZCameraShake.CameraShaker.Instance.ShakeOnce(5f, 10, .1f, 1f);
        Timer.Register(.1f, () => GameManager.instance.BarricadeDestroyed());
        gameObject.SetActive(false);
    }
}
