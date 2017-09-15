using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portraits
{
	public class Loader : MonoBehaviour
	{
        /// <summary>
        /// Prefabs to instantieate
        /// </summary>
		public GameObject gameManager;          
		public GameObject soundManager;        

        #region MonoBehaviour methods
        void Awake()
		{
            //In case there is no reference. Instantiate them
			if (GameManager.instance == null)
				Instantiate(gameManager); 

            if (SoundManager.instance == null)
				Instantiate(soundManager); 

        }
        #endregion
	}
}

