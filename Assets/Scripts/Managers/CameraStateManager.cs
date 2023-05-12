using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraStateManager : MonoBehaviour
{

    CinemachineVirtualCamera _camera;
    [SerializeField] CarManager _carManager;
    [SerializeField] private float _dutchMax = 1f;
    [SerializeField] private float _turboFOV = 75;
    private bool _fovChange;
    void Start()
    {
       _camera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isTurbo = _carManager.IsTurbo();
        _camera.m_Lens.FieldOfView = isTurbo ? Mathf.Lerp(_camera.m_Lens.FieldOfView, _turboFOV, .2f) : Mathf.Lerp(_camera.m_Lens.FieldOfView, 60, .2f);
        _camera.m_Lens.Dutch = isTurbo ? Random.Range(-_dutchMax, _dutchMax) : 0;
    }
}
