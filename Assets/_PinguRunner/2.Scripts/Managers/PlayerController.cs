using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller of player movement and properties
/// </summary>

public enum CoursePosition { Left = 0 , Midle = 1, Right = 2}

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [Header("Movement Properties")]
    [Tooltip("The high the player will get"), SerializeField]
    private float _jumpFoce = 4f;
    [Tooltip("The force with the player will fall"), SerializeField]
    private float _gravity = 12f;
    [Tooltip("The start speed the player will run"), SerializeField]
    private float _originalSpeed = 7.0f;
    [Tooltip("The speed the player will rotate"), SerializeField]
    private float _turnSpeed = 0f;
    [Tooltip("Time to increase the speed"), SerializeField]
    private float _speedIncreaseTime = 2.5f;
    [Tooltip("Amount to increase the speed"), SerializeField]
    private float _speedIncreaseAmount = 0.1f;
    [Tooltip("the x distance the player will move"), SerializeField]
    private float _laneDistance = 2.0f;

    [Space(), Tooltip("Animator Controller of player Gameobject"), SerializeField]
    private Animator _playerAnimator = null;
    [Space(), Tooltip("Character Controller component"), SerializeField]
    private CharacterController _characterController = null;


    //reference to calculated velocity the player will fall
    private float _verticalVelocity;
    //enum with the actual lane position. It starts in the midle because the player it's at origin
    private CoursePosition _desiredLane = CoursePosition.Midle;

    private bool _isRunning = false;
    //time of last tick of speed
    private float _speed;

    

    void Start()
    {
        _speed = _originalSpeed;
    }

    void Update()
    {
        if (!_isRunning) return;

        //Get the input of keyboard to controle wich lane we want to move
        if (MobileInputManager.Instance.SwipeLeft)
        {
            CalculateLaneToMove(false);
        }

        if (MobileInputManager.Instance.SwipeRight)
        {
            CalculateLaneToMove(true);
        }
        //Get the position and move to that position
        MoveAndRotate();
    }

    //set the conditional to begin the execution of functions of movement in update
    public void StartRunning()
    {
        _isRunning = true;
        _playerAnimator.SetTrigger("StartRunning");
        StartCoroutine(ImproveSpeed());
    }

    private void MoveAndRotate()
    {
        //Calculate where we should be
        Vector3 targetPosition = transform.localPosition.z * Vector3.forward;
        switch (_desiredLane)
        {
            case CoursePosition.Left:
                targetPosition += Vector3.left * _laneDistance;
                break;

            case CoursePosition.Right:
                targetPosition += Vector3.right * _laneDistance;
                break;
        }

        //Calculating the delta to move
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.localPosition).normalized.x * _speed;

         bool isGrounded = IsGrounded();
        _playerAnimator.SetBool("Grounded", isGrounded);
        //Calculate y in case of jump
        if(isGrounded)
        {   
            moveVector.y = -0.1f;
            if (MobileInputManager.Instance.SwipeUP)
            {
                _playerAnimator.SetTrigger("Jump");
                _verticalVelocity =  _jumpFoce;
                isGrounded = false;
            }

            if (MobileInputManager.Instance.SwipeDown)
            {
                StartSliding();
                Invoke("StopSliding", 1f);
            }


        }
        else
        {
            _verticalVelocity -= (_gravity * Time.deltaTime);

            // fast fall mechanic
            if(MobileInputManager.Instance.SwipeDown)
            {
                _verticalVelocity = -_jumpFoce;
            }
        }
        moveVector.y = _verticalVelocity;
        moveVector.z = _speed;
        
        // Controlling the movement 
        _characterController.Move(moveVector * Time.deltaTime);

        //Rotating the player to where he is going
        Vector3 dir = _characterController.velocity;
        if(dir == Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, _turnSpeed);
        }
    }

    //Calculate the right lane to where we should move
    private void CalculateLaneToMove(bool goingRight)
    {
        int laneToMove = (int)_desiredLane;
        laneToMove += (goingRight) ? 1 : -1;
        laneToMove = Mathf.Clamp(laneToMove, (int)CoursePosition.Left, (int)CoursePosition.Right);
        _desiredLane = (CoursePosition)laneToMove;
        
    }

    //check if the player is in the ground or jumping
   private bool IsGrounded()
    {
        Ray groundRay = new Ray(
            new Vector3(
                _characterController.bounds.center.x, 
                _characterController.bounds.center.y - _characterController.bounds.extents.y + 0.2f,
                _characterController.bounds.center.z
                ),
            Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    } 

    private void StartSliding ()
    {
        _playerAnimator.SetBool("Sliding", true);
        _characterController.height /= 2;
        _characterController.center = new Vector3(
            _characterController.center.x,
            _characterController.center.y /2,
            _characterController.center.z
            );
    }
    private void StopSliding ()
    {
        _playerAnimator.SetBool("Sliding", false);
        _characterController.height *= 2;
        _characterController.center = new Vector3(
            _characterController.center.x,
            _characterController.center.y * 2, 
            _characterController.center.z
            );
    }

    private void Crash()
    {
        _playerAnimator.SetTrigger("Death");
        _isRunning = false;
        GameManager.Instance.OnDeath();
    }
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch(hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break; 
        }
    }

    private IEnumerator ImproveSpeed()
    {
        WaitForSeconds timeToWait = new WaitForSeconds(_speedIncreaseTime);
        while (_isRunning)
        {
            yield return timeToWait;
            _speed += _speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(_speed - _originalSpeed);
        }
    }
}
