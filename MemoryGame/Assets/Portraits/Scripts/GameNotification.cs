using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portraits
{
	public class GameNotifications
	{
		/// <summary>
		/// Global Game Status Notifications
		/// </summary>
		public const string GMN_START = "playGame";
		public const string GMN_EXIT = "exitGame";
        //public const string GMN_UPDATE_MATCH = "match";
        public const string GMN_RESUME_GAME = "resume";
        public const string GMN_CONTINUE = "continueNext";
		public const string GMN_RETRY = "retryLevel";

		/// <summary>
		/// GamePlay (Level) Notifications to /PortraitsController
		/// </summary>
		public const string	PL_PAIR_SELECTED = "pairSelected";
		public const string PL_TIME_OUT = "timeOut";
		public const string PL_RESTART = "restart";
		public const string PL_CONTINUE = "next";
		//public const string PL_MATCH = "match";

	}

    public class MyCustomEventArgs : System.EventArgs
    {
        public bool DoOverride { get; set; }
        public int id { get; private set; }
        public Vector2 coordinates { get; private set; }
		public float? timeLeft { get; private set; }

		public MyCustomEventArgs(int id, Vector2 coordinates,float? timeLeft = null)
        {
            DoOverride = false;
            this.id = id;
            this.coordinates = coordinates;
			this.timeLeft = timeLeft;
        }
    }

}

