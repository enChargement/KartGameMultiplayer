using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class CarManager : NetworkBehaviour
{
    Rigidbody rb;

    private float _steer = 0f;
    private float _acceleration = 0f;
    public bool IsDead = false;
    private bool _canDrift = false;
    private bool _isDrifting = false;
    private bool _isTurbo = false;
    private float _driftTimer = 0f;
    private bool _grounded = true;
    private bool _needRotCorrection = false;

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

    public Transform GroundedPointLeft;
    public Transform GroundedPointRight;
    [SerializeField] private GameObject camera;

    [Header("Visual Transform")]
    public Transform VisualCar;
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
    public float TimeForTurbo1 = 1.5f;
    public float TimeForTurbo2 = 2.5f;
    public float TimeForTurbo3 = 4.0f;

    [Space(10)]

    [Header("GUI Parameters")]
    public TextMeshProUGUI TimerText;


    [Space(10)]

    [Header("Particles Parameters")]
    private int _sparklesIndex = 0;
    public int ParticleIndex { get { return _sparklesIndex; } set { _sparklesIndex = value; } }

    public List<ParticleSystem> LeftSparkles = new List<ParticleSystem>();

    public List<ParticleSystem> RightSparkles = new List<ParticleSystem>();

    public List<TrailRenderer> TrailDrifts = new List<TrailRenderer>();

    public List<ParticleSystem> ExhaustFlames = new List<ParticleSystem>();

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();
        if (IsOwner)
        {
            camera.SetActive(true);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;
        if (IsDead) return; 
        _localVelocity = transform.InverseTransformDirection(rb.velocity);
        //before physic
        IsGrounded();

        //Physic
        ForwardForce();
        TurnForce();
        Drifting();
        AddGravity();
        CorrectRotation();


        //car visuals
    }

    private void Update()
    {
        if (!IsOwner) return;

        GetInputs();
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

        bool left = Physics.Raycast(GroundedPointLeft.position, -GroundedPointLeft.up, out RaycastHit hitleft, 0.1f);
        bool right = Physics.Raycast(GroundedPointRight.position, -GroundedPointLeft.up, out RaycastHit hitright, 0.1f);
        if (left && hitleft.collider.tag.Equals("Wall"))
        {
            left = false;
        }
        if (right && hitright.collider.tag.Equals("Wall"))
        {
            right = false;
        }
        _grounded = left || right;
        _needRotCorrection = !left || !right;
        Debug.DrawRay(GroundedPointLeft.position, -GroundedPointLeft.up * .1f, Color.green);
        Debug.DrawRay(GroundedPointRight.position, -GroundedPointLeft.up * .1f, Color.green);
    }

    public IEnumerator DeadState()
    {
        IsDead = true;
        rb.isKinematic = true;
        VisualCar.gameObject.SetActive(false);
        var colliders = GetComponents<BoxCollider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        for (int i = 5; i>=0; i--)
        {
            TimerText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        TimerText.text = "";
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        VisualCar.gameObject.SetActive(true);
        rb.isKinematic = false;
        IsDead = false;
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

    public bool IsTurbo()
    {
        return _isTurbo;
    }

    public bool IsDrifting()
    {
        return _isDrifting;
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
        _localVelocity.z = _localVelocity.z + InversionAxisVelocity/3 * Time.fixedDeltaTime;
        rb.velocity = transform.TransformDirection(_localVelocity);
    }

    /// <summary>
    /// add an impulse to the car in proportion to the drift length
    /// </summary>
    private void DriftImpulse()
    {
        if(_sparklesIndex == -1 || rb.velocity.magnitude < 2f)
        {
            _sparklesIndex = -1;
            _driftTimer = 0f;
            return;
        }
        rb.AddForce(transform.forward * (30f + _sparklesIndex * DriftImpulseForce),ForceMode.Impulse);
        _driftTimer = 0f;
        StartCoroutine(SetExhaustsFlame(_sparklesIndex));
        
    }

    private void AddGravity()
    {
        //rb.AddForce(Vector3.down * gravityMultiplier);

        Debug.DrawRay(transform.position, Vector3.down * gravityMultiplier, Color.green);
    }

    private void CorrectRotation()
    {
        
        if (_needRotCorrection)
        {
           /* Debug.Log("Rotation correction");
            var carRotation = rb.transform.rotation.eulerAngles;
            var LerpX = Mathf.LerpAngle(carRotation.x, 0, RealignementForce * 0.0001f);
            var LerpZ = Mathf.LerpAngle(carRotation.z, 0, RealignementForce * 0.0001f);
            rb.rotation = Quaternion.Euler(new Vector3(LerpX,carRotation.y,LerpZ));*/
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
        float angle = rb.velocity.magnitude * (_localVelocity.z > 0 ? 1 : -1) * visualSpeed;
        float multiplier = _isDrifting ? 1.5f : 1;
        FL.transform.Rotate(Vector3.right, angle);
        FR.transform.Rotate(Vector3.right, angle);
        BL.transform.Rotate(Vector3.right, angle * multiplier);
        BR.transform.Rotate(Vector3.right, angle * multiplier);
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

        if (_driftTimer < TimeForTurbo1) { SetSparklesIndex(-1); }
        else if (_driftTimer < TimeForTurbo2) { SetSparklesIndex(0); }
        else if (_driftTimer < TimeForTurbo3) { SetSparklesIndex(1); }
        else if (TimeForTurbo3 <= _driftTimer) { SetSparklesIndex(2); }
    }

    private IEnumerator SetExhaustsFlame(int index)
    {
        _isTurbo = true;
        ExhaustFlames.ForEach(ps => ps.Play());
        yield return new WaitForSeconds(.5f + index * .5f);
        ExhaustFlames.ForEach(ps => ps.Stop());
        _isTurbo = false;
    }

    #endregion

}
