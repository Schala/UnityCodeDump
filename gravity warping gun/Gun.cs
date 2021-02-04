using UnityEngine;

public class Gun : MonoBehaviour
{
	[SerializeField] LayerMask targetMask;
	[SerializeField] Transform spawnPoint = null;
	[SerializeField] Transform endPoint = null;
	[SerializeField] LineRenderer rayVisual = null;
	Camera pov = null;

	private void Awake()
	{
		rayVisual.positionCount = 2;
		rayVisual.enabled = false;
		pov = Camera.main;
	}

	private void OnMouseDown() => Shoot();

	private void OnMouseUp() => rayVisual.enabled = false;

	void Shoot()
	{
		var targetFound = Physics.Raycast(spawnPoint.position, pov.transform.TransformDirection(Vector3.forward), out RaycastHit hit);
		rayVisual.enabled = true;
		rayVisual.SetPosition(0, spawnPoint.position);
		if (!targetFound)
		{
			rayVisual.SetPosition(1, endPoint.position);
			return;
		}

		rayVisual.SetPosition(1, hit.point);

		var gw = hit.collider.GetComponent<GravityWarp>();
		if (gw == null) return;
		gw.Warp(true);
	}

	private void OnMouseDrag() => Shoot();
}
