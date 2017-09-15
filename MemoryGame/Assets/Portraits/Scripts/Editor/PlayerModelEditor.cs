using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Portraits
{
	/// <summary>
	/// To Create Model from Project View. MUST be unique! 
	/// </summary>
	[CustomEditor(typeof(PlayerModel))]
	public class PlayerModelEditor : Editor
	{
		PlayerModel playerModel;

		public void OnEnable()
		{
			playerModel = (PlayerModel)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorUtility.SetDirty(playerModel);
		}

	}
}

