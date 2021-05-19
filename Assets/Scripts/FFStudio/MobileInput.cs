using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

namespace FFStudio
{
    public class MobileInput : MonoBehaviour
    {
		#region Fields
			
		[Header( "Fired Events" )]
		public SwipeInputEvent swipeInputEvent;
		public IntGameEvent tapInputEvent;
		public StringGameEvent screenTapEvent;

		[Header("Shared Variables")]
        public SharedVector3 shared_InputDirection;
		public SharedFloatPropertyTweener input_cofactor;

		// [Header("LeanFinger Components")]
		// public LeanFingerHeld leanFingerHeld;

		float deadZoneThreshold;
		float horizontalInputThreshold;
		Vector2 inputOrigin;
		LeanFingerDelegate fingerUpdate;

		#endregion

		#region UnityAPI
		private void Awake()
		{
			deadZoneThreshold                 = Screen.width * GameSettings.Instance.deadZoneThreshold / 100;
			horizontalInputThreshold          = Screen.width * GameSettings.Instance.horizontalInputPercentage / 100;
			shared_InputDirection.sharedValue = Vector3.zero;
			inputOrigin                       = Vector2.zero;

			// leanFingerHeld.MinimumAge = GameSettings.Instance.input_finger_ExprireTime;
			input_cofactor.changeDuration = GameSettings.Instance.inputAccelerationCofactorDuration;

			fingerUpdate = FingerDown;
		}		
		#endregion

		#region API
		public void Swiped( Vector2 delta )
		{
			swipeInputEvent.ReceiveInput( delta );
		}

		public void Tapped( int count )
		{
			tapInputEvent.eventValue = count;
			tapInputEvent.Raise();
		}

		public void LeanFingerUpdate(LeanFinger finger)
		{
			fingerUpdate( finger );
		}

		public void LeanFingerUp()
		{
			fingerUpdate = FingerDown;

			input_cofactor.sharedValue = 0;
			input_cofactor.KillTween();

			inputOrigin                       = Vector2.zero;
			shared_InputDirection.sharedValue = Vector3.zero;
		}
		#endregion

		#region Implementation
		void FingerDown( LeanFinger finger )
		{
			inputOrigin  = finger.ScreenPosition;
			fingerUpdate = FingerUpdate;

			input_cofactor.SetValue( 1 );

			shared_InputDirection.sharedValue = Vector3.zero;
		}

		void FingerUpdate( LeanFinger finger )
		{
			var diff = ( finger.ScreenPosition - inputOrigin );

			if(Mathf.Abs(diff.x) <= deadZoneThreshold)
				shared_InputDirection.sharedValue.x = 0;
			else 
				shared_InputDirection.sharedValue.x = GiveNormalizedHorizontal(diff.x) * GameSettings.Instance.inputHorizontalCofactor;

			shared_InputDirection.sharedValue.z = 1f;
		}		

		float GiveNormalizedHorizontal(float horizontalDiff)
		{
			return Mathf.Min( horizontalDiff / horizontalInputThreshold, 1 );
		}
		#endregion
    }
}