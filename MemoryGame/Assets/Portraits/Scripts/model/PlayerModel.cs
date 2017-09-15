using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portraits
{
	public class LvlProgress
	{
		public enum PlayMode
		{
			CountDown,
			CountUp
		}

		public PlayMode CurrentPlayMode {get;set;}
		public uint Score{ get; set;}
		public int Level{ get; set;}
		public float TimeLeft { get; set; } //Just relevant for PM Count Up

		public LvlProgress(PlayMode pm, uint score, int level, float timeLeft = 0)
		{
			CurrentPlayMode = pm;
			Score = score;
			Level = level;
			TimeLeft = timeLeft;
		}
	}

	[CreateAssetMenu(menuName = "Portraits Game/Unique Models/Player Model", fileName = "New playerModel,asset")]
	public class PlayerModel : ScriptableObject
	{
		[SerializeField]
		private uint _scoredPairs;	//Scored pairs of current level
		public uint ScroredPairs{ get; set;}

		[SerializeField]
		private int _level; //Current level 
		public int Level{ get;set; }
		
		//Scored time. Mode CountDown=0 Mode Count Up: XX
		[SerializeField]
		private uint _scoredTime;    
		public uint ScoredTime { get; set; }

		[Header("Play Mode")]
		[Tooltip("Active PlayMode")]
		[SerializeField]
		private LvlProgress.PlayMode _activePlayMode;
		public LvlProgress.PlayMode ActivePlayMode{ get; set;}

		[SerializeField]
		private Dictionary<LvlProgress.PlayMode, List<LvlProgress>> progress;

		/// <summary>
		///Method to save the progress of the current level
		///In case needed overladed method passing the level and PlayMode as parameters
		/// </summary>
		public void SaveProgress()
		{
			if(progress == null)
			{
				progress = new Dictionary<LvlProgress.PlayMode, List<LvlProgress>>();
				progress[LvlProgress.PlayMode.CountDown] = new List<LvlProgress>();
				progress[LvlProgress.PlayMode.CountDown] = new List<LvlProgress>();
			}
			LvlProgress lvlProgress = new LvlProgress(_activePlayMode, _scoredPairs, _level, _scoredTime);
			progress[_activePlayMode].Add(lvlProgress);
		}

	}
}

