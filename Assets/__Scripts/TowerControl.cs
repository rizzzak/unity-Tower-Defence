using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour
{
    [Header("Set in Inspector")]
    //public float timeBetweenAttacks;
    //public float timeAttackDone;
    
    public float attackPeriod;
    public float attackRadius;
    public Projectile projectile;
    public float lowestDistToEnemy = 0.2f;

    [Header("Set Dynamically")]
    //public float attackCounter;
    public float timeAttackNext;
    public float timeAfterLastAttack;
    public Enemy target;
    public bool isAttacking = false;
    private void Awake()
    {
        timeAfterLastAttack = 0f;
    }
    public void Update()
    {
        timeAfterLastAttack += Time.deltaTime;

        //выбор ближайшего врага, если цели нет / уничтожена
        if(target == null || target.isDead)
        {
            Enemy enemy = GetNearestEnemy();
            if (enemy != null)
                target = enemy;
            else
                target = null;
        }
        //если цель есть
        else
        {
            if (Time.time > timeAttackNext)
            {
                isAttacking = true;
                timeAttackNext = Time.time + attackPeriod;
                timeAfterLastAttack = 0f;
            }
            else isAttacking = false;
            if (isAttacking == true)
                Attack();
            if ((target.transform.position - transform.position).magnitude > attackRadius)
                target = null;
        }
        
    }
    private void FixedUpdate()
    {
        
    }
    public void Attack()
    {
        isAttacking = false;
        Projectile newProjectile = Instantiate(projectile) as Projectile;
        newProjectile.transform.position = transform.position;
        if(newProjectile.eType == ProjectileType.arrow)
        {
            Manager.instance.audioSource.PlayOneShot(SoundManager.instance.hit);
        }
        else if (newProjectile.eType == ProjectileType.rock)
        {
            Manager.instance.audioSource.PlayOneShot(SoundManager.instance.rock);
        }
        else if (newProjectile.eType == ProjectileType.fireball)
        {
            Manager.instance.audioSource.PlayOneShot(SoundManager.instance.fireball);
        }
        if(target == null && projectile != null)
        {
            Destroy(newProjectile.gameObject);
        }
        else
        {
            //move projectile towards target enemy
            StartCoroutine(MoveProjectile(newProjectile));
        }
    }
    IEnumerator MoveProjectile(Projectile projectile)
    {
        while(GetTargetDistance()>0f && target != null && projectile != null)
        {
            var dir = target.transform.position - transform.position;
            var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            projectile.transform.position = Vector2.MoveTowards(projectile.transform.position
                , target.transform.position, projectile.speed * Time.deltaTime);
            yield return null;
        }
        if(target == null && projectile != null)
        {
            Destroy(projectile.gameObject);
        }
    }
    float GetTargetDistance()
    {
        if(target == null)
            return 0f;
        float distance = (transform.position - target.transform.position).magnitude;
        if ((transform.position - target.transform.position).magnitude < lowestDistToEnemy)
            return 0f;
        return distance;
    }
    List<Enemy> GetEnemiesInRadius()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();
        foreach (Enemy enemy in Manager.instance.enemyList)
        {
            if ((transform.position - enemy.transform.position).magnitude <= attackRadius)
                enemiesInRange.Add(enemy);
        }
        return enemiesInRange;
    }
    Enemy GetNearestEnemy()
    {
        Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity;
        foreach (Enemy enemy in GetEnemiesInRadius())
        {
            if (enemy.isDead)
                continue;
            if ((transform.position - enemy.transform.position).magnitude <= smallestDistance)
            {
                smallestDistance = (transform.position - enemy.transform.position).magnitude;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
}
