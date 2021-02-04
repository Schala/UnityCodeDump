using Cinemachine;
using UnityEngine;

public enum BattleSlot : byte
{
	Center = 0,
	Left,
	Right,
	BackCenter,
	BackLeft,
	BackRight
}

public enum BattleInitiative : byte
{
	Normal = 0,
	PlayerAmbushed,
	EnemyAmbushed
}

public class BattleSystem : MonoBehaviour
{
	public static BattleSystem Instance { get; private set; } = null;
	public static bool Active { get; private set; } = false;

	[SerializeField] GameObject[] playerPrefabs = null;
	[SerializeField] Transform playerStartMedian = null;
	[SerializeField] Transform enemyStartMedian = null;
	[SerializeField] GameObject controlledPlayer = null;
	[SerializeField] int maxPlayers = 3;
	[SerializeField] int maxEnemies = 6;
	[SerializeField] float spacing = 25f;
	[SerializeField] float cameraHeight = 15f;
	[SerializeField] float cameraDistance = 30f;
	GameObject[] playerInstances = null;
	GameObject[] enemyInstances = null;
	BattleUnit[] units = null;
	Camera pov = null;
	CinemachineBrain povBrain = null;

	int enemiesThisBattle = 0;
	int unitsThisBattle = 0;
	int roundCounter = 0;
	int currentTurn = 0; // todo: determine order via speed + action. Item usage is faster than attacks
	float cameraDelta = 0f;
	BattleInitiative initiative = BattleInitiative.Normal;

	private void LateUpdate()
	{
		if (!Active) return;

		pov.transform.RotateAround(transform.position, Vector3.up, cameraDelta);
	}

	private void Update()
	{
		if (!Active) return;

		NextTurn();
	}

	public BattleUnit GetTarget(BattleUnit requester)
	{
		var tags = requester.GetComponent<TagCollection>();
		if (tags.tags.Contains("Player"))
			return enemyInstances[Random.Range(0, enemiesThisBattle)].GetComponent<BattleUnit>();
		else
			return playerInstances[Random.Range(0, playerPrefabs.Length)].GetComponent<BattleUnit>();
	}

	void NextTurn()
	{
		if (!units[currentTurn].ActedThisTurn) return;

		currentTurn++;
		if (currentTurn >= unitsThisBattle)
			NextRound();

		print($"{units[currentTurn].Name}'s turn!");
		StartCoroutine(units[currentTurn].Attack(GetTarget(units[currentTurn])));
	}

	void NextRound()
	{
		for (int i = 0; i < unitsThisBattle; i++)
			units[i].ActedThisTurn = false;
		currentTurn = 0;
	}

	private void Awake()
	{
		if (Instance != null)
			Destroy(gameObject);
		Instance = this;

		playerInstances = new GameObject[maxPlayers];
		enemyInstances = new GameObject[maxEnemies];
		units = new BattleUnit[maxPlayers + maxEnemies];

		cameraDelta = Time.deltaTime;
		pov = Camera.main;
		povBrain = pov.GetComponent<CinemachineBrain>();
	}

	public void NewBattle(params GameObject[] enemyPrefabs)
	{
		povBrain.enabled = false; // Cinemachine makes automated orbital rotation a headache
		pov.transform.position = transform.position + (Vector3.forward * cameraDistance) + (Vector3.up * cameraHeight);
		pov.transform.LookAt(transform.position, Vector3.up);

		Active = true;
		controlledPlayer.SetActive(false);

		enemiesThisBattle = Random.Range(1, maxEnemies + 1);
		unitsThisBattle = playerPrefabs.Length + enemiesThisBattle;

		for (int i = 0; i < enemiesThisBattle; i++)
		{
			var enemy = SpawnEnemy((BattleSlot)i);
			enemyInstances[i] = enemy;
			var battler = enemy.GetComponent<BattleUnit>();
			battler.Name = $"COVID Pill {(char)(i + 65)}";
			units[playerPrefabs.Length + i] = battler;
		}

		for (int i = 0; i < playerPrefabs.Length; i++)
		{
			var tags = playerPrefabs[i].GetComponent<TagCollection>();
			var player = SpawnPlayer((BattleSlot)i, tags.tags[0]);
			playerInstances[i] = player;
			units[i] = player.GetComponent<BattleUnit>();
		}

		StartCoroutine(units[currentTurn].Attack(GetTarget(units[currentTurn])));
	}

	public void EndBattle()
	{
		for (int i = 0; i < enemiesThisBattle; i++)
		{
			if (enemyInstances[i] != null)
			{
				if (enemyInstances[i].activeInHierarchy)
					enemyInstances[i].SetActive(false);
				enemyInstances[i] = null;
			}
		}

		for (int i = 0; i < playerPrefabs.Length; i++)
		{
			if (playerInstances[i] != null)
			{
				if (playerInstances[i].activeInHierarchy)
					playerInstances[i].SetActive(false);
				playerInstances[i] = null;
			}
		}

		controlledPlayer.SetActive(true);
		Active = false;
		povBrain.enabled = true;
	}

	GameObject SpawnPlayer(BattleSlot slot, string tag = "")
	{
		var position = playerStartMedian.position;

		switch (slot)
		{
			case BattleSlot.Left: position.x += spacing; break;
			case BattleSlot.Right: position.x -= spacing; break;
			case BattleSlot.BackCenter: position.z -= spacing; break;
			case BattleSlot.BackLeft:
				position.x += spacing;
				position.z -= spacing;
				break;
			case BattleSlot.BackRight:
				position.x -= spacing;
				position.z -= spacing;
				break;
			default: break;
		}

		GameObject player;
		if (tag == "")
			player = ObjectPool.Instance.Get("Player");
		else
			player = ObjectPool.Instance.Get(tag);

		player.transform.position = position;
		player.transform.forward = initiative == BattleInitiative.PlayerAmbushed ? -playerStartMedian.forward : playerStartMedian.forward;
		player.SetActive(true);

		return player;
	}

	GameObject SpawnEnemy(BattleSlot slot, string tag = "")
	{
		var position = enemyStartMedian.position;

		switch (slot)
		{
			case BattleSlot.Left: position.x += spacing; break;
			case BattleSlot.Right: position.x -= spacing; break;
			case BattleSlot.BackCenter: position.z -= spacing; break;
			case BattleSlot.BackLeft:
				position.x += spacing;
				position.z -= spacing;
				break;
			case BattleSlot.BackRight:
				position.x -= spacing;
				position.z -= spacing;
				break;
			default: break;
		}

		GameObject enemy;
		if (tag == "")
			enemy = ObjectPool.Instance.Get("Enemy");
		else
			enemy = ObjectPool.Instance.Get(tag);

		enemy.transform.position = position;
		enemy.transform.forward = initiative == BattleInitiative.EnemyAmbushed ? -enemyStartMedian.forward : enemyStartMedian.forward;
		enemy.SetActive(true);

		return enemy;
	}
}
