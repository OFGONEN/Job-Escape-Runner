﻿using System.Collections;
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

        [Foldout("Input Settings"), Tooltip("Percentage of the screen to register a horizontal input")] public float deadZoneThreshold;
        [Foldout("Input Settings"), Tooltip("How much shorten the screen")] public float horizontalInputPercentage = 25f;
        [Foldout("Input Settings"), Tooltip("Duration for input cofactor to reach intented value")] public float inputAccelerationCofactorDuration;
        [Foldout("Input Settings"), Tooltip("Cofactor for x value of the input")] public float inputHorizontalCofactor = 0.5f;

        [Tooltip("Time until finger held input expires")] public float input_finger_ExprireTime;

        [Foldout("Obstacle Settings")] public float obstacle_bounciness;
        [Foldout("Obstacle Settings")] public float obstacle_rotating_forceToApply;
        
        [ Foldout( "Physics" ) ] public float force        = 7500.0f;
        [ Foldout( "Physics" ) ] public float angularSpeed = 150.0f;
        
        [ Foldout( "Physics" ), MinMaxSlider( -90, +90 ) ]
        public Vector2 angularClamping = new Vector2( -30, +30 );
        
        [ Foldout( "Physics" ), Tooltip( "Velocity of the player" ) ]
        public float velocityClamp = 15;

		[Tooltip( "Threshold distance for level progress to be 1" )] public float finishLineDistanceThreshold = 2f;
		[ System.Serializable ]
        public class PlayerSettings
		{
            [ Tooltip( "Mass of the player rigidbody" ) ]
			public float rigidBody_Mass = 5;

            [ Tooltip( "Drag of the player rigidbody" ) ]
			public float rigidBody_Drag = 1;

			[ Tooltip( "If user exceeds this time without having enough momentum level fails" ) ] 
			public float lowMomentum_TimeThreshold = 1;

            [ Tooltip( "Threshold value for momentum countdown to be count" ) ]
			public float lowMomentum_Threshold = 0.1f;
		}
        
		[ System.Serializable ]
        public class AIAgentSettings
		{
            [ Tooltip( "Mass of the player rigidbody" ) ]
			public float rigidBody_Mass = 5;

            [ Tooltip( "Drag of the player rigidbody" ) ]
			public float rigidBody_Drag = 1;
		}
        
		public PlayerSettings player;
		public AIAgentSettings aIAgent;

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
