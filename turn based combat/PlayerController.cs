using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] float thresholdMultiplier = 20f;
    [SerializeField] float moveSpeedMultiplier = 2f;
    CharacterController controller;
    Vector3 input = Vector3.zero;
    int health;
    float thresholdDelta = 0f;
    float threshold = 0f;
    bool moving = false;

    private void Awake() => controller = GetComponent<CharacterController>();

	void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (moving)
        {
            controller.Move(input * moveSpeedMultiplier * Time.deltaTime);
            thresholdDelta += Time.deltaTime;
        }

        if (thresholdDelta >= threshold)
            BattleSystem.Instance.NewBattle();
    }

	private void OnEnable()
	{
        thresholdDelta = 0f;
        threshold = Random.value * thresholdMultiplier;
    }

	public void Move(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();

        input = new Vector3(value.x, 0f, value.y);

        var turnAngle = Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, turnAngle, 0f);

        if (context.performed) moving = true;
        if (context.canceled) moving = false;
    }
}
