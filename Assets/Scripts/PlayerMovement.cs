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

    [SerializeField] private float _jumpForce;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _targetRotationY = 0f;
    [SerializeField] private float _boardAdjustSpeed;
    [SerializeField] private float _playerAdjustSpeed;
    

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
            //Board adjust to normal.
            Quaternion boardTargetRotation = Quaternion.FromToRotation(_board.transform.up, hit.normal) * _board.transform.rotation;
            _board.transform.rotation = Quaternion.Lerp(_board.transform.rotation, boardTargetRotation, _boardAdjustSpeed * Time.deltaTime);

            //Player adjust to normal.
            Quaternion playerTargetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, playerTargetRotation, _playerAdjustSpeed * Time.deltaTime);

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

        if (Input.GetKey(KeyCode.D))
        {
            _targetRotationY += _rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _targetRotationY -= _rotationSpeed * Time.deltaTime;
        }

        Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _targetRotationY, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }

    void FixedUpdate()
    {
        if (_isGrounded)
        {
            Vector3 forwardDirection = _frontBoard.transform.forward;
            float yVelocity = _rb.velocity.y;
            _rb.velocity = forwardDirection * _rb.velocity.magnitude;
            _rb.velocity = new Vector3(_rb.velocity.x, yVelocity, _rb.velocity.z);
        }
    }
}
