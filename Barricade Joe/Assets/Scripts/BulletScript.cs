using System.Collections;
using UnityEngine;

public class BulletScript : MonoBehaviour, IPooledObject
{
    public float speed = 5;
    public float DelayToActive = 2;
    public float damagePoint = 1;
    public bool isPhysicControlled = true;

    TrailRenderer t;
    Rigidbody rb;

    public void OnObjectSpawn()
    {
        if (isPhysicControlled)
        {
            if (t == null)
                t = GetComponentInChildren<TrailRenderer>();
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            t.Clear();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dodger"))
            other.GetComponentInParent<Ai_Script>().DodgeBullet();
        if (other.CompareTag("Enemy"))
            other.GetComponent<Ai_Script>().ReduceHealth(damagePoint, this);
        if (!other.CompareTag("Player") && !other.CompareTag("Bullet"))
            HitEffect();
    }

    void FixedUpdate()
    {
        if (isPhysicControlled)
            rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
    }
    void HitEffect()
    {
        ObjectPooler.instance.SpawnFromPool("Hit", transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(DelayToActive);
        gameObject.SetActive(false);
    }

    public void OnObjectActive()
    {
        if (isPhysicControlled)
            StartCoroutine(Delay());
    }
}
