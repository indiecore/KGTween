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
using UnityEngine;

namespace KGTools.Animation
{
	public abstract class TweenColour : KGTween
	{

		#region Data

		/// <summary>
		/// The starting colour for the tween.
		/// </summary>
		[SerializeField, Header("----- Tween Colour -----")]
		protected Color startColour = Color.white;

		/// <summary>
		/// The end colour for the tween.
		/// </summary>
		[SerializeField]
		protected Color endColour = Color.white;

		/// <summary>
		/// The operating colour for this tween.
		/// </summary>
		protected abstract Color Colour
		{
			get;
			set;
		}

		#endregion

		#region Implementation Specific

		/// <summary>
		/// This function handles the implementation specific logic for the tween.
		/// </summary>
		/// <param name="tweenFactor">The specific value to use for the tween operation.</param>
		/// <param name="isFinished">Is the tweening operation finished or not.</param>
		protected override void HandleTweenUpdate(float tweenFactor, bool isFinished)
		{
			this.Colour = Color.Lerp(startColour, endColour, tweenFactor);
		}

		/// <summary>
		/// Sets the start function to the current value of the object it is on.
		/// </summary>
		public override void SetStartToCurrentValue()
		{
			this.startColour = this.Colour;
		}

		/// <summary>
		/// Set the end of the tween value to the current value this component's object is at.
		/// </summary>
		public override void SetEndToCurrentValue()
		{
			this.endColour = this.Colour;
		}

		#endregion

	}
}
