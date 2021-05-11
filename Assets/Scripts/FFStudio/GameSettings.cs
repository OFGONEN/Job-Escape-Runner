using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
        #region Fields
        public int maxLevelCount;
        [Foldout("UI Settings"), Tooltip("Duration of the movement for ui element")] public float ui_Entity_Move_TweenDuration;
        [Foldout("UI Settings"), Tooltip("Duration of the fading for ui element")] public float ui_Entity_Fade_TweenDuration;
		[Foldout("UI Settings"), Tooltip("Duration of the scaling for ui element")] public float ui_Entity_Scale_TweenDuration;
		[Foldout("UI Settings"), Tooltip("Duration of the movement for floating ui element")] public float ui_Entity_FloatingMove_TweenDuration;
        [Foldout("UI Settings"), Tooltip("Percentage of the screen to register a swipe")] public int swipeThreshold;

        [Tooltip("Time until finger held input expires")] public float input_finger_ExprireTime;

        [Foldout("Obstacle Settings")] public float obstacle_bounciness;

        [ System.Serializable ]
        public class PlayerSettings
		{
			public float force = 10000.0f;

			public float angularSpeed = 150.0f;
			[ MinMaxSlider( -90, +90 ) ]
			public Vector2 angularClamping = new Vector2( -30, +30 );
		}

		public PlayerSettings player;

		private static GameSettings instance;

        private delegate GameSettings ReturnGameSettings();
        private static ReturnGameSettings returnInstance = LoadInstance;

        public static GameSettings Instance
        {
            get
            {
                return returnInstance();
            }
        }
        #endregion

        #region Implementation
        static GameSettings LoadInstance()
        {
            if (instance == null)
                instance = Resources.Load<GameSettings>("game_settings");

            returnInstance = ReturnInstance;

            return instance;
        }

        static GameSettings ReturnInstance()
        {
            return instance;
        }
        #endregion
    }
}
