using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject _board;
    [SerializeField] private GameObject _frontBoard;
    [SerializeField] private GameObject _rearBoard;
    private Rigidbody _rb;
    private bool _isGrounded;


    [SerializeField] private float rotationSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _turnForce;

    [Header("Raycast")]
    [SerializeField] private LayerMask _terrainLayer;
    [SerializeField] private float _raycastDistance;

    [Header("Debug")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI isGroundedText;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, _raycastDistance, _terrainLayer))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(_board.transform.up, hit.normal) * _board.transform.rotation;
            
            _board.transform.rotation = Quaternion.Lerp(_board.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //transform.rotation = targetRotation;



            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }

        speedText.text = "Speed: " + _rb.velocity.magnitude;
        isGroundedText.text = "Is Grounded: " + _isGrounded;

        if(Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            
        }
    }

    void FixedUpdate()
    {
        if (_isGrounded)
        {
            Vector3 forwardDirection = _frontBoard.transform.forward;
            float yVelocity = _rb.velocity.y; // Guardar la velocidad en el eje Y
            _rb.velocity = forwardDirection * _rb.velocity.magnitude;
            _rb.velocity = new Vector3(_rb.velocity.x, yVelocity, _rb.velocity.z); // Asignar la velocidad en el eje Y nuevamente
        }
    }

    public Vector3 ComputeTorque(Quaternion desiredRotation){
        //q will rotate from our current rotation to desired rotation
        Quaternion q = desiredRotation * Quaternion.Inverse(transform.rotation);
        //convert to angle axis representation so we can do math with angular velocity
        Vector3 x;
        float xMag;
        q.ToAngleAxis (out xMag, out x);
        x.Normalize ();
        //w is the angular velocity we need to achieve
        Vector3 w = x * xMag * Mathf.Deg2Rad / Time.fixedDeltaTime;
        w -= _rb.angularVelocity;
        //to multiply with inertia tensor local then rotationTensor coords
        Vector3 wl = transform.InverseTransformDirection (w);
        Vector3 Tl;
        Vector3 wll = wl;
        wll = _rb.inertiaTensorRotation * wll;
        wll.Scale(_rb.inertiaTensor);
        Tl = Quaternion.Inverse(_rb.inertiaTensorRotation) * wll;
        Vector3 T = transform.TransformDirection (Tl);
        return T;
    }
}
