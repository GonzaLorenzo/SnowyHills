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
    [SerializeField] private ParticleSystem _snowTrail;
    private GameObject _currentBoard;

    private Rigidbody _rb;
    private bool _isGrounded;
    private bool _launchRaycast = true;

    //BOOL DE TESTEO.
    private bool hasLost = false;
    private bool switchGamemode = false;
    //BOOL DE TESTEO.

    [Header("Movement Numbers")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _forwardForce;
    [SerializeField] private float _backwardsForce;
    [SerializeField] private float _parallelForce;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _airRotationSpeed;
    private float _targetRotationY = 0f;
    private float _targetRotationX = 0f;

    [Header("Motion Adjustment")]
    [SerializeField] private float _boardAdjustSpeed;
    [SerializeField] private float _playerAdjustSpeed;
    

    [Header("Raycast")]
    [SerializeField] private LayerMask _terrainLayer;
    [SerializeField] private float _raycastDistance;

    [Header("Debug")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI isGroundedText;
    public TextMeshProUGUI hasLostText;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, _raycastDistance, _terrainLayer) && _launchRaycast)
        {
            //Board adjust to normal.
            Quaternion boardTargetRotation = Quaternion.FromToRotation(_board.transform.up, hit.normal) * _board.transform.rotation;
            _board.transform.rotation = Quaternion.Lerp(_board.transform.rotation, boardTargetRotation, _boardAdjustSpeed * Time.deltaTime);

            //Player adjust to normal.
            Quaternion playerTargetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, playerTargetRotation, _playerAdjustSpeed * Time.deltaTime);

            if(!_isGrounded)
            {
                CheckCurrentBoard();
                _isGrounded = true;
                _snowTrail.Play();
            }
        }
        else
        {
            if(_snowTrail.isPlaying)
            {
                _snowTrail.Stop();
            }
            _isGrounded = false;
        }

        speedText.text = "Speed: " + _rb.velocity.magnitude;
        velocityText.text = "Velocity: " + _rb.velocity;
        isGroundedText.text = "Is Grounded: " + _isGrounded;

        if(_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            StartCoroutine(StopRaycast());
        }

        if (Input.GetKey(KeyCode.D))
        {
            _rb.AddForce(-_currentBoard.transform.forward * _backwardsForce * Time.deltaTime, ForceMode.Force);
            _rb.AddForce(_currentBoard.transform.right * _parallelForce * Time.deltaTime, ForceMode.Force);
            if(_isGrounded)
            {
                _targetRotationY += _rotationSpeed * Time.deltaTime;
            }
            else
            {
                _targetRotationY += _airRotationSpeed * Time.deltaTime;
            }
            
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _rb.AddForce(-_currentBoard.transform.forward * _backwardsForce * Time.deltaTime, ForceMode.Force);
            _rb.AddForce(-_currentBoard.transform.right * _parallelForce * Time.deltaTime, ForceMode.Force);
            if(_isGrounded)
            {
                _targetRotationY -= _rotationSpeed * Time.deltaTime;
            }   
            else
            {
                _targetRotationY -= _airRotationSpeed * Time.deltaTime;
            }
            
        }

        //_targetRotationY = Mathf.Clamp(_targetRotationY, -45f, 45f); Sirve pero hay que poner los numeros a mano en la montaña final

        if(_isGrounded)
        {
            Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _targetRotationY, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _targetRotationY, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _airRotationSpeed);
        }
    }

    void FixedUpdate()
    {
        if (_isGrounded)
        {
            if (_rb.velocity.magnitude > _maxSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxSpeed;
            }

            Vector3 forwardDirection = _currentBoard.transform.forward;
            _rb.AddForce(forwardDirection * _forwardForce * Time.deltaTime, ForceMode.Force);
        }
    }

    private void ChangeCurrentBoard()
    {
        if(_currentBoard = _frontBoard)
        {
            _currentBoard = _rearBoard;
        }
        else
        {
            _currentBoard = _frontBoard;
        }
    }

    private void CheckCurrentBoard()
    {
        //Prueba con producto punto. Parece ser esto.
        Vector3 velocityDirection = _rb.velocity.normalized;

        float frontDot = Vector3.Dot(_frontBoard.transform.forward, velocityDirection);
        float rearDot = Vector3.Dot(_rearBoard.transform.forward, velocityDirection);

        if (frontDot > rearDot)
        {
            _currentBoard = _frontBoard;
        }
        else
        {
            _currentBoard = _rearBoard;
        }
    }

    IEnumerator StopRaycast()
    {
        _launchRaycast = false;
        yield return new WaitForSeconds(0.5f);
        _launchRaycast = true;
    }
}
