using UnityEngine;


public class CanonBall : Weapon
{

    [SerializeField] private GameObject explosion;
    MeshRenderer ballRenderer;

    private float _damage = 60f;
    private float _velocity = 0f;

    protected override float Damage { get { return _damage; } set { _damage = value; } }
    protected override float Velocity { get { return _velocity; } set { _velocity = value; } }

    void Start()
    {
        ballRenderer = GetComponent<MeshRenderer>();  
    }

    private void OnCollisionEnter(Collision collision)
    {
        string colTag = collision.gameObject.tag;
        if(colTag.Equals("Ground") || colTag.Equals("RedTeam") || colTag.Equals("BlueTeam"))
        {
            ballRenderer.enabled = false;
            explosion.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.tag;
        if (otherTag.Equals("Ground"))
        {
            Destroy(gameObject);
        }
        else
        {
            CheckTags(otherTag,other);
        }
    }
}
