using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portraits
{
	public class HudView : MonoBehaviour
	{
		/// <summary>
		/// Cached components 
		/// </summary>
		private Transform _hTransform;
		private Transform _parentTransform;
		private Text _timer;
		private Text _coolDownTimer;
		private Text _scoreTx; 

		private PortraitsController _pController;
		private SoundManager _soundManager; 
		private PortraitsController.GPState _currentState; //Current state of the gamePlay

		/// <summary>
		/// Data to display timers
		/// </summary>
		private float _cooldownTs;
		private float _playingTs;
		private bool _timeRunning = false;
		private float _timeLeft;

		public bool Enabled
		{
			set
			{
				_hTransform.gameObject.SetActive(value);
			}
		}

		#region MonoBehaviour methods
		void Awake()
		{
			_hTransform = this.transform;
			_parentTransform = _hTransform.Find("Hub");
			_timer = _parentTransform.Find("Timer").GetComponent<Text>();
			_coolDownTimer = _parentTransform.Find("CoolDownTimer").GetComponent<Text>();
			_scoreTx = _parentTransform.Find("Score").GetComponent<Text>();

			//Suscribe to events of the Controller
			SuscriptionsActive(true);
		}

		void Start()
		{
			//Access to Level states
			_pController = PortraitsController.Instance;
			_currentState = _pController.CurrentState;
			_cooldownTs = (float)(_pController.PLevel.ActiveWaitingTimer);
			_playingTs = (float)(_pController.PLevel.ActiveTimer);
			_soundManager = SoundManager.instance;

			_scoreTx.text = _pController.CurrentScore + "/" + _pController.MaxScore;
		}

		void Update()
		{
			if (!_timeRunning)
				return;

			_timeLeft -= Time.deltaTime;

			Text tUpdate = _currentState == PortraitsController.GPState.CoolDown ? _coolDownTimer : _timer;
			UpdateTimerDisplay(tUpdate,_timeLeft);

			if (_timeLeft <= 0)
			{
				_timeRunning = false;
				//Notify
				_pController.Notify(GameNotifications.PL_TIME_OUT, this);
			}
		}
		void Destroy()
		{
			//Unsuscribe events 
			SuscriptionsActive(false);
		}

		void OnEnable()
		{
			SuscriptionsActive(true);
		}

		void OnDisable()
		{
			SuscriptionsActive(false);
		}

		#endregion
		private void SuscriptionsActive(bool suscribe)
		{
			if (suscribe)
			{
				PortraitsController.OnStateChange += HandleGamePlayStateChange;
				PortraitsController.OnMatch += HandleUpdateScore;
			}
			else
			{
				PortraitsController.OnStateChange -= HandleGamePlayStateChange;
				PortraitsController.OnMatch -= HandleUpdateScore;
			}
		}
		#region Listen events handlers
		/// <summary>
		/// This method attends the changes during game play to decide which elements of the UI to show
		/// </summary>
		private void HandleGamePlayStateChange()
		{
			_currentState = _pController.CurrentState;
			switch(_currentState)
			{
				case PortraitsController.GPState.CoolDown:
					Visible(true);
					_coolDownTimer.gameObject.SetActive(true);
					_timer.gameObject.SetActive(false);
					_scoreTx.gameObject.SetActive(false);
					_timeRunning = true;
					_timeLeft = _cooldownTs;
					break;
				case PortraitsController.GPState.Playing:
					Visible(true);
					_coolDownTimer.gameObject.SetActive(false);
					_timer.gameObject.SetActive(true);
					_scoreTx.gameObject.SetActive(true);
					_timeRunning = true;
					_timeLeft = _playingTs;
					break;
                case PortraitsController.GPState.OutOfTime:
                case PortraitsController.GPState.Success:
					StartCoroutine(DoVanish());
					_timeRunning = false;
					break;
			}
		}
		private void Visible(bool value)
		{
			_parentTransform.gameObject.SetActive(value);
			_scoreTx.text = _pController.CurrentScore + "/" + _pController.MaxScore;
		}
		public IEnumerator DoVanish()
		{
			yield return new WaitForSeconds(0.5f);
			Visible(false);
		}

		private void HandleUpdateScore(CustomPair firstS, CustomPair secondS)
		{
			_scoreTx.text = _pController.CurrentScore + "/" + _pController.MaxScore;
		}

		#endregion
		private void UpdateTimerDisplay(Text tUpdate,float secs)
		{
			tUpdate.GetComponent<Text>().text = TimeCalculation((int)secs);
			//SoundManager.instance.PlaySFXById(SoundManager.SFX_COUNT_TICK);
		}

		#region Utilities - Move to generic place 
		public static string TimeCalculation(int tSec)
		{
			int h = (tSec / 3600);
			int m = ((tSec - h * 3600) / 60);
			int s = tSec - (h * 3600 + m * 60);
			//return /*h.ToString() + ":" +*/ m.ToString() + ":" + s.ToString();

			return string.Format("{0}:{1:00}", m, s);
		}
		#endregion
	}
}

