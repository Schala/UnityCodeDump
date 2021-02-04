using UnityEngine;

namespace StudioJamJan2021
{
    public class MouseLook : MonoBehaviour
    {
		[SerializeField] float sensitivity = 100f;
		[SerializeField] Transform body = null;
		float xRotation = 0f;

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
			var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

			xRotation -= mouseY;
			xRotation = Mathf.Clamp(xRotation, -90f, 90f);

			transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
			body.Rotate(Vector3.up * mouseX);
		}
	}
}
