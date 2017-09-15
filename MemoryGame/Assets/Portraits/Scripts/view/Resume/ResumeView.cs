using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portraits
{
	public class ResumeView : MonoBehaviour
	{
		/// <summary>
		/// Constants
		/// </summary>
		private const string LEVEL_TX = "Level";		//NTH get text Dynamically
		private const string SUCCESS_TX = "LEVEL UP!!";		
		private const string FAILED_TX = "Try again"; 
		private const string FINISH_TX = "WIN!! CONGRATS!!!"; 
		private const string SLASH = "/"; 
		private const string SPACE = " ";
		private const string ANIM_TRIGGER_ID = "successTrigger";
		private const string ANIM_ID = "HouseEnd";

		private PortraitsController _pController;
		private GameManager _gManager;
		private bool _lastLvl = false; 

		/// <summary>
		/// Cached components
		/// </summary>
		private Text _headlineTx;
		private Text _resultTx;
		private Text _scoreTx;
		private Text _timeTx;
		private Button _retryBtn;
		private Button _continueBtn;
		private Button _quitBtn;
		private Transform _resumePanel;
		private GameObject _stillBg;
		private GameObject _animBg;
		/// <summary>
		/// Control final animation
		/// </summary>
		private Animator _animator;
		private bool _animationTriggered = false; 

		#region MonoBehaviour methods
		void Awake()
		{
			_stillBg = transform.Find("Background").gameObject;
			_animBg = transform.Find("HouseAnimated").gameObject;
			_animator = _animBg.GetComponent<Animator>();

			//Text 
			_resumePanel = transform.Find("ResumePanel");
			Transform holder = _resumePanel.Find("TextHolder");
			_headlineTx = holder.Find("Headline").GetComponent<Text>();
			_resultTx = holder.Find("Result").GetComponent<Text>();
			_scoreTx = holder.Find("Score").GetComponent<Text>();
			_timeTx = holder.Find("Time").GetComponent<Text>();

			//Buttons
			holder = transform.Find("ResumePanel").Find("ButtonsHolder");
			_retryBtn = holder.Find("RetryBtn").GetComponent<Button>();
			_continueBtn = holder.Find("ContinueBtn").GetComponent<Button>();
			_quitBtn = holder.Find("QuitBtn").GetComponent<Button>();

			//Suscribe to Controller events
			SuscriptionsActive(true);
		}

		void Start()
		{
			_gManager = GameManager.instance;
			_pController = PortraitsController.Instance;
		}

		void OnDestroy()
		{   
			//Unsuscribe to Controller events
			SuscriptionsActive(false);
		}

		void OnDisable()
		{
			SuscriptionsActive(false);
		}

		#endregion

		public bool Active
		{
			set
			{
				transform.gameObject.SetActive(value);
			}
		}

		private void SuscriptionsActive(bool suscribe)
		{
			if (suscribe)
			{
				PortraitsController.OnSuccess += HandleShowSuccess;
				PortraitsController.OnFailed += HandleShowFailed;
			}
			else
			{
				PortraitsController.OnSuccess -= HandleShowSuccess;
				PortraitsController.OnFailed -= HandleShowFailed;
			}
		}

		void Update()
		{
			if (!_animationTriggered) return;

			AnimatorStateInfo sInfo = _animator.GetCurrentAnimatorStateInfo(0);
			if (_animator.GetCurrentAnimatorStateInfo(0).IsName(ANIM_ID))
			{
				return;
			}
			else { 
				_animationTriggered = false;
				StartCoroutine(DoShowSuccessDialog());
			}
			//if (_animator.IsInTransition(0)) return;	
		}
		#region Event Handlers

		IEnumerator DoShowSuccessDialog()
		{
			yield return new WaitForSeconds(5f);
			ShowSuccessDialog(_lastLvl);
		}
		private void ShowSuccessDialog(bool lastLvl)
		{
			//_stillBg.SetActive(true)
			_resumePanel.gameObject.SetActive(true);
			_headlineTx.text = LEVEL_TX + SPACE + (_gManager.CurrentLevel + 1);
			_scoreTx.text = _pController.CurrentScore + SLASH + _pController.MaxScore;

			bool showTime = (GameManager.instance.CurrentGameMode != LvlProgress.PlayMode.CountDown);
			_timeTx.gameObject.SetActive(showTime);

			_resultTx.text = (!lastLvl) ? SUCCESS_TX : FINISH_TX;
			_continueBtn.gameObject.SetActive(!lastLvl);
			_retryBtn.gameObject.SetActive(true);
			_quitBtn.gameObject.SetActive(true);

			SoundManager.instance.PlaySFXById(SoundManager.SFX_LEVELUP);
		}

		private void ShowSuccess(bool lastLvl)
		{
			_lastLvl = lastLvl;
			_animationTriggered = true;
			SoundManager.instance.PlaySFXById(SoundManager.SFX_SUCCESS); //TODO: Sync sound better with the animation
			_resumePanel.gameObject.SetActive(false);
			_animBg.SetActive(true);
			_animator.SetTrigger(ANIM_TRIGGER_ID);
		}

		private void HandleShowSuccess(bool lastLvl)
		{
			StartCoroutine(DoShowSuccess(lastLvl));
		}

		public IEnumerator DoShowSuccess(bool lastLvl)
		{
			yield return new WaitForSeconds(0.2f);
			ShowSuccess(lastLvl);
		}

		private void ShowFailed()
		{
			_stillBg.SetActive(true);
			_animBg.SetActive(false);

			_resumePanel.gameObject.SetActive(true);
			_headlineTx.text = LEVEL_TX + SPACE + (_gManager.CurrentLevel + 1);
			_scoreTx.text = _pController.CurrentScore + SLASH + _pController.MaxScore;

			bool showTime = (_gManager.CurrentGameMode != LvlProgress.PlayMode.CountDown);
			_timeTx.gameObject.SetActive(showTime);

			_timeTx.text = HudView.TimeCalculation(_pController.TimeLeft);
			_resultTx.text = FAILED_TX;
			_continueBtn.gameObject.SetActive(false);
			_retryBtn.gameObject.SetActive(true);
			_quitBtn.gameObject.SetActive(true);

			SoundManager.instance.PlaySFXById(SoundManager.SFX_LEVEL_FAILED);
		}

		public IEnumerator DoShowFailed()
		{
			yield return new WaitForSeconds(1.2f);
			ShowFailed();
		}

		private void HandleShowFailed()
		{
			StartCoroutine(DoShowFailed());
		}

		#endregion

		#region Button Handlers
		public void HandleRetryDown()
		{
			//SoundManager.instance.PlaySFXById(SoundManager.SFX_SELECTION);
			_pController.Notify(GameNotifications.PL_RESTART, this);

			_resumePanel.gameObject.SetActive(false);
			_stillBg.gameObject.SetActive(false);
			_animBg.gameObject.SetActive(false);
		}

		public void HandleExitDown()
		{
			SoundManager.instance.PlaySFXById(SoundManager.SFX_SELECTION);
			_gManager.Notify(GameNotifications.GMN_EXIT, this);
		}

		public void HandleNextDown()
		{
			//SoundManager.instance.PlaySFXById(SoundManager.SFX_SELECTION);
			_pController.Notify(GameNotifications.PL_CONTINUE, this);
			_gManager.Notify(GameNotifications.GMN_CONTINUE, this);

			_resumePanel.gameObject.SetActive(false);
			_stillBg.gameObject.SetActive(false);
			_animBg.gameObject.SetActive(false);
		}
		#endregion
	}

}

