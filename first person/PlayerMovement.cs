using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float speed = 12f;
	[SerializeField] float gravity = -9.81f;
	[SerializeField] Transform groundCheck = null;
	[SerializeField] float groundDistance = 0.4f;
	[SerializeField] LayerMask groundMask;
	[SerializeField] float jumpHeight = 3f;

	CharacterController controller = null;
	Vector3 velocity = Vector3.zero;
	bool grounded = true;

	private void Awake()
	{
		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		if (grounded && velocity.y < 0f)
			velocity.y = -2f; // forces the player on the ground better than a value of 0

		var x = Input.GetAxis("Horizontal");
		var z = Input.GetAxis("Vertical");
		var move = transform.right * x + transform.forward * z;

		controller.Move(move * speed * Time.deltaTime); // move from input

		if (Input.GetButtonDown("Jump") && grounded)
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime); // factor in the velocity

	}
}
