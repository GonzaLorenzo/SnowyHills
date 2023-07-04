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

    [SerializeField] private GameObject _currentBoard;

    private Rigidbody _rb;
    private bool _isGrounded;
    private bool _launchRaycast = true;

    //BOOL DE TESTEO.
    public bool hasLost = false;
    //BOOL DE TESTEO.

    [SerializeField] private float _jumpForce;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _airRotationSpeed;
    private float _targetRotationY = 0f;
    private float _targetRotationX = 0f;
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
            }
        }
        else
        {
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
            if(_isGrounded)
            {
                _targetRotationY -= _rotationSpeed * Time.deltaTime;
            }   
            else
            {
                _targetRotationY -= _airRotationSpeed * Time.deltaTime;
            }
            
        }
        /* if(!_isGrounded && Input.GetKey(KeyCode.S)) Encontrar la forma de rotar en eje x en el aire.
        {
            _targetRotationX += _airRotationSpeed * Time.deltaTime;
        }
        if(!_isGrounded && Input.GetKey(KeyCode.W))
        {
            _targetRotationX -= _airRotationSpeed * Time.deltaTime;
        } */

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
            //Vector3 forwardDirection = _frontBoard.transform.forward;
            Vector3 forwardDirection = _currentBoard.transform.forward;
            float yVelocity = _rb.velocity.y;
            _rb.velocity = forwardDirection * _rb.velocity.magnitude;
            _rb.velocity = new Vector3(_rb.velocity.x, yVelocity, _rb.velocity.z);

            //Probar agregar una fuerza hacia el forwardDirection. Para que sea mas pesado el giro.
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
        /* if(_frontBoard.transform.position.y < _rearBoard.transform.position.y)
        {
            _currentBoard = _frontBoard;
        }
        else
        {
            _currentBoard = _rearBoard;
        } */

        //Funciona mejor pero no es esto.
        if(Vector3.Distance(_frontBoard.transform.position, _rb.velocity) < Vector3.Distance(_rearBoard.transform.position, _rb.velocity))
        {
            _currentBoard = _rearBoard;
        }
        else
        {
            _currentBoard = _frontBoard;
        }
    }

    IEnumerator StopRaycast()
    {
        _launchRaycast = false;
        yield return new WaitForSeconds(0.5f);
        _launchRaycast = true;
    }
}
