using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    arrow, fireball, rock
};
public class Projectile : MonoBehaviour
{
    [SerializeField]
    ProjectileType _eType;

    [SerializeField]
    int _damage;
    [SerializeField]
    float _speed;

    public ProjectileType eType { get { return _eType; } }
    public int damage { get { return _damage; } }
    public float speed { get { return _speed; } }
}
