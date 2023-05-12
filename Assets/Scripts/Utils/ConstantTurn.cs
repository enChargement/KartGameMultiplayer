using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantTurn : MonoBehaviour
{
    [SerializeField] private float _turnAngle = 25f;
    void Update()
    {
        transform.Rotate(Vector3.up,_turnAngle * Time.deltaTime);
    }
}
