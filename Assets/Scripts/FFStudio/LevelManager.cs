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

        [Header("Fired Events")]
        public GameEvent levelCompleted;
        public GameEvent levelFailedEvent;
		public GameEvent activatePlayerRagdoll;

		[Header("Level Releated")]
        public SharedFloatProperty levelProgress;
        public PhysicMaterial obstaclePhysicMaterial;
		public SharedReferenceProperty playerRigidbodyReference;


		// Private Fields
		private Rigidbody playerRigidbody;
		private UnityMessage update;
		private float playerMomentumTime;
		#endregion

		#region UnityAPI

		private void OnEnable()
        {
            levelLoadedListener  .OnEnable();
            levelRevealedListener.OnEnable();
            levelStartedListener .OnEnable();

			playerRigidbodyReference.changeEvent += OnPlayerRigidbodyChange;
		}

        private void OnDisable()
        {
            levelLoadedListener  .OnDisable();
            levelRevealedListener.OnDisable();
            levelStartedListener .OnDisable();

			playerRigidbodyReference.changeEvent -= OnPlayerRigidbodyChange;
        }

        private void Awake()
        {

            levelLoadedListener.response   = LevelLoadedResponse;
            levelRevealedListener.response = LevelRevealedResponse;
            levelStartedListener.response  = LevelStartedResponse;

            obstaclePhysicMaterial.bounciness = GameSettings.Instance.obstacle_bounciness;

			update = ExtensionMethods.EmptyMethod;
		}

        private void Update()
        {
			update();
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
        
        void OnPlayerRigidbodyChange()
        {
            if(playerRigidbodyReference.sharedValue == null)
            {
				update = ExtensionMethods.EmptyMethod;
				playerMomentumTime = 0;
			}
            else 
            {
                playerRigidbody = playerRigidbodyReference.sharedValue as Rigidbody;
				playerMomentumTime = 0;
				update = CheckPlayerMomentum;
			}
        }

        void CheckPlayerMomentum()
        {
            if(playerMomentumTime >= GameSettings.Instance.player.momentum_CountDownTime)
            {
                FFLogger.Log( "Player lost momentum" );
				activatePlayerRagdoll.Raise();
				update = ExtensionMethods.EmptyMethod;
			}

			if(playerRigidbody.velocity.magnitude <= GameSettings.Instance.player.momentum_Magnitude)
				playerMomentumTime += Time.deltaTime;
            else
				playerMomentumTime = 0;
		}

        #endregion
    }
}