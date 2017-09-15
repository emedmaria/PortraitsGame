using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portraits
{
	public class MenuView : MonoBehaviour
	{

		//To determine visible panels
		private MenuOption _currentSelection;
		public MenuOption CurrentSelection
		{
			set
			{
				_currentSelection = value;
				UpdateDisplay();
			}
			get { return _currentSelection; }
		}

		public enum MenuOption
		{
			Options,
			Play,
			Sound,
			Exit,
			Help
		}
		//Cached components, child GO and reference
		private Transform _mTransform;
		private GameManager _gameManager;
		private SoundManager _soundManager;

		/// <summary>
		///  Panels
		/// </summary>
		private GameObject _helpPanel;
		private GameObject _optionsPanel;
		private GameObject _quitPanel;
		private GameObject _soundPanel;
		private Slider _ambientSlider;
		private Slider _sFXSlider;

		/// <summary>
		/// Properties
		/// </summary>
		public bool Active
		{
			set
			{
				_mTransform.gameObject.SetActive(value);
			}
		}

		#region MonoBehaviour methods
		void Awake()
		{
			_mTransform = this.transform;
			_currentSelection = MenuOption.Options;

			//Sub panels
			_helpPanel = _mTransform.Find("HelpPanel").gameObject;
			_optionsPanel = _mTransform.Find("OptionsPanel").gameObject;
			_quitPanel = _mTransform.Find("QuitPanel").gameObject;
			_soundPanel = _mTransform.Find("SoundPanel").gameObject;

			//Sliders
			_ambientSlider = _soundPanel.transform.Find("AmbientSlider").GetComponent<Slider>();
			_sFXSlider = _soundPanel.transform.Find("AmbientSlider").GetComponent<Slider>();

			//Suscribe to GameManager certain events
			GameManager.OnGameInitialized += UpdateDisplay;
		}

		void Start()
		{
			//Reference to Managers
			_gameManager = GameManager.instance;
			_soundManager = SoundManager.instance;
		}
		void Destroy()
		{
			//Unsuscrible  Handlers 
			GameManager.OnGameInitialized -= UpdateDisplay;
		}
		#endregion

		#region
		private void UpdateDisplay()
		{
			//GameManager.OnGameInitialized -= UpdateDisplay;
			//Active = true; 

			switch (_currentSelection)
			{
				case MenuOption.Options:
					_helpPanel.SetActive(false);
					_quitPanel.SetActive(false);
					_optionsPanel.SetActive(true);
					_soundPanel.SetActive(false);
					break;
				case MenuOption.Exit:
					_helpPanel.SetActive(false);
					_quitPanel.SetActive(true);
					_optionsPanel.SetActive(true);
					_soundPanel.SetActive(false);
					break;
				case MenuOption.Help:
					_helpPanel.SetActive(true);
					_quitPanel.SetActive(false);
					_optionsPanel.SetActive(true);
					_soundPanel.SetActive(false);
					break;
				case MenuOption.Sound:
					_soundPanel.SetActive(true);
					_helpPanel.SetActive(false);
					_quitPanel.SetActive(false);
					_optionsPanel.SetActive(true);
					break;
				case MenuOption.Play:
					_soundPanel.SetActive(false);
					Active = false;
					break;
			}
		}
		#endregion
		#region Buttons EventTrigger Handlers
		/// <summary>
		/// Menu Options Handler functions
		/// </summary>
		public void OnHandleDefaultOptions()
		{
			SoundManager.instance.PlaySFXById(SoundManager.SFX_SELECTION);
			_currentSelection = MenuOption.Options;
		}

		public void OnHandlePlaySelected()
		{
			//TODO: Change to fade
			SoundManager.instance.PlaySFXById(SoundManager.SFX_SELECTION);
			_soundManager.StopSingleByID(SoundManager.AmbientMusic.Menu);
			_currentSelection = MenuOption.Play;
			Active = false;
			_gameManager.Notify(GameNotifications.GMN_START, this);
		}

		public void OnHandleQuitSelected()
		{
			_soundManager.PlaySFXById(SoundManager.SFX_SELECTION);
			_currentSelection = MenuOption.Exit;
			UpdateDisplay();
		}

		public void OnHandleExitConfirm()
		{
			_soundManager.PlaySFXById(SoundManager.SFX_SELECTION);
			_currentSelection = MenuOption.Exit;
			Active = false;
			_gameManager.Notify(GameNotifications.GMN_EXIT, this);
		}

		public void OnHandleExitCancel()
		{
			_soundManager.PlaySFXById(SoundManager.SFX_SELECTION);
			_currentSelection = MenuOption.Options;
			UpdateDisplay();
		}

		public void OnHandleHelpSelected()
		{
			_soundManager.PlaySFXById(SoundManager.SFX_SELECTION);
			_currentSelection = MenuOption.Help;
			UpdateDisplay();
		}

		public void OnHandleHelpConfirm()
		{
			_soundManager.PlaySFXById(SoundManager.SFX_SELECTION);
			_currentSelection = MenuOption.Options;
			UpdateDisplay();
		}

		public void OnHandleSoundSelected()
		{
			_soundManager.PlaySFXById(SoundManager.SFX_SELECTION);
			_currentSelection = MenuOption.Sound;
			UpdateDisplay();
		}

		public void OnHandleSoundClose()
		{
			_soundManager.PlaySFXById(SoundManager.SFX_SELECTION);
			_currentSelection = MenuOption.Options;
			UpdateDisplay();
		}


		/// <summary>
		/// Slider controls
		/// </summary>
		public void MusicSlideChanged()
		{
			_soundManager.HandleMusicVolumeChange(_ambientSlider.value);
		}

		public void sFXSlideChanged()
		{
			_soundManager.HandleFXVolumeChange(_sFXSlider.value);
		}
		#endregion
	}
}
