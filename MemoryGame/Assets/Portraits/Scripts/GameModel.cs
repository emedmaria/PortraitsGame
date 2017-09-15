using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portraits
{
	[CreateAssetMenu(menuName = "Portraits Game/Unique Models/Game Model", fileName = "New gameModel,asset")]
	public class GameModel:ScriptableObject
	{
		//Posible Game modes
		enum GameMode
		{
			CountDown,
			CountUp
		}

		[SerializeField]
		private List<PortraitsLevel> _gameLevels;           //List of levels that compound the game

		[Tooltip("Possible play modes")]
		[SerializeField]
		private GameMode playMode = GameMode.CountDown;

		public int MaxLevel
		{
			get {
				return _gameLevels != null ? _gameLevels.Count-1: 0; }
		}

		/// <summary>
		/// Given and id/index returns the data of the level
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public PortraitsLevel GetLevel(int id)
		{
			return _gameLevels!= null ? _gameLevels[id] : null;
		}


	}
}

