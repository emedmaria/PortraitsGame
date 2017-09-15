using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Portraits
{
	/// <summary>
	/// Controller of the global state of the Game
	/// </summary>
	public class GameManager : MonoBehaviour {

		/// <summary>
		/// Define the global states of the GameApp
		/// </summary>
		enum GlobalState
		{
			Menu,
			InGame,
			Exit,
			Continue
			//Resume
		}
		private GlobalState _currentGState; 

		/// <summary>
		/// Reference to the controllers of subSystems
		/// </summary>
		private PortraitsController _portraitsController;   //Controller of the level
		private PortraitsLevel _currentLevelData;           //CurrentLevelData
		private int _currentLevel;                          //CurrentLevel index
		public int CurrentLevel
		{
			get { return _currentLevel; }
		}
		public bool IsLastLevel
		{
			get { return _currentLevel == _gameModel.MaxLevel; }
		}
		private LvlProgress.PlayMode _currentGameMode;   
		public LvlProgress.PlayMode CurrentGameMode
		{
			get;
			private set;
		}                    

		//Player and game Models
		[SerializeField]
		private GameModel _gameModel;                        //Global data for the Game(App) - ScriptableObject drag to inspector
		[SerializeField]
		private PlayerModel _playerModel;					//Player data  - ScriptableObject drag to the inspector

        public static GameManager instance = null;          //Static instance of GameManager to access from any other script -Singleton

        #region Events to suscribe 
        //To listen when Game is Initialized
        public delegate void GameInitialized();
		public static event GameInitialized OnGameInitialized;

        //To listen when the Global state of the game has changed
        public delegate void GlobalStateUpdated();
		public static event GlobalStateUpdated OnStateUpdated;
		#endregion


		#region MonoBehaviour methods
		//Awake is always called before any Start functions
		void Awake()
		{
			if (instance == null)
				instance = this;
			else if (instance != this)
				Destroy(gameObject); //Enforces our singleton, only one instance

            //Not to destroy on reload Scene
            DontDestroyOnLoad(gameObject);

			//Set default Game global state
			_currentGState = GlobalState.Menu;
		}

		void Start()
		{
			InitGame();
		}
		#endregion

		#region Global Game
		void InitGame()
		{
			//Cache or assign components
			_portraitsController = transform.GetComponent<PortraitsController>();

			//Data
			_currentLevel = _playerModel.Level-1;
			_currentLevelData = _gameModel.GetLevel(_currentLevel);
			_currentGameMode = _playerModel.ActivePlayMode;

			if (OnGameInitialized != null)
				OnGameInitialized();
			
			//Se puede hacer el set up del level sin iniciarlo
			_portraitsController.SetUpLevel(_currentLevelData);
		}
		#endregion

		#region Notifications
		public void Notify(string notifID, Object p_target, params object[] param)
		{	
			switch (notifID)
			{
				case GameNotifications.GMN_START:
					_currentGState = GlobalState.InGame;
					_portraitsController.UpdateGamePlay();
					break;
				case GameNotifications.GMN_CONTINUE:
					SaveProgress();
					_currentGState = GlobalState.InGame;
					_portraitsController.UpdateGamePlay();
					break;
				case GameNotifications.GMN_EXIT:
					_currentGState = GlobalState.Exit;
					#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
					#else
					Application.Quit();
					#endif
					break; 
			}
		}
		/// <summary>
		/// Saves score of current level and updates to next level
		/// </summary>
		private void SaveProgress()
		{
			//Update Player Data
			_playerModel.SaveProgress();
			//Prepare Next level
			_currentLevel = (!IsLastLevel) ? (_currentLevel + 1) : _currentLevel;
			_currentLevelData = _gameModel.GetLevel(_currentLevel);
			_portraitsController.SetUpLevel(_currentLevelData);
		}
		#endregion

		//this is called only once, and the paramter tell it to be called only after the scene was loaded
		//(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static public void CallbackInitialization()
		{
			//register the callback to be called everytime the scene is loaded
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		//This is called each time a scene is loaded.
		static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			//instance.level++;
			//instance.InitGame();
		}
	}
}
