using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
        #region Fields
        [Header("Event Listeners")]
        public EventListenerDelegateResponse levelLoadedListener;
        public EventListenerDelegateResponse levelRevealedListener;
        public EventListenerDelegateResponse levelStartedListener;
		public EventListenerDelegateResponse playerTriggeredFinishLine;
		public EventListenerDelegateResponse playerTriggeredFenceListener;
		public EventListenerDelegateResponse netTriggerListener;
		public EventListenerDelegateResponse screenTapInputListener;

		[Header("Fired Events")]
        public GameEvent levelCompleted;
		public IntGameEvent activateEntityRagdoll;
		public GameEvent resetLevel;

		[Header("Level Releated")]
        public SharedFloatProperty levelProgress;
        public PhysicMaterial obstaclePhysicMaterial;
		public SharedReferenceProperty playerRigidbodyReference;
		public SharedReferenceProperty levelFinishLineReference;


		// Private Fields
		private Transform levelFinishLine;
		private Rigidbody playerRigidbody;
		private UnityMessage playerMomentumCheck;
		private UnityMessage levelProgressCheck;
		private float playerLowMomentumTimer;
		private float finishLineDistance;
		#endregion

		#region UnityAPI

		private void OnEnable()
        {
            levelLoadedListener                .OnEnable();
            levelRevealedListener              .OnEnable();
            levelStartedListener               .OnEnable();
			netTriggerListener                 .OnEnable();
			screenTapInputListener                   .OnEnable();
			playerTriggeredFinishLine          .OnEnable();
			playerTriggeredFenceListener       .OnEnable();

			playerRigidbodyReference.changeEvent += OnPlayerRigidbodyChange;
			levelFinishLineReference.changeEvent += OnLevelFinishLineChanged;
		}

        private void OnDisable()
        {
            levelLoadedListener                .OnDisable();
            levelRevealedListener              .OnDisable();
            levelStartedListener               .OnDisable();
			netTriggerListener                 .OnDisable();
			screenTapInputListener                   .OnDisable();
			playerTriggeredFinishLine          .OnDisable();
			playerTriggeredFenceListener       .OnDisable();

			playerRigidbodyReference.changeEvent -= OnPlayerRigidbodyChange;
			levelFinishLineReference.changeEvent -= OnLevelFinishLineChanged;
        }

        private void Awake()
        {
            levelLoadedListener.response          = LevelLoadedResponse;
            levelRevealedListener.response        = LevelRevealedResponse;
            levelStartedListener.response         = LevelStartedResponse;
            netTriggerListener.response           = GroundTriggeredResponse;
            playerTriggeredFenceListener.response = PlayerTriggeredNetResponse;
            playerTriggeredFinishLine.response    = PlayerTriggeredFinishLineResponse;
			screenTapInputListener.response             = ExtensionMethods.EmptyMethod;

			obstaclePhysicMaterial.bounciness = GameSettings.Instance.obstacle_bounciness;

			playerMomentumCheck = ExtensionMethods.EmptyMethod;
			levelProgressCheck  = ExtensionMethods.EmptyMethod;
		}

        private void Update()
        {
			playerMomentumCheck();
			levelProgressCheck();
		}

        #endregion

        #region Implementation
        void LevelLoadedResponse()
        {
            levelProgress.SetValue(0);

			screenTapInputListener.response = StartChecks;
		}

        void LevelRevealedResponse()
        {

        }

        void LevelStartedResponse()
        {

        }

        void StartChecks()
        {
			playerMomentumCheck = CheckPlayerMomentum;
			levelProgressCheck  = CheckLevelProgress;

			screenTapInputListener.response = ExtensionMethods.EmptyMethod;
		}

        void PlayerTriggeredFinishLineResponse()
        {
            FFLogger.Log( "Finish Line Triggered" );
			//TODO: close input
			//TODO: start second phase ? 
			//TODO: Level reset maybe ? 
		}

        void GroundTriggeredResponse()
        {
			var changeEvent = netTriggerListener.gameEvent as ReferenceGameEvent;
			( changeEvent.eventValue as Collider ).gameObject.SetActive( false );

            FFLogger.Log( "Disable:" + ( changeEvent.eventValue as Collider ).gameObject.name  );
		}

        void PlayerTriggeredNetResponse()
        {

			var changeEvent = playerTriggeredFenceListener.gameEvent as ReferenceGameEvent;
			var instanceId = ( changeEvent.eventValue as Collider ).gameObject.GetInstanceID();

			var go = ( changeEvent.eventValue as Collider ).gameObject.name;
			FFLogger.Log( "Fence: " + go );

			activateEntityRagdoll.eventValue = instanceId;
			activateEntityRagdoll.Raise();
		}

        void OnLevelFinishLineChanged()
        {
            if(levelFinishLineReference.sharedValue == null)
            {
				levelFinishLine = null;
				levelProgressCheck = ExtensionMethods.EmptyMethod;
			}
            else 
            {
				levelFinishLine     = levelFinishLineReference.sharedValue as Transform;
				finishLineDistance  = Vector3.Distance( levelFinishLine.position, playerRigidbody.position );
			}
		}
        
        void OnPlayerRigidbodyChange()
        {
            if( playerRigidbodyReference.sharedValue == null )
            {
				playerMomentumCheck = ExtensionMethods.EmptyMethod;
				playerLowMomentumTimer = 0;
			}
            else 
            {
                playerRigidbody = playerRigidbodyReference.sharedValue as Rigidbody;
				playerLowMomentumTimer = 0;
			}
        }

        // This method is only for UI display of level progression not for actually deciding if the user finish the run
        void CheckLevelProgress()
        {
			var distance = Vector3.Distance( playerRigidbody.position, levelFinishLine.position );
			var progress = distance / finishLineDistance;

			if( distance <= GameSettings.Instance.finishLineDistanceThreshold )
				progress = 0;

			levelProgress.SetValue( 1 - progress );
		}

        void CheckPlayerMomentum()
        {
            if( playerLowMomentumTimer >= GameSettings.Instance.player.lowMomentum_TimeThreshold )
            {
                FFLogger.Log( "Player lost momentum" );
				activateEntityRagdoll.Raise();
				playerMomentumCheck = ExtensionMethods.EmptyMethod;
			}

			if( playerRigidbody.velocity.magnitude <= GameSettings.Instance.player.lowMomentum_Threshold )
				playerLowMomentumTimer += Time.deltaTime;
            else
				playerLowMomentumTimer = 0;
		}

        #endregion
    }
}