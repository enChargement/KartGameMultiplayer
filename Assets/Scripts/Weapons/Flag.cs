using UnityEngine;

public class Flag : Weapon
{
    private float _damage = 0f;
    private float _velocity = 0f;

    [SerializeField] private Collider _flagCollider;
    [SerializeField] private Transform _blueSpawn;
    [SerializeField] private Transform _redSpawn;

    protected override float Damage { get { return _damage; } set { _damage = value; } }
    protected override float Velocity { get { return _velocity; } set { _velocity = value; } }

    private void ReturnToBase()
    {
        transform.position = Vector3.zero; //teleport to flag position
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
                    WeaponsManager manager = other.GetComponent<WeaponsManager>();
                    if (manager != null)
                    {
                        _flagCollider.enabled = false;
                        manager.GiveWeapon(gameObject);
                        Destroy(gameObject);
                    }
                }
                else if (otherTag.Equals("RedTeam"))
                {
                    
                    ReturnToBase();

                }
                break;
            case "BlueTeam":
                if (otherTag.Equals("RedTeam"))
                {
                    WeaponsManager manager = other.GetComponent<WeaponsManager>();
                    if (manager != null)
                    {
                        _flagCollider.enabled = false;
                        manager.GiveWeapon(gameObject);
                        Destroy(gameObject);
                    }
                }
                else if (otherTag.Equals("BlueTeam"))
                {
                    
                    ReturnToBase();
                }
                break;
        }
    }

}
