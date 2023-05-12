using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ProjectileLifeTime : MonoBehaviour
{
    [SerializeField] private float TimeBeforeDestroy = 20f;
    private float _timer = 0f;
    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if( _timer >= TimeBeforeDestroy)
        {
            Destroy(gameObject);
        }
    }
}
