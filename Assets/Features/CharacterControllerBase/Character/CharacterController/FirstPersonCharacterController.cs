using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonCharacterController : MonoBehaviour, ICharacterSignals
{
	public struct MoveInputData {
		public readonly Vector2 move;
		public readonly bool jump;

		public MoveInputData(Vector2 move, bool jump) {
			this.move = move;
			this.jump = jump;
		}
	}
	
	public float strideLength => _strideLength;
	private Subject<Vector3> _moved;
	public IObservable<Vector3> moved => _moved;

	private ReactiveProperty<bool> _isRunning;
	public ReactiveProperty<bool> isRunning => _isRunning;

	private Subject<Unit> _landed;
	public IObservable<Unit> landed => _landed;
	
	private Subject<Unit> _jumped;
	public IObservable<Unit> jumped => _jumped;
	
	private Subject<Unit> _stepped;
	public IObservable<Unit> stepped => _stepped;
	
	// References:
	
	private CharacterController _characterController;
	private Camera _camera;
	[Header("Input")]
	public CharacterInput characterInput;
	
	[Header("Properties")]
	public float walkSpeed = 5f;
	public float runSpeed = 10f;
	public float jumpSpeed = 20f;
	public float _strideLength = 2.5f;
	public float stickToGround = 5f;
	[Range(-90, 0)]
	public float minViewAngle = -60f; // How much can the user look down (in degrees)
	[Range(0, 90)]
	public float maxViewAngle = 60f; // How much can the user look up (in degrees)

	private float xRotation = 0;
	



	private void Awake() {
		_characterController = GetComponent<CharacterController>();
		_camera = GetComponentInChildren<Camera>();

		_isRunning = new ReactiveProperty<bool>(false);
		_moved = new Subject<Vector3>().AddTo(this);
		_jumped = new Subject<Unit>().AddTo(this);
		_landed = new Subject<Unit>().AddTo(this);
		_stepped = new Subject<Unit>().AddTo(this);
	}

	private void Start() {
		// This sticks the character to the ground immediately so our first frame counts as "grounded".
		// Otherwise we get a spurious landing at program start.
		_characterController.Move(-stickToGround * transform.up);


		// ... and latch it until update. (+ produce false values for all other ticks.)
		var jumpLatch = CustomObservables.Latch(this.UpdateAsObservable(), characterInput.jump, false);

		// Now zip jump and movement together so we can handle them at the same time.
		// Zip only works here because both Movement and jumpLatch will emit at the same
		// frequency: during Update.


		// Handle movement input (WASD-style) with run (Shift).
		characterInput.move
			.Zip(jumpLatch, (m, j) => new MoveInputData(m,  j))
			.Where(moveInputData => moveInputData.jump || moveInputData.move != Vector2.zero || _characterController.isGrounded == false)
			.Subscribe(i => {
				// Note: CharacterController is a stateful object. But as long as I only modify it from this
				// function, I can be reasonably sure things will work as expected.
				var wasGrounded = _characterController.isGrounded;
				
				// Vertical movements (jumping and gravity) are the player's y-axis.
				var verticalVelocity = 0f;
				if (i.jump && wasGrounded) {
					// We're on the ground and want to jump.
					verticalVelocity = jumpSpeed;
					_jumped.OnNext(Unit.Default);
					Debug.Log("Jump detected");
				} else if (!wasGrounded) {

					// We're in the air: apply gravity.
					// todo: how much gravity to add? *Time.deltaTime is too small, without multiplyer it is too high
					verticalVelocity = _characterController.velocity.y + (Physics.gravity.y * Time.deltaTime * 3.0f);
				} else {
					Debug.Log("other");
					// We're otherwise on the ground: push us down a little.
					// (Required for character.isGrounded to work.)
					verticalVelocity = -Mathf.Abs(stickToGround);
				}

				// Horizontal movements are the player's x- and z-axes.
				var currentSpeed = characterInput.run.Value ? runSpeed : walkSpeed;
				var horizontalVelocity = i.move * currentSpeed; //Calculate velocity (direction * speed).

				// Combine horizontal and vertical into player coordinate space.
				var characterVelocity = transform.TransformVector(new Vector3(
					horizontalVelocity.x, // input x (+/-) corresponds to strafe right/left (player x-axis)
					verticalVelocity,
					horizontalVelocity.y)); // input y (+/-) corresponds to forward/back (player z-axis)

				// Apply movement.
				var distance = characterVelocity * Time.deltaTime;

				_characterController.Move(distance);

				// Set ICharacterSignals output signals related to the movement:

				var tempIsRunning = false;
				
				if (wasGrounded && _characterController.isGrounded) {
					// Both started and ended this frame on the ground.
					
					_moved.OnNext(_characterController.velocity * Time.deltaTime);

					if (_characterController.velocity.magnitude > 0)
					{
						// The chaarcter is running if the input is active and
						// the character is actually moving on the ground
						tempIsRunning = characterInput.run.Value;
					}
				}
				if (!wasGrounded && _characterController.isGrounded) {
					// Didn't start on the ground, but ended up there.
					_landed.OnNext(Unit.Default);
				}

				_isRunning.Value = tempIsRunning;
			}).AddTo(this);

		// Track distance walked to emit step events.
		var stepDistance = 0f;
		moved.Subscribe(w => {
			stepDistance += w.magnitude;
			if (stepDistance > _strideLength)
				_stepped.OnNext(Unit.Default);
			stepDistance %= _strideLength;
		}).AddTo(this);

		// Handle mouse input (free mouse look).
		characterInput.look
			.Where(v => v != Vector2.zero) // We can ignore this if mouse look is zero.
			.Subscribe(inputLook => {
				// Translate 2D mouse input into euler angle rotations.

				// inputLook.x rotates the character around the vertical axis (with + being right)
				var horzLook = inputLook.x * Vector3.up  * Time.deltaTime;
				transform.localRotation *= Quaternion.Euler(horzLook);
				//transform.Rotate(horzLook);

				
				// inputLook.y rotates the camera around the horizontal axis (with + being up)
				var vertLook = inputLook.y * Vector3.left * Time.deltaTime;
				var newQ = _camera.transform.localRotation * Quaternion.Euler(vertLook);

				// We have to flip the signs and positions of min/max view angle here because the math
				// uses the contradictory interpretation of our angles (+/- is down/up).
				_camera.transform.localRotation = ClampRotationAroundXAxis(newQ, -maxViewAngle, -minViewAngle);
				/*
				xRotation -= inputLook.y * Time.deltaTime;
				xRotation = Mathf.Clamp(xRotation, -90f, 90f);
				view.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);*/
			}).AddTo(this);
	}
	

	// Ripped straight out of the Standard Assets MouseLook script. (This should really be a standard function...)
	private static Quaternion ClampRotationAroundXAxis(Quaternion q, float minAngle, float maxAngle) {
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

		angleX = Mathf.Clamp(angleX, minAngle, maxAngle);

		q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}
}





