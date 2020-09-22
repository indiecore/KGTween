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
	/// <summary>
	/// This class is the base class for all component tweeners.
	/// The goal for this is to have a class that can be added to game objects to do basic tweening operations.
	/// </summary>
	public abstract class KGTween : MonoBehaviour
	{

		#region Const

		/// <summary>
		/// The default maximum timestep to use when the 
		/// </summary>
		private const float DEFAULT_MAX_TIMESTEP = 1f;

		#endregion

		#region Enums

		/// <summary>
		/// The tween style of this tween.
		/// </summary>
		public enum TweenStyle
		{
			/// <summary>
			/// This tween will play once and then remain at the end value.
			/// </summary>
			Once,
			/// <summary>
			/// This tween will continue to play; after completing it will reset to the beginning of the tween next frame.
			/// </summary>
			Loop,
			/// <summary>
			/// This tween will continue to play, reversing direction when the tween is completed.
			/// </summary>
			PingPong
		}

		#endregion

		#region Data

		/// <summary>
		/// Defines the animation curve that is used for the tween.
		/// Defaults to ease-in/ease-out.
		/// </summary>
		[SerializeField, Header("----- KG Tween -----")]
		private AnimationCurve tweenCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

		/// <summary>
		/// The tweening style for this tween.
		/// </summary>
		[SerializeField]
		public TweenStyle tweenStyle = TweenStyle.Once;

		/// <summary>
		/// The amount of time in seconds that this tween should take to go from start to end.
		/// </summary>
		[SerializeField]
		private float tweenLength = 1f;

		/// <summary>
		/// Should this tweener run in real time.
		/// </summary>
		[SerializeField]
		private bool useRealTime = false;

		/// <summary>
		/// Should this tween start playing automatically when it is enabled.
		/// </summary>
		[SerializeField]
		private bool startOnEnable = true;

		/// <summary>
		/// Amount of time in seconds to wait between starting and 
		/// </summary>
		[SerializeField]
		private float waitOnStartTime = 0f;

		[SerializeField, Header("----- Events -----")]
		private UnityEngine.Events.UnityEvent onComplete = null;
		/// <summary>
		/// This function will be called when the tween is completed. 
		/// The tween component will still be active when this callback is called.
		/// It will only be called during TweenMode.Once
		/// </summary>
		public UnityEngine.Events.UnityEvent OnComplete
		{
			get
			{
				return this.onComplete;
			}
		}

		[SerializeField]
		private UnityEngine.Events.UnityEvent onLoop = null;
		/// <summary>
		/// Called whenever the tweener reaches the end and resets or changes direction.
		/// </summary>
		public UnityEngine.Events.UnityEvent OnLoop
		{
			get
			{
				return this.onLoop;
			}
		}

		[SerializeField, Tooltip("Called whenever the tween gets it's value updated.")]
		private UnityEngine.Events.UnityEvent onValueUpdated = null;
		/// <summary>
		/// Called whenever the tween value updates.
		/// </summary>
		public UnityEngine.Events.UnityEvent OnValueUpdated
		{
			get
			{
				return this.onValueUpdated;
			}
		}

		/// <summary>
		/// The current "position" of the tween. Always a value between 0 and 1, 0 being the start of the tween, 1 being the end.
		/// </summary>
		private float tweenPosition = 0f;

		/// <summary>
		/// The internal working value for the duration of the 
		/// </summary>
		private float currentDuration = 0f;


		private float amountPerDelta = 0f;
		/// <summary>
		/// Calculates the amount changed per delta time.
		/// </summary>
		public float AmountPerDelta
		{
			get
			{

				if (this.currentDuration != this.tweenLength)
				{
					this.currentDuration = this.tweenLength;
					this.amountPerDelta = Mathf.Abs((tweenLength > 0f) ? 1f / tweenLength : DEFAULT_MAX_TIMESTEP);
				}

				return this.amountPerDelta;
			}
		}

		/// <summary>
		/// The game time that this tween should start processing from.
		/// </summary>
		private float startTime = 0f;

		/// <summary>
		/// Has this tween started. 
		/// </summary>
		private bool isStarted = false;

		/// <summary>
		/// If set to play once this flag will be set to true once the game has finished.
		/// </summary>
		private bool isFinished = false;

		private bool isPlaying = false;
		/// <summary>
		/// Is this tween currently playing.
		/// </summary>
		public bool IsPlaying
		{
			get
			{
				return this.isPlaying;
			}
		}

		#endregion

		#region MonoBehaviour

		/// <summary>
		/// When this tweener disables we should 
		/// </summary>
		protected virtual void OnDisable()
		{
			this.isStarted = false;
			this.isFinished = false;
			this.isPlaying = false;
		}

		/// <summary>
		/// On enable check if the start on enable value is set to true and if it is force start the tween.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (this.startOnEnable)
			{
				this.ResetToBeginning(true);
			}
		}

		/// <summary>
		/// On start update immediately so there is no delay in frame processing.
		/// </summary>
		protected virtual void Start()
		{
			this.Update();
		}

		/// <summary>
		/// Update the tween.
		/// </summary>
		protected virtual void Update()
		{
			if (!this.IsPlaying)
			{
				return;
			}

			float frameDeltaTime = this.useRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
			float time = this.useRealTime ? Time.unscaledTime : Time.time;
		
			if (!this.isStarted)
			{
				this.isStarted = true;
				this.startTime = time + this.waitOnStartTime;
			}

			// Don't process any tween updates until the start time has come.
			if (time < this.startTime)
			{
				return;
			}

			// Once the tween has started we simply need to sample the value.

			this.tweenPosition += this.AmountPerDelta * frameDeltaTime;

			this.isFinished = false;
			// Check if the tween has finished.
			switch (this.tweenStyle)
			{
			case TweenStyle.Once:
				if (this.tweenPosition > 1f || this.tweenPosition < 0f) {
					// Clamp overflow of the tween position.
					this.tweenPosition = Mathf.Clamp01(this.tweenPosition);
					this.isFinished = true;

					// Manually sample to set the tween to the completed end.
					this.Sample();

					// On Complete only gets called when the tween will finish tweening.
					if (this.OnComplete != null)
					{
						this.OnComplete.Invoke();
					}

					this.isPlaying = false;
				}

				break;
			case TweenStyle.Loop:
				// We only need to check for going over the end of the position
				if (this.tweenPosition > 1f)
				{
					// Reset the tween factor to near 0, keeping any remainder.
					// Keeping the remainder will eliminate hitches when the timing doesn't conform to a frame barrier.
					this.tweenPosition -= Mathf.Floor(this.tweenPosition);

					if (this.OnLoop != null)
					{
						this.OnLoop.Invoke();
					}
				}

				break;
			case TweenStyle.PingPong:
				// With the ping pong we need to check both the 
				if (this.tweenPosition > 1f)
				{
					// Set the tween factor to the remainder of whatever overtween we got.
					this.tweenPosition = 1f - (this.tweenPosition - Mathf.Floor(this.tweenPosition));
					
					// Reverse the tween direction.
					this.amountPerDelta *= -1f;
				}
				else if (this.tweenPosition < 0f)
				{
					// Invert the tween factor so the remainder is above zero.
					this.tweenPosition *= -1f;
					this.tweenPosition -= Mathf.Floor(this.tweenPosition);

					// Reverse direction.
					this.amountPerDelta *= -1;
				}

				break;
			default:
				Debug.LogError("Do not recognize TweenMode: " + this.tweenStyle.ToString());
				break;
			}

			if (!this.isFinished)
			{
				this.Sample();
			}
		}

		/// <summary>
		/// When added to a component call the set values.
		/// </summary>
		protected virtual void Reset() 
		{
			if (!this.isStarted) 
			{
				this.SetStartToCurrentValue();
				this.SetEndToCurrentValue();

				this.ResetToBeginning();
			}
		}

		#endregion

		#region Tween Controls

		/// <summary>
		/// Play this tween in the forward direction.
		/// </summary>
		public void PlayForward()
		{
			this.Play();
		}

		/// <summary>
		/// Play this tween in reverse.
		/// </summary>
		public void PlayReverse()
		{
			this.Play(false);
		}

		/// <summary>
		/// Play this tween.
		/// </summary>
		/// <param name="playForward">If set to <c>true</c> play the tween forward, otherwise play it backwards.</param>
		public void Play(bool playForward = true)
		{
			this.amountPerDelta = Mathf.Abs(this.AmountPerDelta);
			
			if (!playForward)
			{
				this.amountPerDelta *= -1f;
			}

			this.isPlaying = true;
		}

		/// <summary>
		/// Immediately stop playing this tween.
		/// </summary>
		public void Stop()
		{
			this.isPlaying = false;
		}

		/// <summary>
		/// Resets this tween to the beginning of the tween.
		/// </summary>
		/// <param name="forceStart">Force the tween to start.</param>
		public void ResetToBeginning(bool forceStart = false)
		{
			this.isStarted = false;

			if (forceStart)
			{
				this.isPlaying = true;
			}

			this.tweenPosition = (this.AmountPerDelta < 0f) ? 1f : 0f;
			this.Sample();
		}

		/// <summary>
		/// Sets the tween factor to zero.
		/// </summary>
		public virtual void SetTweenToStart()
		{
			this.tweenPosition = 0f;
			this.Sample(true);
		}

		/// <summary>
		/// Sets the tween factor directly to 1.
		/// </summary>
		public virtual void SetTweenToEnd() {
			this.tweenPosition = 1f;
			this.Sample(true);
		}

		/// <summary>
		/// Handles sampling from the animation curve for the tweener.
		/// </summary>
		/// <param name="forceFinishValue">Should we force the finished value to a specific state.</param>
		private void Sample(bool? forceFinishValue = null)
		{
			if (forceFinishValue == null || !forceFinishValue.HasValue)
			{
				this.HandleTweenUpdate(this.tweenCurve.Evaluate(this.tweenPosition), this.isFinished);
			}
			else
			{
				this.HandleTweenUpdate(this.tweenCurve.Evaluate(this.tweenPosition), forceFinishValue.Value);
			}

			if (this.onValueUpdated != null)
			{
				this.onValueUpdated.Invoke();
			}
		}

		#endregion

		#region Implementation Specific

		/// <summary>
		/// This function handles the implementation specific logic for the tween.
		/// </summary>
		/// <param name="tweenFactor">The specific value to use for the tween operation.</param>
		/// <param name="isFinished">Is the tweening operation finished or not.</param>
		protected abstract void HandleTweenUpdate(float tweenFactor, bool isFinished);

		/// <summary>
		/// Sets the start function to the current value of the object it is on.
		/// </summary>
		public abstract void SetStartToCurrentValue();

		/// <summary>
		/// Set the end of the tween value to the current value this component's object is at.
		/// </summary>
		public abstract void SetEndToCurrentValue();

		#endregion

	}
}
