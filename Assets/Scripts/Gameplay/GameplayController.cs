using UnityEngine;
using UnityEngine.SceneManagement;

// attach to a gameobject in a level
// controls the flow of the gameplay

[RequireComponent(typeof(BoxCollider))]
public class GameplayController : MonoBehaviour
{
	public enum GameState
	{
		Tutorial,
		Setup,
		Play,
		Win,
		Lose
	}

	public GameState CurrentState { get; private set; }

	// Components to enable/disable based on game state

	private DungeonSetupPhase setupPhase;
	[HideInInspector] public DungeonPlayPhase playPhase;
	private DungeonWinPhase winPhase;
	private DungeonLosePhase losePhase;
	[HideInInspector] public DungeonPrefabSelect prefabSelect;
	private GameObject goal;

	[SerializeField] private float timeToCount;
	[SerializeField] private int heroesToSpawn = 10; // Number of heroes to spawn
	[SerializeField] private float spawnInterval = 1f; // Base spawn interval
	[SerializeField] private float spawnIntervalVariation = 1f; // Variation in spawn interval
	[SerializeField] private int startingEnergy = 500;
	[SerializeField] private int energyRegenPerSecond = 10;
	public BoxCollider dungeonBounds;

	private void Awake()
	{
		// find the required gameobjects for each state
		setupPhase = GameObject.Find("DungeonSetupPhase").GetComponent<DungeonSetupPhase>();
		playPhase = GameObject.Find("DungeonPlayPhase").GetComponent<DungeonPlayPhase>();
		winPhase = GameObject.Find("DungeonWinPhase").GetComponent<DungeonWinPhase>();
		losePhase = GameObject.Find("DungeonLosePhase").GetComponent<DungeonLosePhase>();
		prefabSelect = GameObject.Find("PrefabSelectController").GetComponent<DungeonPrefabSelect>();
		goal = GameObject.Find("Goal");

		dungeonBounds = GetComponent<BoxCollider>();

		// pass a reference of the GameplayController to each
		setupPhase.gameplayController = this;
		playPhase.gameplayController = this;

		setupPhase.timeToCount = this.timeToCount;
		playPhase.heroesToSpawn = this.heroesToSpawn;
		playPhase.spawnInterval = this.spawnInterval;
		playPhase.spawnIntervalVariation = this.spawnIntervalVariation;

		prefabSelect.startingEnergy = this.startingEnergy;
		prefabSelect.energyRegenPerSecond = this.energyRegenPerSecond;
	}

	void Start()
	{
		InitialState();
		// check win and lose conditions here, to be cleaner
		// DungeonHeroSpawner has the info we need to check for win con
		playPhase.OnWinConditionMet.AddListener(WinGame);
		goal.GetComponent<Health>().OnDied.AddListener(LoseGame);
	}
	// Called by the play/try again buttons in win and lose phase
	public void PlayAgain()
	{
		// Get the current scene index
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		// Reload the current scene
		SceneManager.LoadScene(currentSceneIndex);
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void StartGame()
	{
		// Transition to the play phase when the game starts
		SetGameState(GameState.Play);
	}

	public void WinGame()
	{
		// Transition to the win phase when the player wins
		SetGameState(GameState.Win);
	}

	public void LoseGame()
	{
		// Transition to the lose phase when the player loses
		SetGameState(GameState.Lose);
	}

	void InitialState()
	{
		// deactivate all gameobjects after getting references to them
		setupPhase.gameObject.SetActive(false);
		playPhase.gameObject.SetActive(false);
		winPhase.gameObject.SetActive(false);
		losePhase.gameObject.SetActive(false);
		// Start the game in the setup phase
		SetGameState(GameState.Setup);
	}

	void SetGameState(GameState newState)
	{
		// Deactivate components from the previous state
		DeactivateComponents(CurrentState);

		// Activate components for the new state
		ActivateComponents(newState);

		// Update the current state
		CurrentState = newState;
	}

	void ActivateComponents(GameState state)
	{
		switch (state)
		{
			case GameState.Setup:
				setupPhase.gameObject.SetActive(true);
				setupPhase.Begin();
				break;
			case GameState.Play:
				playPhase.gameObject.SetActive(true);
				playPhase.Begin();
				break;
			case GameState.Win:
				Helper.DisableGameObject(prefabSelect.gameObject);
				winPhase.gameObject.SetActive(true);
				winPhase.Begin();
				break;
			case GameState.Lose:
				Helper.DisableGameObject(prefabSelect.gameObject);
				losePhase.gameObject.SetActive(true);
				losePhase.Begin();
				break;
		}
	}

	void DeactivateComponents(GameState state)
	{
		switch (state)
		{
			case GameState.Setup:
				Helper.DisableGameObject(setupPhase.gameObject);
				break;
			case GameState.Play:
				Helper.DisableGameObject(playPhase.gameObject);
				break;
			case GameState.Win:
				Helper.DisableGameObject(winPhase.gameObject);
				break;
			case GameState.Lose:
				Helper.DisableGameObject(losePhase.gameObject);
				break;
		}
	}
}