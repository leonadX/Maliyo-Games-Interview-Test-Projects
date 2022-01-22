using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[SelectionBase]
public class Ai_Script : MonoBehaviour, IPooledObject
{
    public EnemyType m_enemyType;
    [SerializeField] float health = 2;
    [SerializeField] BloodType m_bloodType;

    [Header("Exploding Guys")]
    [SerializeField] float ExplosionRadius = 5;
    [SerializeField] float BlastDamage = 5;

    [Header("Healer")]
    [SerializeField] GameObject Effect;
    [SerializeField] float healingAmount;
    [SerializeField] float healingRate = .5f;
    Ai_Script healerTarget;

    [Header("Audio")]
    [SerializeField] Audio[] DeathSound;

    Color myColor = Color.red;
    Transform player;
    NavMeshAgent agent;
    Animator anim;
    BulletScript lastBullet = null;
    GameObject tracker;
    TargetIndicatorInCavas targetCanvasLocal;
    public float Health
    {
        get => health; set
        {
            health = value;
            if (health <= 0)
                Death();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();


        anim = GetComponent<Animator>();
        myColor = transform.GetChild(1).GetComponent<Renderer>().material.GetColor("_BaseColor");
        switch (m_enemyType)
        {
            case EnemyType.Zombies:
                //agent.speed = Random.Range(.25f, .85f);
                agent.speed = Random.Range(.65f, 1.55f);
                anim.speed = agent.speed * 4;
                agent.SetDestination(player.position);
                break;

            case EnemyType.BombBlaster:
                //anim.speed = agent.speed * 4;
                agent.SetDestination(player.position);
                break;

            case EnemyType.FastZombies:
                agent.speed += Random.Range(0, 1.2f);
                anim.speed = agent.speed * 4;
                agent.SetDestination(player.position);
                break;

            case EnemyType.Giant:
                anim.speed = agent.speed * 4;
                agent.SetDestination(player.position);
                break;

            case EnemyType.Wizard:
                agent.SetDestination(player.position + (Random.onUnitSphere).SetPositionY(transform.position.y) * Random.Range(10, 15));
                StartCoroutine(WizardAI());
                break;

            case EnemyType.Healer:
                if (Effect)
                    Effect.SetActive(false);
                StartCoroutine(HealerAI());
                break;

            default:
                break;
        }

        CreateTargetInCanvas();
    }
    private void OnDisable()
    {
        if (tracker && targetCanvasLocal.Target == this)
            tracker.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {

        if (tracker && targetCanvasLocal.Target == this)
            tracker.gameObject.SetActive(false);
    }
    void CreateTargetInCanvas()
    {
        //Instanciando indicador
        targetCanvasLocal = ObjectPooler.instance.SpawnFromPool("indicator", Vector3.zero, Quaternion.identity, false).GetComponent<TargetIndicatorInCavas>();
        //targetCanvasLocal.objectWorld = this;
        //Colocando indicador como filho do Canvas
        //targetCanvasLocal.transform.SetParent(canvas.transform);
        //Definindo uma posição inicial para o indicador (0,0)
        targetCanvasLocal.transform.localPosition = Vector3.zero;
        targetCanvasLocal.Target = transform;

        //Pegando o tipo de Render atual
        //Canvas canvasScript = canvas.GetComponent<Canvas>();
        //targetCanvasLocal.renderMode = canvasScript.renderMode;

        targetCanvasLocal.padding = 30f;
        targetCanvasLocal.GetComponent<Image>().color = myColor;
        targetCanvasLocal.thisObject = targetCanvasLocal.GetComponent<RectTransform>();
        //targetCanvasLocal.mainCanvas = canvas.GetComponent<RectTransform>();

        //Definindo um tamanho para o indicador
        targetCanvasLocal.thisObject.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        tracker = targetCanvasLocal.gameObject;
    }
    private void GetNextHealingTarget()
    {
        Ai_Script a = GameManager.instance.Ais.RandomItem().GetComponent<Ai_Script>();
        if (a != null)
        {
            agent.SetDestination(a.transform.position + (-a.transform.forward * 1.5f));
            healerTarget = a;
        }

        else
            agent.SetDestination(player.position + (Random.onUnitSphere).SetPositionY(transform.position.y) * Random.Range(10, 15));
    }

    IEnumerator HealerAI()
    {
        yield return null;
        GetNextHealingTarget();
        anim.SetBool("walking", true);

        while (true)
        {
            if (healerTarget == null || !healerTarget.gameObject.activeSelf || healerTarget == this)
            {
                anim.SetBool("walking", false);
                yield return new WaitForSeconds(Random.Range(1, 3));
                GetNextHealingTarget();
                anim.SetBool("walking", true);
                yield return null;
            }
            if (Vector3.Distance(agent.destination, transform.position) < agent.stoppingDistance + 2f)
            {
                agent.isStopped = true;
                if (Effect)
                    Effect.SetActive(true);
                anim.SetBool("walking", false);

                yield return new WaitForSeconds(Random.Range(.4f, 3f));
                int Count = Random.Range(1, 4);

                for (int i = 0; i < Count; i++)
                {
                    if (healerTarget == null || !healerTarget.gameObject.activeSelf || healerTarget == this)
                        continue;
                    transform.LookAt(healerTarget.transform);
                    anim.SetTrigger("spawn");
                    yield return new WaitForSeconds(.9f);

                    ObjectPooler.instance.SpawnFromPool("HealingEffect", healerTarget.transform.position.SetPositionY(1), Quaternion.identity);
                    healerTarget.Health += 5;

                    yield return new WaitForSeconds(healingRate);
                }
                yield return new WaitForSeconds(Random.Range(1, 3f));
                if (Effect)
                    Effect.SetActive(false);
                agent.isStopped = false;
                agent.SetDestination(player.position);
                anim.SetBool("walking", true);
            }
            yield return null;
        }
    }
    IEnumerator WizardAI()
    {
        anim.SetBool("walking", true);
        while (true)
        {
            if (Vector3.Distance(agent.destination, transform.position) < agent.stoppingDistance + .1f)
            {
                anim.SetBool("walking", false);
                transform.LookAt(player);
                yield return new WaitForSeconds(Random.Range(.4f, 3f));

                anim.SetTrigger("spawn");
                yield return new WaitForSeconds(1.8f);

                int enemyCount = Random.Range(5, 11);
                for (int i = 0; i < enemyCount; i++)
                {
                    ObjectPooler.instance.SpawnFromPool("Minions", transform.position + (Random.onUnitSphere).SetPositionY(transform.position.y), transform.rotation);
                }

                yield return new WaitForSeconds(Random.Range(1, 3f));

                agent.SetDestination(player.position + (Random.onUnitSphere).SetPositionY(transform.position.y) * Random.Range(10, 15));
                anim.SetBool("walking", true);
            }
            yield return null;
        }
    }

    void Death()
    {
        if (tracker && targetCanvasLocal.Target == this)
            tracker.gameObject.SetActive(false);
        if (gameObject.activeInHierarchy)
            StartCoroutine(_Death());
    }
    IEnumerator _Death()
    {
        ParticleSystem ps = ObjectPooler.instance.SpawnFromPool("Splatter", transform.position, transform.rotation).GetComponent<ParticleSystem>();
        ps.startColor = myColor;
        if (m_bloodType == BloodType.huge || m_enemyType == EnemyType.Wizard || m_enemyType == EnemyType.BombBlaster || m_enemyType == EnemyType.Healer)
            ObjectPooler.instance.SpawnFromPool("Blood", transform.position, transform.rotation).GetComponent<BloodDecal>().SpawnBlood(m_bloodType, myColor);
        else if (Random.value > .6f)
            ObjectPooler.instance.SpawnFromPool("Blood", transform.position, transform.rotation).GetComponent<BloodDecal>().SpawnBlood(m_bloodType, myColor);

        AudioManager.instance.PlayAudio(DeathSound.RandomItem());

        if (m_enemyType == EnemyType.BombBlaster)
        {
            yield return null;
            EZCameraShake.CameraShaker.Instance.ShakeOnce(5f, 10, .1f, 1f);
            GetComponent<Collider>().enabled = false;
            ObjectPooler.instance.SpawnFromPool("BigExplosion", transform.position.SetPositionY(1), Quaternion.identity);
            Collider[] C = Physics.OverlapSphere(transform.position, ExplosionRadius);
            for (int i = 0; i < C.Length; i++)
            {
                Ai_Script a = C[i].gameObject.GetComponent<Ai_Script>();
                //if (a.m_enemyType == EnemyType.BombBlaster)
                //{
                //    yield return null;
                //    yield return null;
                //}
                if (a != null && a != this)
                {
                    if (a.m_enemyType == EnemyType.BombBlaster)
                        Timer.Register(.1f, () => a.ReduceHealth(BlastDamage, null));
                    else
                        a.ReduceHealth(BlastDamage, null);
                    continue;
                }
                Barricade c = C[i].gameObject.GetComponent<Barricade>();
                if (c != null)
                {
                    c.Death();
                    continue;
                }
                if (C[i].GetComponent<PlayerController>() != null)
                    C[i].GetComponent<PlayerController>().GameOver();
            }
        }
        else
            EZCameraShake.CameraShaker.Instance.ShakeOnce(1f, 2.5f, .1f, .5f);
        if (m_enemyType == EnemyType.Giant)
            GameManager.instance.AdjustCoins(5);
        GameManager.instance.RegisterDeath(gameObject);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (m_enemyType == EnemyType.BombBlaster)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
        }
    }
    public void ReduceHealth(float byHowMuch, BulletScript bullet = null)
    {
        if (bullet != null)
            if (lastBullet == bullet) return;
        if (bullet != null)
            lastBullet = bullet;
        Health -= byHowMuch;

    }

    public void OnObjectSpawn()
    {
        //Start();
    }

    public void OnObjectActive()
    {

        Start();

        //RegisteredEnd = true;
        GameManager.instance.OnGameEnd += GameEnd;
    }
    //bool RegisteredEnd = false;
    void GameEnd()
    {
        if (agent.isOnNavMesh)
            agent.isStopped = true;
    }
    public enum EnemyType
    {
        Zombies, FastZombies, Wizard, Giant, BombBlaster, Healer
    }

    public void DodgeBullet()
    {
        int dir = Random.Range(0, 1);
        Vector3 direction = (dir == 0) ? Vector3.right : Vector3.left;
        float speed = gameObject.name == "Zombie Stickman" ? 0.95f : 1.0f;
        gameObject.transform.Translate(direction * speed);
    }
}
