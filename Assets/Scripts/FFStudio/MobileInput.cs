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

        // [Header("LeanFinger Components")]
        // public LeanFingerHeld leanFingerHeld;

        int swipeThreshold;
        Vector2 inputOrigin;
		LeanFingerDelegate fingerUpdate;

		#endregion

		#region UnityAPI
		private void Awake()
		{
			swipeThreshold                    = Screen.width * GameSettings.Instance.swipeThreshold / 100;
			shared_InputDirection.sharedValue = Vector3.zero;
			inputOrigin                       = Vector2.zero;

			// leanFingerHeld.MinimumAge = GameSettings.Instance.input_finger_ExprireTime;

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

			inputOrigin                       = Vector2.zero;
			shared_InputDirection.sharedValue = Vector3.zero;
		}
		#endregion

		#region Implementation
		void FingerDown( LeanFinger finger )
		{
			inputOrigin  = finger.ScreenPosition;
			fingerUpdate = FingerUpdate;

			shared_InputDirection.sharedValue = Vector3.zero;
		}

		void FingerUpdate( LeanFinger finger )
		{
			var diff = ( finger.ScreenPosition - inputOrigin );

			if(Mathf.Abs(diff.x) <= swipeThreshold)
				shared_InputDirection.sharedValue.x = 0;
			else 
				shared_InputDirection.sharedValue.x = diff.normalized.x;

			shared_InputDirection.sharedValue.z = 1f;
		}		
		#endregion
    }
}