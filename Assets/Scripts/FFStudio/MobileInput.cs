using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

namespace FFStudio
{
    public class MobileInput : MonoBehaviour
    {
		[Header( "Fired Events" )]
		public SwipeInputEvent swipeInputEvent;
		public IntGameEvent tapInputEvent;
		public StringGameEvent screenTapEvent;

		[Header("Shared Variables")]
        public SharedVector3 shared_InputDirection;

        [Header("Lean Components")]
        public LeanFingerHeld leanFingerHeld;

        int swipeThreshold;
        Vector3 inputDirection;
        private void Awake()
		{
			swipeThreshold = Screen.width * GameSettings.Instance.swipeThreshold / 100;
            shared_InputDirection.sharedValue = inputDirection = Vector3.zero;

            leanFingerHeld.MinimumAge = GameSettings.Instance.input_finger_ExprireTime;
        }
		public void Swiped( Vector2 delta )
		{
			swipeInputEvent.ReceiveInput( delta );
		}
		public void Tapped( int count )
		{
			tapInputEvent.eventValue = count;

			tapInputEvent.Raise();
		}

		public void FingerDown(LeanFinger finger)
		{
			if(finger.ScreenPosition.x <= Screen.width / 2)
			{
                inputDirection.x = Mathf.Max(-1, inputDirection.x - 1); // Min value is -1 
				screenTapEvent.eventValue = "left";
			}
			else 
			{
                inputDirection.x = Mathf.Min(1, inputDirection.x + 1); // Max value is 1
				screenTapEvent.eventValue = "right";
            }

			screenTapEvent.Raise();

			inputDirection.z = 1;
            shared_InputDirection.sharedValue = inputDirection;
        }

		public void FingerExpire(LeanFinger finger)
		{
            shared_InputDirection.sharedValue = inputDirection = Vector3.zero;
        }


    }
}