using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//[RequireComponent(typeof(AttackAbility))]
//[RequireComponent(typeof(BlockAbility))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour {

    
    private Vector2 movementDirection = new Vector2(0,0);
    private NavMeshAgent agent;
    private TopDownInput controls;
    private Vector3 attentionDirection = new Vector3(0,0,1);


    [Header("References")]
    [SerializeField] public Animator animator;


    [Header("General Parameters")]

    [SerializeField] private float speed = 8.0f;

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
   
	void Awake ()
    {
        agent = GetComponent<NavMeshAgent>();
        controls = new TopDownInput();
    }
	
	void Update ()
    {
        
        // Fetch control Input.
        movementDirection = controls.Character.Move.ReadValue<Vector2>();

        // Update velocity.
        agent.velocity = new Vector3(movementDirection.x, 0, movementDirection.y) * speed ;


        // Update animator with input specific
        animator.SetFloat("velocity", agent.velocity.magnitude);
        animator.SetBool("isMoving", agent.velocity.magnitude > 0.3f);
    }
}