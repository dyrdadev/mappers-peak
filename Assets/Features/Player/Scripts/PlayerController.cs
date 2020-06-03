using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//[RequireComponent(typeof(AttackAbility))]
//[RequireComponent(typeof(BlockAbility))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour {

    
    private Vector3 movementDirection = new Vector3(0,0,0);
    private NavMeshAgent agent;
    private Vector3 attentionDirection = new Vector3(0,0,1);


    [Header("References")]
    [SerializeField] public Animator animator;


    [Header("General Parameters")]

    [SerializeField] private float speed = 8.0f;

   
	void Awake ()
    {
        agent = GetComponent<NavMeshAgent>();
    }
	
	void Update ()
    {
        // Fetch control Input.
        movementDirection = new Vector3(
             Input.GetAxis("Horizontal"),
             0,
             Input.GetAxis("Vertical")
        );

        // Update velocity.
        agent.velocity = new Vector3(movementDirection.x, 0, movementDirection.z) * speed ;


        // Update animator with input specific
        animator.SetFloat("velocity", agent.velocity.magnitude);
        animator.SetBool("isMoving", agent.velocity.magnitude > 0.3f);
    }
}