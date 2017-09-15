using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Portraits
{
	[CreateAssetMenu(menuName = "Portraits Game/Levels/New portrait level", fileName = "New portraits level,asset")]
	public class PortraitsLevel : ScriptableObject
	{
		[Header("Identifier of the level")]
		[SerializeField]
		private int _id;
		public int Id
		{
			get;
			set;
		}
		/// <summary>
		/// Constants
		/// </summary>
		public const int NUM_CHARACTERS = 10;
		private const int DEFAULT_ITEMS_ROW = 4;
		private const int MAX_ITEMS_ROW = 5;

		public enum GridType
		{
			Square,
			Random,
		}

		//[Header("Grid Type - Distribution of the portraits")]
		//[Tooltip("Active Grid Type")]
		//[SerializeField]
		private GridType _activeGridType;
		public GridType ActiveGridType
		{
			get;
			set;
		}

		//[Header("Level settings")]
		//[Tooltip("# Pairs to match")]
		//[SerializeField]
		private int _numPairs;
		public int NumPairs
		{
			get;
			set;
		}

		//[Header("Count down timer")]
		//[Tooltip("# max Time in seconds")]
		//[SerializeField]
		private int _activeTimer;
		public int ActiveTimer
		{
			get
			{
				if (_activeTimer == 0)
				{
					_activeTimer = countDownTs;
				}
				return _activeTimer;
			}
		}

		//[Header("Waiting time until the game starts")]
		//[Tooltip("# max Time in seconds")]
		//[SerializeField]
		private int _activeWaitingTimer;
		public int ActiveWaitingTimer
		{
			get
			{
				if (_activeWaitingTimer == 0)
				{
					_activeWaitingTimer = waitingTs;
				}
				return _activeWaitingTimer;
			}
		}

		//Field - Current Portraits to show - lenght = numPairs*2
		[Header("Portraits shown")]
		[Tooltip("It's mandatory duplicate each ID in order to have a match!")]
		[SerializeField]
		private List<int> _portraitIDs;
		public List<int> PortraitIDs
		{
			get { return _portraitIDs; }
		}

		//Property - map the portrait IDs in a grid
		private int[,] _gridData;           //TODO: Change to jagged vector 
		public int[,] GridData
		{
			get
			{
				if (_gridData == null)
				{
					ListToGrid();
				}
				return _gridData;
			}
		}

		private int _itemsByRow = DEFAULT_ITEMS_ROW;

        /// <summary>
        /// This Random generator would usually suit better in an EditorClass
        /// Nontheles, he it will allow us to generate random data at runtime
        /// </summary>
        /// 

        //public  static readonly int[] RANGE_COOLDOWN_TS = new int[2]{ 3, 10 };
        public const int RANGE_COOLDOWN_TS_MIN = 3;
        public const int RANGE_COOLDOWN_TS_MAX = 10;
        public const int RANGE_TIMER_TS_MIN = 15;
        public const int RANGE_TIMER_TS_MAX = 80;
        public  const int RANGE_PAIRS_MIN = 6;
        public  const int RANGE_PAIRS_MAX = 10;

        [Header("Random Settings for the level")]

        [Tooltip("Waiting time (seconds)")]
        [Range(RANGE_COOLDOWN_TS_MIN, RANGE_COOLDOWN_TS_MAX)]
        [SerializeField]
        private int waitingTs; 
        public int WaitingTs
        {
            get { return waitingTs; }
        }
		[Tooltip("Count down time (seconds)")]
		[Range(RANGE_TIMER_TS_MIN, RANGE_TIMER_TS_MAX)]
		[SerializeField]
		private int countDownTs = RANGE_TIMER_TS_MIN;

        [Tooltip("Number of pairs to match")]
        [Range(RANGE_PAIRS_MIN, RANGE_PAIRS_MAX)]
        [SerializeField]
        private int nPairs;

		[Tooltip("Distribution of the portraits")]
		[SerializeField]
		private GridType gridType = GridType.Square;

        public void OnValidate()
        {
            //Update Active values with settings values
            _numPairs = nPairs;

            //Update values with the chosed in the inspector
            _activeWaitingTimer = waitingTs;
            _activeTimer = countDownTs;
            _activeGridType = gridType;
            //_activePlayMode = playMode; 

        }

        private int NumDuplicates(int[] idList, int id)
		{
			int cont = 0;
			for (int i = 0; i < idList.Length; i++)
			{
				if (idList[i] == id) cont++;
			}
			return cont;
		}

		public void Randomize()
		{
			//Update active parameters
			_numPairs = nPairs;
			_activeWaitingTimer = waitingTs;
			_activeTimer = countDownTs;
			//_activePlayMode = playMode;
			_activeGridType = gridType;

			_portraitIDs = new List<int>();

			//Define items per row
			_itemsByRow = nPairs % MAX_ITEMS_ROW == 0 ? MAX_ITEMS_ROW : DEFAULT_ITEMS_ROW;
			int itemsByCol = (nPairs * 2) / _itemsByRow;

			//TODO: Change to jagged vector instead multidimensional
			_gridData = new int[_itemsByRow, itemsByCol];

			int lenght = nPairs * 2;
			int[] usedIDs = new int[lenght];
			int i;
			for (i = 0; i < lenght; i++)
			{
				int id;
				int numDuplicates;
				int index;
				do
				{
					id = UnityEngine.Random.Range(1, nPairs + 1); 
					numDuplicates = NumDuplicates(usedIDs, id);
				} while (numDuplicates == 2);

				do
				{
					index = UnityEngine.Random.Range(0, lenght);

				} while (usedIDs[index] != 0);

				usedIDs[index] = id;
			}

			//Fill List and gridData
			int iX = 0;
			int iY = 0;

			for (i = 0; i < usedIDs.Length; i++)
			{
				_portraitIDs.Add(usedIDs[i]);

				if (i > 0 && (i % itemsByCol != 0))
				{
					iX = i / itemsByCol;
					iY++;
				}
				else
				{
					iX = i / itemsByCol;
					iY = 0;
				}

				_gridData[iX, iY] = usedIDs[i];
			}
		}

		private void ListToGrid()
		{
			//Fill List and gridData
			int iX = 0;
			int iY = 0;
			int i = 0;

			if (_portraitIDs == null || _portraitIDs.Count == 0)
			{
				Debug.Log("Grid cannot be created out of an empty list of portraitIDS");
				return;
			}

			//Define items per row
			_itemsByRow = nPairs % MAX_ITEMS_ROW == 0 ? MAX_ITEMS_ROW : DEFAULT_ITEMS_ROW;
			int itemsByCol = (nPairs * 2) / _itemsByRow;
			_gridData = new int[_itemsByRow, itemsByCol];

			for (i = 0; i < _portraitIDs.Count; i++)
			{
				if (i > 0 && (i % itemsByCol != 0))
				{
					iX = i / itemsByCol;
					iY++;
				}
				else
				{
					iX = i / itemsByCol;
					iY = 0;
				}

				_gridData[iX, iY] = _portraitIDs[i];
			}
		}


    }
}

