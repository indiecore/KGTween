/*
 * Copyright (c) 2020 Kristopher Gay
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
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

		/// <summary>
		/// The tween that the inspector is currently targeting.
		/// </summary>
		private KGTween targetTween = null;

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
