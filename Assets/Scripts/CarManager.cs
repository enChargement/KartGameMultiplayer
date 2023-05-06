using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class CarManager : MonoBehaviour
{
    Rigidbody rb;

    private float _steer = 0f;
    private float _acceleration = 0f;
    private bool _drift = false;
    private bool _grounded = true;

    private Vector3 _localVelocity;

    public float speed = 100f;
    public float turn = 5f;
    public float gravityMultiplier = 10f;
    public float SteeringForce = 100f;
    public float RealignementForce = 10f;
    public float visualSpeed = 2f;
    public float additionnalSpeed = 0f;

    public Transform FL_turn;
    public Transform FR_turn;

    public Transform FL;
    public Transform FR;
    public Transform BL;
    public Transform BR;

    public int ParticleIndex = 0;

    public float TempInversionVelocity = 1f;

    public List<ParticleSystem> LeftSparkles = new List<ParticleSystem>();

    public List<ParticleSystem> RightSparkles = new List<ParticleSystem>();


    public List<TrailRenderer> TrailDrifts = new List<TrailRenderer>();


    public Transform GroundedPoint;



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
        SteeringWheel();
        TurnWheel();
    }
    #region Before Physic
    private void GetInputs()
    {
        _steer = Input.GetAxis("Horizontal");
        _acceleration = Input.GetAxis("Vertical");
        _drift = Input.GetKey(KeyCode.Space);
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
            rb.AddRelativeForce(Vector3.forward * (-speed-additionnalSpeed)); 
    }

    private void Drifting()
    {
        /*if(rb.velocity.magnitude < 6.2f)
        {
            _drift = false;
        }*/
        if (_drift && _grounded)
        {
            float steeringSens = _steer != 0 ? _steer : -_localVelocity.x;

            float yRot = rb.transform.rotation.eulerAngles.y + (SteeringForce * 0.6f + Mathf.Abs(_steer) * SteeringForce) * (steeringSens > 0 ? 1 : -1) * (_localVelocity.z > 0 ? 1 : -1) * Time.fixedDeltaTime;

            rb.rotation = Quaternion.Euler(new Vector3(rb.transform.rotation.eulerAngles.x, yRot, rb.transform.rotation.eulerAngles.z));
            if (_localVelocity.x < 0)
            {
                _localVelocity.x = _localVelocity.x - TempInversionVelocity * Time.fixedDeltaTime;
            }
            else
            {
                _localVelocity.x = _localVelocity.x + TempInversionVelocity * Time.fixedDeltaTime;
            }
            _localVelocity.z = _localVelocity.z + TempInversionVelocity * Time.fixedDeltaTime;
            rb.velocity = transform.TransformDirection(_localVelocity);
            for (int i = 0; i < 3; i++)
            {
                ParticleSystem psRight = RightSparkles[i];
                ParticleSystem psLeft = LeftSparkles[i];
                if (i == ParticleIndex)
                {
                    psRight.Play();
                    psLeft.Play();
                    Debug.Log("rank : " + i);
                }
                else
                {
                    psRight.Stop();
                    psLeft.Stop();
                }
            }
            TrailDrifts.ForEach(trail => trail.emitting = true);
            
        }

        else
        {
            _localVelocity.x = 0;
            rb.velocity = transform.TransformDirection(_localVelocity);
            LeftSparkles.ForEach(item => item.Stop());
            RightSparkles.ForEach(item => item.Stop());
            TrailDrifts.ForEach(trail => trail.emitting = false);
        }
    }

    private void TurnForce()
    {
        
        
        if (Mathf.Abs(_localVelocity.z) > 0.1f)
        {
            float yRot = rb.transform.rotation.eulerAngles.y + _steer * SteeringForce * (_localVelocity.z > 0 ? 1 : -1) * (4-Mathf.Abs(_acceleration))/4 * Time.fixedDeltaTime;
            float lerpRot = Mathf.LerpAngle(rb.transform.rotation.eulerAngles.y, yRot, 10);
            rb.rotation = Quaternion.Euler(new Vector3(rb.transform.rotation.eulerAngles.x, lerpRot, rb.transform.rotation.eulerAngles.z));
        }
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
        leftLocalEuler.y = _steer * 30f;
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

    #endregion

}
