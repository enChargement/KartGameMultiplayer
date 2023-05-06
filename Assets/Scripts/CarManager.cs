using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class CarManager : MonoBehaviour
{
    Rigidbody rb;

    private float _steer = 0f;
    private float _acceleration = 0f;
    private bool _canDrift = false;
    public bool _isDrifting = false;
    private float _driftTimer = 0f;
    private bool _grounded = true;

    private Vector3 _localVelocity;

    [Header("Vehicule Parameters")]
    public float speed = 100f;
    public float turn = 5f;
    public float gravityMultiplier = 10f;
    public float SteeringForce = 100f;
    public float RealignementForce = 10f;
    public float visualSpeed = 2f;
    public float additionnalSpeed = 0f;
    public float DriftImpulseForce = 30f;

    [Space(10)]
    public Transform GroundedPoint;

    [Header("Wheels Transform")]
    public Transform FL_turn;
    public Transform FR_turn;

    [Space(10)]
    public Transform FL;
    public Transform FR;
    public Transform BL;
    public Transform BR;

    [Header("Drift Parameters")]
    [Range(0f,5f)]
    public float InversionAxisVelocity = 1f;
    [Range(0f, 1f)]
    public float ConstantTurnDrift = 0.6f;
    [Range(0f, 4f)]
    public float DriftOverSteerDivider = 2f;

    [Header("Particles Parameters")]
    private int _sparklesIndex = 0;
    public int ParticleIndex { get { return _sparklesIndex; } set { _sparklesIndex = value; } }

    public List<ParticleSystem> LeftSparkles = new List<ParticleSystem>();

    public List<ParticleSystem> RightSparkles = new List<ParticleSystem>();


    public List<TrailRenderer> TrailDrifts = new List<TrailRenderer>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _localVelocity = transform.InverseTransformDirection(rb.velocity);
        //before physic
        IsGrounded();
        GetInputs();

        //Physic
        ForwardForce();
        TurnForce();
        Drifting();
        AddGravity();
        CorrectRotation();

        Debug.Log("drift velocity : "+ _localVelocity);

        //car visuals
    }

    private void Update()
    {
        SteeringWheel();
        TurnWheel();
        CheckSparklesDrift();
    }



    #region Before Physic
    private void GetInputs()
    {
        _steer = Input.GetAxis("Horizontal");
        _acceleration = Input.GetAxis("Vertical");
        _canDrift = Input.GetKey(KeyCode.Space);
    }
    private void IsGrounded()
    {
        RaycastHit hit;
        _grounded = Physics.Raycast(GroundedPoint.position, -GroundedPoint.up, out hit, 0.1f);
        Debug.DrawRay(GroundedPoint.position, -GroundedPoint.up * .1f, Color.green);
    }

    #endregion

    #region Physic
    private void ForwardForce()
    {
        if (_acceleration > 0f && _grounded)
            rb.AddRelativeForce(Vector3.forward * (speed+additionnalSpeed));
        else if (_acceleration < 0f && _grounded)
            rb.AddRelativeForce(Vector3.forward * (-(speed/1.5f)-additionnalSpeed)); 
    }

    private void TurnForce()
    {
        
        
        if (Mathf.Abs(_localVelocity.z) > 0.1f)
        {
            float yRot = rb.transform.rotation.eulerAngles.y + _steer * SteeringForce * (_localVelocity.z > 0 ? 1 : -1) * (4-Mathf.Abs(_acceleration))/4 * Time.fixedDeltaTime;
            float lerpRot = Mathf.LerpAngle(rb.transform.rotation.eulerAngles.y, yRot, 1);
            rb.rotation = Quaternion.Euler(new Vector3(rb.transform.rotation.eulerAngles.x, yRot, rb.transform.rotation.eulerAngles.z));
        }
    }

    private void Drifting()
    {
        if(rb.velocity.magnitude < 5f || _localVelocity.z < 0 || (Mathf.Abs(_steer) < .65f && !_isDrifting))
        {
            _canDrift = false;
        }
        if (_canDrift && _grounded)
        {
            _driftTimer += Time.fixedDeltaTime;

            DriftTurn();

            InversionVelocity();

            if (_isDrifting) { return; }

            _isDrifting = true;
            PlayDriftParticles();
        }
        else
        {
            _localVelocity.x /= 5 ;
            rb.velocity = transform.TransformDirection(_localVelocity);

            if (_isDrifting)
            {
                _isDrifting = false;
                DriftImpulse();
                _sparklesIndex = -1;
                StopDriftParticles();
            }

        }
    }

    /// <summary>
    /// Defines the rotation of the car during the drift
    /// </summary>
    private void DriftTurn()
    {
        float turnDirection = (-_localVelocity.x > 0 ? 1 : -1);

        float constantTurnInDrift = SteeringForce * ConstantTurnDrift * turnDirection;

        float additionalTurn = (_steer + 1 * turnDirection) / DriftOverSteerDivider * SteeringForce;


        float yRot = rb.transform.rotation.eulerAngles.y + (constantTurnInDrift + additionalTurn) * Time.fixedDeltaTime;
        float lerpRot = Mathf.LerpAngle(rb.transform.rotation.eulerAngles.y, yRot, .8f);

        rb.rotation = Quaternion.Euler(new Vector3(rb.transform.rotation.eulerAngles.x, lerpRot, rb.transform.rotation.eulerAngles.z));
    }

    /// <summary>
    /// invere the amount of velocity between x and z axis to simulate drifting
    /// </summary>
    private void InversionVelocity()
    {
        if (_localVelocity.x < 0)
        {
            _localVelocity.x = _localVelocity.x - InversionAxisVelocity * Time.fixedDeltaTime;
        }
        else
        {
            _localVelocity.x = _localVelocity.x + InversionAxisVelocity * Time.fixedDeltaTime;
        }
        _localVelocity.z = _localVelocity.z + InversionAxisVelocity / 2 * Time.fixedDeltaTime;
        rb.velocity = transform.TransformDirection(_localVelocity);
    }

    /// <summary>
    /// add an impulse to the car in proportion to the drift length
    /// </summary>
    private void DriftImpulse()
    {
        if(_sparklesIndex == -1)
        {
            return;
        }
        rb.AddForce(transform.forward * (DriftImpulseForce + _sparklesIndex * DriftImpulseForce),ForceMode.Impulse);
        _driftTimer = 0f;
        
    }

    private void AddGravity()
    {
        rb.AddForce(Vector3.down * gravityMultiplier);
    }

    private void CorrectRotation()
    {
        
        if (!_grounded)
        {
            var carRotation = rb.transform.rotation.eulerAngles;
            var LerpX = Mathf.LerpAngle(carRotation.x, 0, RealignementForce * 0.0001f);
            var LerpZ = Mathf.LerpAngle(carRotation.z, 0, RealignementForce * 0.0001f);
            rb.rotation = Quaternion.Euler(new Vector3(LerpX,carRotation.y,LerpZ));
        }
    }
    #endregion

    #region car Visuals
    private void SteeringWheel()
    {
        var leftLocalEuler = FL_turn.localEulerAngles;
        leftLocalEuler.y = _steer * 40f;
        FL_turn.localEulerAngles = leftLocalEuler;
        FR_turn.localEulerAngles = leftLocalEuler;
    }

    private void TurnWheel()
    {

        FL.transform.Rotate(Vector3.right, _localVelocity.z * visualSpeed);
        FR.transform.Rotate(Vector3.right, _localVelocity.z * visualSpeed);
        BL.transform.Rotate(Vector3.right, _localVelocity.z * visualSpeed);
        BR.transform.Rotate(Vector3.right, _localVelocity.z * visualSpeed);
    }

    private void PlayDriftParticles()
    {
        for (int i = 0; i < 3; i++)
        {
            ParticleSystem psRight = RightSparkles[i];
            ParticleSystem psLeft = LeftSparkles[i];
            if (i == ParticleIndex)
            {
                if (!psRight.isPlaying)
                    psRight.Play();
                if (!psLeft.isPlaying)
                    psLeft.Play();
                Debug.Log("rank : " + i);
            }
            else
            {
                if (psLeft.isPlaying)
                    psLeft.Stop();
                if (psRight.isPlaying)
                    psRight.Stop();
            }
        }
        TrailDrifts.ForEach(trail => trail.emitting = true);
    }

    private void SetSparklesIndex(int index)
    {
        if(index == _sparklesIndex)
        {
            return;
        }
        _sparklesIndex = index;
        PlayDriftParticles();
    }

    private void StopDriftParticles()
    {
        LeftSparkles.ForEach(item => item.Stop());
        RightSparkles.ForEach(item => item.Stop());
        TrailDrifts.ForEach(trail => trail.emitting = false);
    }

    private void CheckSparklesDrift()
    {
        if (!_isDrifting) return;

        if (_driftTimer < 1.5f) { SetSparklesIndex(-1); }
        else if (_driftTimer < 2.5f) { SetSparklesIndex(0); }
        else if (_driftTimer < 4f) { SetSparklesIndex(1); }
        else if (_driftTimer >= 4) { SetSparklesIndex(2); }
    }

    #endregion

}
