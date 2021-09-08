using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour {
	public CharacterController controller;
	public Transform cam;
	
	public float playerVel = 6f;
	public float turnSmoothingTime = 0.1f;
	private float _turnSmoothingVel;
	
	void Update() {
		// TODO: migrate to new input system
		var horizInput = Input.GetAxisRaw("Horizontal");
		var vertInput = Input.GetAxisRaw("Vertical");
		var inputDirection = new Vector3(horizInput, 0f, vertInput).normalized;

		if (!(inputDirection.magnitude >= 0.1f)) return;
		
		var targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
		var smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothingVel, turnSmoothingTime);
		transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

		var playerDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
		controller.Move(playerVel * Time.deltaTime * playerDirection);
	}
}