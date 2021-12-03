using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Appena mi stacco da terra non aggiorno più _moveDirections e gli ultimi valori li uso per il salto
// Quando sono in aria non aggiorno _moveDirections
// Mettere il Move con _moveDirections fuori da if(_isGrounded) permette di saltare verso una direzione
// Se lo metti dentro salti solo in verticale

public class Player : MonoBehaviour
{
    //VARIABLES
    [SerializeField] private int _moveSpeed = 0;
    [SerializeField] private int _walkSpeed = 3;
    [SerializeField] private int _runSpeed = 10;
    // Gravity
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpHeight = 1.5f;

    private Vector3 _verticalPosition;
    private Vector3 _moveDirections;

    //REFERENCES
    private CharacterController _controller;
    private Animator _anim;


    void Start(){
        _controller = GetComponent<CharacterController>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Update(){
        MoveMe();

        if(Input.GetKeyDown(KeyCode.Mouse0)){
            // Attack();
            StartCoroutine(Attack());
        }
    }

    
    void MoveMe(){
        _isGrounded = Physics.CheckSphere(transform.position, _groundCheckDistance, _groundMask);

        if(_isGrounded && _verticalPosition.y < 0){
            _verticalPosition.y = -2f;
        }

        if(_isGrounded){ 
            if(Input.GetKey(KeyCode.Space)){
                Jump();
            }
            float delta = Time.deltaTime;
            float moveX = Input.GetAxis("Horizontal"); // DESTRA E SINISTRA
            float moveZ = Input.GetAxis("Vertical"); // AVANTI E INDIETRO
            _moveDirections = new Vector3(moveX, 0, moveZ);
            _moveDirections = transform.TransformDirection(_moveDirections);
            
            if(_moveDirections != Vector3.zero && !Input.GetKey(KeyCode.LeftShift)){
                Walk();
            } else if(_moveDirections != Vector3.zero && Input.GetKey(KeyCode.LeftShift)) {
                Run();
            } else if (_moveDirections == Vector3.zero){
                Idle();
            }
            _moveDirections *= _moveSpeed;
        }

        _controller.Move(_moveDirections * Time.deltaTime);

        //Gravity 
        _verticalPosition.y += _gravity * Time.deltaTime;
        _controller.Move(_verticalPosition * Time.deltaTime);
    }

    void Jump(){
        // _verticalPosition.y = 6f;
        _verticalPosition.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
    }

    void Idle(){
        _moveSpeed = 0;
        _anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }

    void Walk(){
        _moveSpeed = _walkSpeed;
        _anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }

    void Run(){
        _moveSpeed = _runSpeed;
        _anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
    }

    IEnumerator Attack(){
        _anim.SetLayerWeight(_anim.GetLayerIndex("Attack"), 1);
      _anim.SetTrigger("Attack");

      yield return new WaitForSeconds(0.9f);
      _anim.SetLayerWeight(_anim.GetLayerIndex("Attack"), 0);
    }
}


