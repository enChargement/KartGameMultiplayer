using System.Collections.Generic;
using UnityEngine;

public class Turret : Weapon
{
    private float _damage = 15f;
    private float _velocity = 0f;

    List<GameObject> _listEnemy = new List<GameObject>();
    GameObject _enemy;
    protected override float Damage { get { return _damage; } set { _damage = value; } }
    protected override float Velocity { get { return _velocity; } set { _velocity = value; } }
    private float nextActionTime = 0.0f;
    public float period = 1f;

    void Update()
    { 
        FireEachSecond();
    }

    public GameObject GetEnnemyTransform()
    {
        return _enemy;
    }

    public bool IsTherAnEnemy()
    {
        return _enemy;
    }

    private void SetCurrentEnemy()
    {
        if( _listEnemy.Count > 0)
        {
            WeaponsManager enemyWeaponManager = _listEnemy[0].GetComponent<WeaponsManager>();
            float health = enemyWeaponManager.GetHealth();
            _enemy = _listEnemy[0];
            _listEnemy.ForEach(enemy => { 
                WeaponsManager weaponManager = enemy.GetComponent<WeaponsManager>();
                float newEnemyHealth = weaponManager.GetHealth();
                if (newEnemyHealth<health) { _enemy = enemy;
                    health = newEnemyHealth;
                } 
            });

        }
        else
        {
            _enemy = null;
        }
    }

    private void removeNull()
    {
        for(int i = 0; i < _listEnemy.Count; i++)
        {
            Debug.Log(_listEnemy[i]);
            if (_listEnemy[i] == null)
            {
                Debug.Log("enemy removed cause null");
                _listEnemy.RemoveAt(i);

                break;
            }
        }
    }

    private void FireEachSecond()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;

            DamagePlayer(_enemy);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.tag;
        string myTag = tag;
        switch (myTag)
        {
            case "RedTeam":
                if (otherTag.Equals("BlueTeam"))
                {
                    _listEnemy.Add(other.gameObject);
                    SetCurrentEnemy();
                }
                else if (otherTag.Equals("RedTeam"))
                {
                    return;
                }
                break;
            case "BlueTeam":
                if (otherTag.Equals("RedTeam"))
                {
                    _listEnemy.Add(other.gameObject);
                    SetCurrentEnemy();

                }
                else if (otherTag.Equals("BlueTeam"))
                {
                    return;
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        removeNull();
        string otherTag = other.tag;
        string myTag = tag;
        switch (myTag)
        {
            case "RedTeam":
                if (otherTag.Equals("BlueTeam"))
                {
                    _listEnemy.Remove(other.gameObject);
                    SetCurrentEnemy();

                }
                else if (otherTag.Equals("RedTeam"))
                {
                    return;
                }
                break;
            case "BlueTeam":
                if (otherTag.Equals("RedTeam"))
                {
                    _listEnemy.Remove(other.gameObject);
                    SetCurrentEnemy();


                }
                else if (otherTag.Equals("BlueTeam"))
                {
                    return;
                }
                break;
        }
    }
}
