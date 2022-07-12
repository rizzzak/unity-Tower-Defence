using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int reward;
    public Transform exit;
    public Transform[] wayPoints;
    public Collider2D colld;
    public Animator anim;

    public int target = 0; // reached goal
    public int health = 1;
    public bool isDead;
    Transform enemy;


    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Transform>();
        colld = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        Manager.instance.RegisterEnemy(this);
        isDead = false;
    }
    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        if(wayPoints != null && !isDead)
        {
            MoveEnemy(step);
        }
    }
    void MoveEnemy(float step)
    {
        if (target < wayPoints.Length)
        {
            enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, step);
        }
        else
        {
            enemy.position = Vector2.MoveTowards(enemy.position, exit.position, step);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "MovingPoint")
        {
            target++;
        }
        else if(other.tag == "Finish")
        {
            Manager.instance.escapedTotal++;
            Manager.instance.escaped++;
            Manager.instance.UnregisterEnemy(this);
            Manager.instance.IsWaveOver();
        }
        else if(other.tag == "Projectile")
        {
            Projectile hitProj = other.gameObject.GetComponent<Projectile>();
            if (hitProj == null)
                Debug.Log("Null hit Proj in Enemy");
            else
            {
                EnemyHit(hitProj.damage);
                Destroy(other.gameObject);
            }
            
        }
    }
    void EnemyHit(int hitDamage)
    {
        if(health > hitDamage)
        {
            //hurt
            anim.Play("Hurt");
            health -= hitDamage;
            Manager.instance.audioSource.PlayOneShot(SoundManager.instance.hit);
        }
        else
        {
            //die
            anim.SetTrigger("DidDie");
            EnemyDie();
            Manager.instance.audioSource.PlayOneShot(SoundManager.instance.death);
        }
    }
    void EnemyDie()
    {
        isDead = true;
        colld.enabled = false;
        Manager.instance.killed++;
        Manager.instance.AddMoney(reward);
        Manager.instance.IsWaveOver();
    }
}
