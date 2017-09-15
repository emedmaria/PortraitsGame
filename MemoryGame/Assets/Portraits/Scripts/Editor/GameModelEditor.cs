using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Portraits
{
	/// <summary>
	/// To Create Model from Project View. MUST be unique! 
	/// </summary>
	[CustomEditor(typeof(GameModel))]
	public class GameModelEditor : Editor
	{
		GameModel gameModel;

		public void OnEnable()
		{
			gameModel = (GameModel)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorUtility.SetDirty(gameModel);
		}

	}
}
