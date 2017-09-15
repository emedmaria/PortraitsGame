using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Portraits
{
	[CustomEditor(typeof(PortraitsLevel))]
	public class PortraitsLevelEditor : Editor
	{
		PortraitsLevel pLevel;

		public void OnEnable()
		{
			pLevel = (PortraitsLevel)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (pLevel.PortraitIDs == null)
				return;

			if (pLevel.PortraitIDs.Count == 0)
			{
				EditorGUILayout.HelpBox("Empty level. PortraidsID is empty", MessageType.Info);

				if (GUILayout.Button("Generate random level", EditorStyles.miniButton))
					pLevel.Randomize();
			}
			else
			{
				if (GUILayout.Button("Update random level", EditorStyles.miniButton))
					pLevel.Randomize();
			}

            EditorUtility.SetDirty(pLevel);
		}

    }

}


