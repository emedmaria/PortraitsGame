using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portraits
{
	public struct CustomPair 
	{
		public int Key;
		public Vector2 Value;

		public CustomPair(int key, Vector2 value)
		{
			this.Key = key;
			this.Value = value;
		}
	}

	public class PortraitsController : MonoBehaviour
	{
		#region Events for suscriptors 
		//Instead of having a reference of the View, the view is subscribed to Controller events. 

		//To listen when the suitable Level should be displayed
		public delegate void TriggerLevelToLoad(PortraitsLevel level);
		public static event TriggerLevelToLoad OnSetUpLevel;

		//Fire changes in the game play
		public delegate void StateChange();
		public static event StateChange OnStateChange;

		//To listen when a selection happens
		public delegate void PairSelected(CustomPair firstS, CustomPair secondS, bool isMatch);
		public static event PairSelected OnPairSelected;

		//To listen when a match happens
		public delegate void Match(CustomPair firstS, CustomPair secondS);
		public static event Match OnMatch;

		//To listen when Success
		public delegate void Success(bool lastLevel = false);
		public static event Success OnSuccess;

		//To listen when Failed
		public delegate void Failed();
		public static event Failed OnFailed;

		#endregion

		//Possible States
		public enum GPState
		{
			CoolDown,	//Cooldown 
			Playing,	//InGame
			OutOfTime,  //Game ended 
			Success     //All matches done
		}

		//Current GM State
		private  GPState _currentState = GPState.CoolDown;
		public GPState CurrentState
		{
			get { return _currentState; }
		}
		private int _currentCountDown;

		//Current Level - Will be giveen by the GameManager who tracks the progess and the current
		/*[SerializeField]
		[Tooltip("Reference to the current level to play")]*/
		private PortraitsLevel _pLevel;
		public PortraitsLevel PLevel
		{
			get { return _pLevel; }
		}

		//Player selections -
		private CustomPair _selection1 = new CustomPair(-1, new Vector2(-1,-1));
		private CustomPair _selection2 = new CustomPair(-1, new Vector2(-1, -1));

		//Needed just in case CountUp Mode
		private int _timeLeft;
		public int TimeLeft
		{
			get { return _timeLeft; }
		}

		//Current Level matches
		private int _currentScore;
		public int CurrentScore
		{
			get { return _currentScore; }
		}
		private int _maxScore;
		public int MaxScore
		{
			get { return _maxScore; }
		}

		private LvlProgress.PlayMode _currentGameMode;
		public LvlProgress.PlayMode CurrentGameMode
		{
			get { return _currentGameMode; }
		}

		#region Singleton
		private static PortraitsController _instance;
		public static PortraitsController Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PortraitsController();
				}
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}
        #endregion

        #region MonoBehaviour methods
        void Awake()
		{
			_instance = this;
		}

		void OnDestroy()
		{
			_instance = null;
		}

		#endregion

		#region Initializations GamePlay 

		public void SetUpLevel(PortraitsLevel level)
		{
			//Update current level
			_pLevel = level;
			_currentScore = 0;
			_maxScore = _pLevel.PortraitIDs.Count/2;
			_currentGameMode = GameManager.instance.CurrentGameMode;

			//Suscribers will do its job 
			if (OnSetUpLevel != null)
				OnSetUpLevel(level);
		}

		public void UpdateGamePlay()
		{
			switch (_currentState)
			{
				case GPState.CoolDown:
					if (OnStateChange != null)
						OnStateChange();
					break;
				case GPState.Playing:
					SoundManager.instance.PlaySingleByID(SoundManager.AmbientMusic.InGame);
					if (OnStateChange != null)
						OnStateChange();
					break;
				case GPState.Success:
					SoundManager.instance.StopSingleByID(SoundManager.AmbientMusic.InGame);
					//TODO: Fade Out Camera
					if (OnStateChange != null)
						OnStateChange();
						if (OnSuccess != null)
							OnSuccess(GameManager.instance.IsLastLevel) ;
						break;
                case GPState.OutOfTime:
					SoundManager.instance.StopSingleByID(SoundManager.AmbientMusic.InGame);
					if (OnFailed != null)
						OnFailed();
					if (OnStateChange != null)
						OnStateChange();
					//TODO: Fade Out Camera
					break;
			}
		}

		#region Get Notified
		//public void Notify(string notifID, Object target, params object[] data)
		public void Notify(string notifID, Object target,MyCustomEventArgs eArgs = null)
		{
			switch (notifID)
			{
				case GameNotifications.PL_PAIR_SELECTED:
					UpdateSelection(eArgs.id,eArgs.coordinates);
					break;
				case GameNotifications.PL_TIME_OUT:
					if(_currentState == GPState.CoolDown)
						_currentState = GPState.Playing;
					else if(_currentState == GPState.Playing)
						_currentState = GPState.OutOfTime;
					UpdateGamePlay(); 
					break;
				case GameNotifications.PL_RESTART:
					_currentState = GPState.CoolDown;
					SetUpLevel(_pLevel);
					UpdateGamePlay();
					break;
				case GameNotifications.PL_CONTINUE:
					_currentState = GPState.CoolDown; //Get ready to start new level once GM proceeds
					break;
			}
		}
		#endregion
		/// <summary>
		/// Logic to check whether there is a match
		/// </summary>
		private void UpdateSelection(int id,Vector2 coordinates)
		{
			//Is the first selection
			if(_selection1.Key == -1)
				_selection1 = new CustomPair(id, coordinates);
			else
				_selection2 = new CustomPair(id, coordinates);

			//Check status of the level
			bool isMatch = CheckMatch();
			if (isMatch)
			{
				SoundManager.instance.PlaySFXById(SoundManager.SFX_GOOD_MATCH);
				//SoundManager.instance.PlaySingleByID(SoundManager.AmbientMusic.InGame);
				//Raise event
				if (OnPairSelected != null)
					OnPairSelected(_selection1,_selection2,isMatch);

				//Update Score
				_currentScore += 1;

				if (OnMatch != null)
					OnMatch(_selection1, _selection2);

				if (_currentScore == _maxScore)
				{
					_currentState = GPState.Success;
					UpdateGamePlay();
				}

				//Reset selections
				_selection1 = _selection2 = new CustomPair(-1, new Vector2(-1, -1));
			}
			else
			{
				//Check whether was second selection
				if (_selection2.Key == -1) return;

				//Raise event 
				if (OnPairSelected != null)
					OnPairSelected(_selection1, _selection2, isMatch);

				//Reset selections
				_selection1 = _selection2 = new CustomPair(-1, new Vector2(-1, -1));
			}
		}

		private bool CheckMatch()
		{
			return _selection1.Key == _selection2.Key;
		}

		#endregion
	}
}

