using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;


namespace Portraits
{
    public class SoundManager : MonoBehaviour
    {
        /// <summary>
        /// In case we can different Ambient sound for the menu and in game
        /// </summary>
        public enum AmbientMusic {
            Menu,
            InGame
        };

        /// <summary>
        /// Sound FX
        /// </summary>
        public const int SFX_SELECTION = 0;
        public const int SFX_COUNT_TICK = 1;
        public const int SFX_GOOD_MATCH = 2;
		public const int SFX_LEVEL_FAILED = 3;
		public const int SFX_LEVELUP = 4;
		public const int SFX_SUCCESS = 5;

		/// <summary>
		/// Default volume settings
		/// </summary>
		private const float DEFAULT_AMBIENT_VOLUME = .3f;
		private const float DEFAULT_SFX_VOLUME = .5f;

        /// <summary>
        /// Audio Source components an Audio Clips
        /// </summary>
        [SerializeField]
        private AudioSource _ambientSource;
		[SerializeField]
		private AudioSource _ambientInGameSource;
		private AudioSource _sFxSource;

        [SerializeField]
        [Tooltip("All sound effects available")]
        private AudioClip[] _soundFXs;

		private Slider _sliderAmbient;
		private Slider _sliderSFX;

		/// <summary>
		/// Properties related to sound volume
		/// </summary>
		public float MusicVolume { get;set;}
        public float SFXVolume{get; set;}


        #region Singleton
        public static SoundManager instance = null;
        #endregion

        #region MonoBehaviour methods
        void Awake()
        {
			// Check if there are any other instances conflicting
			if (instance != null && instance != this)
				Destroy(gameObject);
			
			// Singleton instance
			instance = this;

			//Not to destroy on reload Scene
			DontDestroyOnLoad(gameObject);
		}

		void Start()
        {
			if (_sFxSource == null)
				_sFxSource = gameObject.AddComponent<AudioSource>();

			//Initialize sound levels
			_ambientSource.volume = DEFAULT_AMBIENT_VOLUME;
			_ambientInGameSource.volume = DEFAULT_AMBIENT_VOLUME;
			_sFxSource.volume = DEFAULT_SFX_VOLUME;
            PlaySingleByID(AmbientMusic.Menu);
        }

        #endregion

        #region AudioSource control methods
        public void PlaySingleByID(AmbientMusic id)
        {
            switch (id)
            {
                case AmbientMusic.Menu:
                    _ambientSource.Play();
                    break;
                case AmbientMusic.InGame:
                    _ambientInGameSource.Play();
                    break;
            }
        }
        #endregion

        public void StopSingleByID(AmbientMusic id)
        {
			switch (id)
			{
				case AmbientMusic.Menu:
					_ambientSource.Stop();
					break;
				case AmbientMusic.InGame:
					_ambientInGameSource.Stop();
					break;
			}
		}

        public void HandleMusicVolumeChange(float value)
        {
            _ambientSource.volume = value;
			_ambientInGameSource.volume = value;
		}

        public void HandleFXVolumeChange(float value)
        {
            _sFxSource.volume = value;
        }

        /// <summary>
        /// Given an ID looks within the array of sound FXs, assigns the AudioClip to the Audiosource and plays de clip
        /// </summary>
        /// <param name="id"></param>
        public void PlaySFXById(int id)
        {
            _sFxSource.clip = _soundFXs[id];
            _sFxSource.Play();
        }
	}
}
