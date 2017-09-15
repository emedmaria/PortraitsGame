using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portraits
{
	public class PortraitsView : MonoBehaviour
	{
		private const float DEFAULT_POSX_GRID_5x5 = -4.72f;
		private const float DEFAULT_POSX_4X4 = -3.44f;
		private const float DEFAULT_POSY = 4f;
		private const float DEFAULT_POSX_GRID_SPACE_X = 2.3f;
		private const float DEFAULT_POSX_GRID_SPACE_Y = 2.22f;

		//Reference to data of the level to display 
		private PortraitsLevel _pLevel;

		//Different character portraits (Prefabs)
		[SerializeField]
		private GameObject[] _charactersGO;

		//Cached components
		private Transform _pTransform;
		private Transform _gTransform;

		//Instances to portraitPrefabs
		private GameObject[,] _portraitsGO;

		//Reference to GamePlayController
		private PortraitsController _gpController = null;

		//Values to display
		private int _waitingTs;
		private int _countDownTs;


		//Current gamePlay state
		private PortraitsController.GPState _currentState; 

		#region MonoBehavior methods
		void Awake()
		{
			//Cache components
			_pTransform = transform;
			_gTransform = _pTransform.Find("PortraitsGrid"); //Holder of the portraits
			_gpController = PortraitsController.Instance;
			_currentState = _gpController.CurrentState;

			//Suscribe to PortraitsController events
			SuscriptionsActive(true);
		}

		void OnDisable()
		{
			SuscriptionsActive(false);
			//Unsuscribe
		}

		void OnDestroy()
		{
			int sizeX = _portraitsGO.GetUpperBound(0) + 1;
			int sizeY = _portraitsGO.GetUpperBound(1) + 1;

			for (int i = 0; i < sizeX; i++)
			{
				for (int j = 0; j < sizeY; j++)
				{
					GameObject currentFrame = _portraitsGO[i, j];
					if (currentFrame != null)
					{
						PortraitFrame sc = currentFrame.GetComponent<PortraitFrame>();
						//Unsuscribe
						sc.OnSelection -= HandleFrameSelected;
						sc.DestroyG0();
					}
				}
			}

			//Unsuscribe to portraitsController events
			SuscriptionsActive(false);
		}
		#endregion

		#region Init View
		public void Init(PortraitsLevel pLevel)
		{
			//_charactersGO = new GameObject[PortraitsLevel.NUM_CHARACTERS];
			_pLevel = pLevel;

			//Timers 
			_waitingTs = _pLevel.ActiveWaitingTimer;
			_countDownTs = _pLevel.ActiveTimer;

			GenerateGrid();

			//Suscrible to Portraits controller events
			SuscriptionsActive(true);

			if (!enabled)
			{
				gameObject.SetActive(true);
			}
		}
		private void SuscriptionsActive(bool suscribe)
		{
			if (suscribe)
			{
				PortraitsController.OnSetUpLevel += Init;
				PortraitsController.OnStateChange += HandleGamePlayStateChange;
				PortraitsController.OnPairSelected += HandlePairSelected;
			}
			else
			{
				PortraitsController.OnSetUpLevel -= Init;
				PortraitsController.OnStateChange -= HandleGamePlayStateChange;
				PortraitsController.OnPairSelected -= HandlePairSelected;
			}
		}

		public IEnumerator DoVanish()
		{
			EnableGrid(false);
			yield return new WaitForSeconds(1.2f);
			_gTransform.gameObject.SetActive(false);
		}

		#endregion
		#region Grid related
		private void GenerateGrid()
		{
			ClearGrid(); 

			//TODO: Replace multidimensional array by jagged vect -> faster access
			int[,] gridData = _pLevel.GridData;
			int sizeX = gridData.GetUpperBound(0) + 1;
			int sizeY = gridData.GetUpperBound(1) + 1;

			float startX = _gTransform.localPosition.x;
			float startY = _gTransform.localPosition.y;

			_portraitsGO = new GameObject[sizeX, sizeY];

			for (int i = 0; i < sizeX; i++)
			{
				for (int j = 0; j < sizeY; j++)
				{
					int pId = gridData[i, j];

					GameObject portraitGO = GameObject.Instantiate(_charactersGO[pId - 1], new Vector3(startX + (i * DEFAULT_POSX_GRID_SPACE_X), startY - (j * DEFAULT_POSX_GRID_SPACE_Y), 0f),
											Quaternion.identity, _gTransform);

					//Assign ID
					PortraitFrame pf = portraitGO.GetComponent<PortraitFrame>();
					pf.PortraitID = pId;
					pf.Coordinates = new Vector2(i, j);
					pf.Enabled = false;
					//To listen PortraitFrame 
					pf.OnSelection += HandleFrameSelected;

					_portraitsGO[i, j] = portraitGO;
				}
			}
			//Reposition the holder after populate it
			//TODO: Get Ranges fromLEvel
			float posX = (sizeX == 4) ? DEFAULT_POSX_4X4 : DEFAULT_POSX_GRID_5x5;
			_gTransform.localPosition = new Vector3(posX, DEFAULT_POSY, 0f);
			
		}

		private void CharactersVisibility(bool visible)
		{
			int sizeX = _portraitsGO.GetUpperBound(0) + 1;
			int sizeY = _portraitsGO.GetUpperBound(1) + 1;

			PortraitFrame sc;
			for (int i = 0; i < sizeX; i++)
			{
				for (int j = 0; j < sizeY; j++)
				{
					GameObject currentFrame = _portraitsGO[i, j];
					sc = currentFrame.GetComponent<PortraitFrame>();
					sc.ShowSolution(visible);
				}
			}
		}

		public void Enable(bool value)
		{
			_pTransform.gameObject.SetActive(value);
		}

		private void EnableGrid(bool value)
		{
			int sizeX = _portraitsGO.GetUpperBound(0) + 1;
			int sizeY = _portraitsGO.GetUpperBound(1) + 1;

			PortraitFrame sc;
			for (int i = 0; i < sizeX; i++)
			{
				for (int j = 0; j < sizeY; j++)
				{
					GameObject currentFrame = _portraitsGO[i, j];
					sc = currentFrame.GetComponent<PortraitFrame>();
					sc.Enabled = value;
				}
			}
		}

		private void ClearGrid()
		{
			foreach (Transform child in _gTransform)
				GameObject.Destroy(child.gameObject);
		}

		#endregion

		#region Suscriptions handlers

		/// <summary>
		/// Dispatch selection to the PortraitsController - 
		/// </summary>
		/// <param name="id"></param>
		private void HandleFrameSelected(int id, Vector2 coordinates)
		{
			PortraitsController.Instance.Notify(GameNotifications.PL_PAIR_SELECTED, this, new MyCustomEventArgs(id, coordinates));
		}

		/// <summary>
		/// To listen GamePlay changes
		/// </summary>
		private void HandleGamePlayStateChange()
		{
			_currentState = _gpController.CurrentState;

			switch (_currentState)
			{
				case PortraitsController.GPState.CoolDown:
					_gTransform.gameObject.SetActive(true);
					CharactersVisibility(true);
					EnableGrid(false);
					break;
				case PortraitsController.GPState.Playing:
					_gTransform.gameObject.SetActive(true);
					CharactersVisibility(false);
					EnableGrid(true);
					break;
				case PortraitsController.GPState.OutOfTime:
					EnableGrid(false);
					//HandleSuccess();
					StartCoroutine(DoVanish());
					break;
				case PortraitsController.GPState.Success:
					_gTransform.gameObject.SetActive(false);
					EnableGrid(false);
					StartCoroutine(DoVanish());
					break;
			}
		}
		/// <summary>
		/// Handler to update the display of the portraits depending on the match
		/// Match: Remove cards 
		/// No Match: Hides results again
		/// </summary>
		/// <param name="selection1"></param>
		/// <param name="selection2"></param>
		/// <param name="isMatch"></param>
		private void HandlePairSelected(CustomPair selection1, CustomPair selection2, bool isMatch)
		{
			PortraitFrame pf;
			//Portraits vanish
			if (isMatch)
			{
				pf = _portraitsGO[(int)selection1.Value.x, (int)selection1.Value.y].GetComponent<PortraitFrame>();
				StartCoroutine(pf.DoVanish());
				pf = _portraitsGO[(int)selection2.Value.x, (int)selection2.Value.y].GetComponent<PortraitFrame>();
				StartCoroutine( pf.DoVanish());
			}
			else //Hide Character
			{
				pf = _portraitsGO[(int)selection1.Value.x, (int)selection1.Value.y].GetComponent<PortraitFrame>();
				StartCoroutine(pf.DoFlip(true));
				pf = _portraitsGO[(int)selection2.Value.x, (int)selection2.Value.y].GetComponent<PortraitFrame>();
				StartCoroutine(pf.DoFlip(true));
			}
		}
		#endregion
	}
}
