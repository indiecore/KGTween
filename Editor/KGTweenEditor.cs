using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KGTools.Animation
{

	[CustomEditor(typeof(KGTween), true)]
	public class KGTweenEditor : Editor
	{

		#region Data

		KGTween targetTween = null;

		#endregion

		#region Editor Logic

		/// <summary>
		/// On enable we cache the target as the correct type.
		/// </summary>
		protected void OnEnable()
		{
			this.targetTween = this.target as KGTween;
		}

		/// <summary>
		/// Override the inspector GUI to add some controls to the bottom of the tween panel.
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Set Start To Current Value"))
			{
				Undo.RecordObject(this.target, "Set start to current value");
				this.targetTween.SetStartToCurrentValue();
			}

			if (GUILayout.Button("Set End To Current Value"))
			{
				Undo.RecordObject(this.target, "Set End to current value");
				this.targetTween.SetEndToCurrentValue();
			}
		}

		#endregion

	}

}
