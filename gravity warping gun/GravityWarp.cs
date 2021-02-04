using UnityEngine;

public class GravityWarp : MonoBehaviour
{
	//static readonly Vector3 defaultGravity = Physics.gravity;

	[SerializeField] float gravityMin = 1f;
	[SerializeField] float gravityMax = 25f;
	[SerializeField] float gravityDilusion = 4f;
	[SerializeField] float warpTime = 5f;
	Rigidbody body;
	Vector3 spawnPosition;
	AudioSource audioSource = null;
	float warpTimeDelta = 0f;
	bool warped = false;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		spawnPosition = transform.position;
	}

	private void Update()
	{
		// Because I'm too lazy to spawn another
		if (transform.position.y < -10f)
			transform.position = spawnPosition;

		if (!warped) return;

		if (warpTimeDelta >= warpTime)
		{
			Warp(false);
			return;
		}

		warpTimeDelta += Time.deltaTime;
	}

	public void Warp(bool b)
	{
		warped = b;
		body.useGravity = !b;
		warpTimeDelta = 0f;

		if (!b)
		{
			body.constraints = RigidbodyConstraints.FreezeRotation;
			body.rotation = Quaternion.identity;
			GameManager.main.respawnSound.pitch = Random.Range(0.5f, 1.5f);
			GameManager.main.respawnSound.Play();
			return;
		}

		var warpForce = new Vector3
		{
			x = Random.Range(-gravityMax, gravityMax),
			y = Random.Range(gravityMin, gravityMax) / gravityDilusion,
			z = Random.Range(-gravityMax, gravityMax),
		};

		body.AddForce(warpForce, ForceMode.Impulse);
		body.constraints = RigidbodyConstraints.None;
		body.angularVelocity = warpForce;
		audioSource.pitch = Random.Range(0.5f, 1.5f);
		audioSource.Play();
	}
}
