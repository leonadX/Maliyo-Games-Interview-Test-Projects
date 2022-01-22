using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormy : MonoBehaviour
{
    public Rigidbody2D bulletPrefab;
    public Transform currentGun;
    public VJHandler jsMovement;
    public VJHandler jsAim;

    public bool moveLeft = false;
    public bool moveRight = false;
    public float rotateSpeed = 500f;
    public float wormySpeed = 1;
    public float maxRelativeVelocity;
    public float misileForce = 5; 

    public bool IsTurn { get { return WormyManager.singleton.IsMyTurn(wormId); } }

    public bool fire { get { return WormyManager.singleton.fire; } }
    public int wormId;
    WormyHealth wormyHealth;
    SpriteRenderer ren;

    private void Start()
    {
        wormyHealth = GetComponent<WormyHealth>();
        ren = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!IsTurn)
            return;

        RotateGun();

        var hor = Input.GetAxis("Horizontal");
        float movedir = Mathf.Sqrt(Mathf.Pow(jsMovement.InputDirection.x, 2) + Mathf.Pow(jsMovement.InputDirection.y, 2));
        float aimdir = Mathf.Sqrt(Mathf.Pow(jsAim.InputDirection.x, 2) + Mathf.Pow(jsAim.InputDirection.y, 2));

        //var hor = 0;
        if (jsMovement.InputDirection.x > 0.9 && movedir >= 0.9)
            hor = 1;
        else if (jsMovement.InputDirection.x < -0.9 && movedir >= 0.9)
            hor = -1;

        if (aimdir >= 0.9)
            WormyManager.singleton.fire = true;

        if (hor == 0)
        {
            currentGun.gameObject.SetActive(true);

            ren.flipX = currentGun.eulerAngles.z < 180;

            //StartCoroutine(TimeBeforeFire());
            if (Input.GetKeyDown(KeyCode.Q) || fire){
                var p = Instantiate(bulletPrefab,
                                   currentGun.position - currentGun.right,
                                   currentGun.rotation);

                p.AddForce(-currentGun.right * misileForce, ForceMode2D.Impulse);

                if (IsTurn)
                    WormyManager.singleton.NextWorm();
            }
        }
        else
        {
            currentGun.gameObject.SetActive(false);
            transform.position += Vector3.right *
                                hor *
                                Time.deltaTime *
                                wormySpeed;            
             ren.flipX = Input.GetAxis("Horizontal") > 0;
        }


    }

    void RotateGun()
    {
        Vector3 dir = jsAim.InputDirection;
        var diff = Camera.main.ScreenToWorldPoint(dir * rotateSpeed) - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        currentGun.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > maxRelativeVelocity)
        {
            wormyHealth.ChangeHealth(-3);
            if (IsTurn)
                WormyManager.singleton.NextWorm();
        }  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion"))
        {
            wormyHealth.ChangeHealth(-10);
            if (IsTurn)
                WormyManager.singleton.NextWorm();
        }
            
    }
}
