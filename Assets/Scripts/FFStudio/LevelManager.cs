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
		public EventListenerDelegateResponse playerTriggeredNetListener;

		[Header("Fired Events")]
        public GameEvent levelCompleted;
        public GameEvent levelFailedEvent;
		public GameEvent activatePlayerRagdoll;

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
		private float playerMomentumTime;
		private float finishLineDistance;
		#endregion

		#region UnityAPI

		private void OnEnable()
        {
            levelLoadedListener       .OnEnable();
            levelRevealedListener     .OnEnable();
            levelStartedListener      .OnEnable();
			playerTriggeredFinishLine .OnEnable();
			playerTriggeredNetListener.OnEnable();

			playerRigidbodyReference.changeEvent += OnPlayerRigidbodyChange;
			levelFinishLineReference.changeEvent += OnLevelFinishLineChanged;
		}

        private void OnDisable()
        {
            levelLoadedListener       .OnDisable();
            levelRevealedListener     .OnDisable();
            levelStartedListener      .OnDisable();
			playerTriggeredFinishLine .OnDisable();
			playerTriggeredNetListener.OnDisable();

			playerRigidbodyReference.changeEvent -= OnPlayerRigidbodyChange;
			levelFinishLineReference.changeEvent -= OnLevelFinishLineChanged;
        }

        private void Awake()
        {
            levelLoadedListener.response        = LevelLoadedResponse;
            levelRevealedListener.response      = LevelRevealedResponse;
            levelStartedListener.response       = LevelStartedResponse;
            playerTriggeredNetListener.response = PlayerTriggeredNetResponse;
			playerTriggeredFinishLine.response  = PlayerTriggeredFinishLineResponse;

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
        }

        void LevelRevealedResponse()
        {

        }

        void LevelStartedResponse()
        {

        }

        void PlayerTriggeredFinishLineResponse()
        {
            FFLogger.Log( "Finish Line Triggered" );
            //TODO: close input
            //TODO: start second phase ? 
        }

        void PlayerTriggeredNetResponse()
        {
			// level fail seqeunce
            FFLogger.Log( "A Net Triggered" );
			activatePlayerRagdoll.Raise();
			levelFailedEvent.Raise();
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

				levelProgressCheck  = CheckLevelProgress;
			}
		}
        
        void OnPlayerRigidbodyChange()
        {
            if(playerRigidbodyReference.sharedValue == null)
            {
				playerMomentumCheck = ExtensionMethods.EmptyMethod;
				playerMomentumTime = 0;
			}
            else 
            {
                playerRigidbody = playerRigidbodyReference.sharedValue as Rigidbody;
				playerMomentumTime = 0;
				playerMomentumCheck = CheckPlayerMomentum;
			}
        }

        // This method is only for UI display of level progression not for actually deciding if the user finish the run
        void CheckLevelProgress()
        {
			var distance = Vector3.Distance( playerRigidbody.position, levelFinishLine.position );
			var progress = distance / finishLineDistance;

			if(distance <= GameSettings.Instance.finishLineDistanceThreshold)
				progress = 1;

			levelProgress.SetValue( 1 - progress );
		}

        void CheckPlayerMomentum()
        {
            if(playerMomentumTime >= GameSettings.Instance.player.momentum_CountDownTime)
            {
                FFLogger.Log( "Player lost momentum" );
				activatePlayerRagdoll.Raise();
				levelFailedEvent.Raise();
				playerMomentumCheck = ExtensionMethods.EmptyMethod;
			}

			if(playerRigidbody.velocity.magnitude <= GameSettings.Instance.player.momentum_Magnitude)
				playerMomentumTime += Time.deltaTime;
            else
				playerMomentumTime = 0;
		}

        #endregion
    }
}