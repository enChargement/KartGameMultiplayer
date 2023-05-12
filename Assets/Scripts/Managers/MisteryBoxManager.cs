using System.Collections.Generic;
using UnityEngine;

public class MisteryBoxManager : MonoBehaviour
{


    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private List<GameObject> weapons;

    [SerializeField] private MeshRenderer box;
    private BoxCollider colliderTrigger;
    private bool isActive = true;
    private float timer = 0f;

    void Start()
    {
        colliderTrigger = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            Timer();
        }
    }
    private GameObject GetRandomWeapon()
    {
        int randomNumber = Random.Range(0, weapons.Count);
        return weapons[randomNumber];
    }

    private bool GiveWeapon(GameObject weapon,ref Collider player)
    {
        WeaponsManager playerWeaponManager = player.GetComponent<WeaponsManager>();
        if (playerWeaponManager !=null)
        {
            return playerWeaponManager.GiveWeapon(weapon);
        }
        else
        {
            return false;
        }
    }

    private void Timer()
    {
        timer += Time.deltaTime;
        if(timer >= respawnTime)
        {
            timer = 0f;
            box.enabled = true;
            colliderTrigger.enabled = true;
            isActive = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        string colTag = other.tag;
        if( colTag.Equals("BlueTeam") || colTag.Equals("RedTeam"))
        {
            GameObject weapon = GetRandomWeapon();
            bool result = GiveWeapon(weapon, ref other);
            if (result)
            {
                box.enabled = false;
                colliderTrigger.enabled = false;
                isActive = false;
            }

        }
    }
}
