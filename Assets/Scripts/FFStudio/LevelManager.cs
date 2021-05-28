using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
	#region Fields
        [ Header( "Event Listeners" ) ]
        public EventListenerDelegateResponse levelLoadedListener;
        public EventListenerDelegateResponse levelRevealedListener;
        public EventListenerDelegateResponse levelStartedListener;
		public EventListenerDelegateResponse entityParticipateListener;
		public EventListenerDelegateResponse entityTriggeredFinishLine;
		public EventListenerDelegateResponse entityTriggeredFenceListener;
		public EventListenerDelegateResponse netTriggerListener;
		public EventListenerDelegateResponse screenTapInputListener;

		[Header( "Fired Events" ) ]
        public GameEvent levelCompleted;
        public GameEvent levelFailed;
		public IntGameEvent activateEntityRagdoll;
		public IntGameEvent resetEntityRagdoll;
		public GameEvent resetLevel;

		[Header( "Level Releated" ) ]
        public SharedFloatProperty levelProgress;
        public PhysicMaterial obstaclePhysicMaterial;
		public SharedReferenceProperty playerRigidbodyReference;
		public SharedReferenceProperty levelFinishLineReference;

		/* Private Fields */
		private Transform levelFinishLine;
		private Rigidbody playerRigidbody;
		private float playerFinishLineDistance;

		// Delegates
		private UnityMessage levelProgressCheck;
		private UnityMessage entitiesRankCheck;

		// Race Rank
		[ReadOnly , SerializeField] private List< EntityController > raceParticipants = new List< EntityController >( 4 );
		[ReadOnly , SerializeField] private List< EntityController > currentRanks     = new List< EntityController >( 4 );
		[ReadOnly , SerializeField] private List< EntityController > finishedRanks     = new List< EntityController >( 4 );


		#endregion

		#region UnityAPI
		private void OnEnable()
        {
            levelLoadedListener			.OnEnable();
            levelRevealedListener		.OnEnable();
            levelStartedListener		.OnEnable();
			netTriggerListener			.OnEnable();
			screenTapInputListener		.OnEnable();
			entityTriggeredFinishLine	.OnEnable();
			entityTriggeredFenceListener.OnEnable();
			entityParticipateListener   .OnEnable();

			playerRigidbodyReference.changeEvent += OnPlayerRigidbodyChange;
			levelFinishLineReference.changeEvent += OnLevelFinishLineChange;

			FFLogger.Log( "LevelManager OnEnable" );
		}

        private void OnDisable()
        {
            levelLoadedListener			.OnDisable();
            levelRevealedListener		.OnDisable();
            levelStartedListener		.OnDisable();
			netTriggerListener			.OnDisable();
			screenTapInputListener		.OnDisable();
			entityTriggeredFinishLine	.OnDisable();
			entityTriggeredFenceListener.OnDisable();
			entityParticipateListener   .OnDisable();

			playerRigidbodyReference.changeEvent -= OnPlayerRigidbodyChange;
			levelFinishLineReference.changeEvent -= OnLevelFinishLineChange;
        }

        private void Awake()
        {
            levelLoadedListener.response          = LevelLoadedResponse;
            levelRevealedListener.response        = LevelRevealedResponse;
            levelStartedListener.response         = LevelStartedResponse;
            netTriggerListener.response           = NetTriggeredResponse;
            entityTriggeredFenceListener.response = FenceTriggeredResponse;
            entityTriggeredFinishLine.response    = EntityTriggeredFinishLineResponse;
			entityParticipateListener.response    = EntityParticipatedRace;
			screenTapInputListener.response		  = ExtensionMethods.EmptyMethod;

			obstaclePhysicMaterial.bounciness = GameSettings.Instance.obstacle_bounciness;

			levelProgressCheck  = ExtensionMethods.EmptyMethod;
			entitiesRankCheck   = ExtensionMethods.EmptyMethod;
		}

        private void Update()
        {
			levelProgressCheck();
			entitiesRankCheck();
		}
	#endregion

	#region Implementation
        void LevelLoadedResponse()
        {
            levelProgress.SetValue(0);
			raceParticipants.Clear();
			finishedRanks.Clear();
		}

        void LevelRevealedResponse()
        {
			screenTapInputListener.response = StartChecks;
			
			entitiesRankCheck   = CheckEntityRanks;

			levelFinishLine    = levelFinishLineReference.sharedValue as Transform;
			playerFinishLineDistance = Vector3.Distance( levelFinishLine.position, playerRigidbody.position );
        }

        void LevelStartedResponse()
        {

        }

		void EntityParticipatedRace()
		{
			var changeEvent = entityParticipateListener.gameEvent as ReferenceGameEvent;
			raceParticipants.Add( changeEvent.eventValue as EntityController );
		}

        void StartChecks()
        {
			FFLogger.Log( "Checks Started!" );

			levelProgressCheck  = CheckLevelProgress;

			screenTapInputListener.response = ExtensionMethods.EmptyMethod;
		}

		void StopChecks()
		{
			levelProgressCheck  = ExtensionMethods.EmptyMethod;
		}

        void EntityTriggeredFinishLineResponse()
        {
            FFLogger.Log( "Finish Line Triggered" );
			var changeEvent = entityTriggeredFinishLine.gameEvent as ReferenceGameEvent;
			var entity = ( changeEvent.eventValue as Collider ).gameObject;
			var instanceId = entity.GetInstanceID();

			activateEntityRagdoll.eventValue = instanceId;
			activateEntityRagdoll.Raise();

			var entityController = entity.GetComponent< EntityController >();
			raceParticipants.Remove( entityController );
			finishedRanks.Add( entityController );

			FFLogger.Log( entityController.Rank + " Rank: " + entity.name );

			entityController.FinishLineCrossed();

			if(entityController.CompareTag("Player"))
			{
				FFLogger.Log( "Player finished Race at rank:" + entityController.Rank );
				StopChecks();
			}
		}

        void NetTriggeredResponse()
        {
			var changeEvent = netTriggerListener.gameEvent as ReferenceGameEvent;
			( changeEvent.eventValue as Collider ).gameObject.SetActive( false );

			// FFLogger.Log( "Disable:" + ( changeEvent.eventValue as Collider ).gameObject.name  );
		}

        void FenceTriggeredResponse()
        {
			var changeEvent = entityTriggeredFenceListener.gameEvent as ReferenceGameEvent;
			var entity = ( changeEvent.eventValue as Collider ).gameObject;
			var instanceId = entity.GetInstanceID();

			activateEntityRagdoll.eventValue = instanceId;
			activateEntityRagdoll.Raise();

			// reset entity after delay
			resetEntityRagdoll.eventValue = instanceId;
			resetEntityRagdoll.Raise();
		}

        void OnLevelFinishLineChange()
        {
			if( levelFinishLineReference.sharedValue == null )
            {
				levelFinishLine = null;
				levelProgressCheck = ExtensionMethods.EmptyMethod;
				entitiesRankCheck  = ExtensionMethods.EmptyMethod;
			}
		}
        
        void OnPlayerRigidbodyChange()
        {
            playerRigidbody = playerRigidbodyReference.sharedValue as Rigidbody;
        }

        // This method is only for UI display of level progression, not for actually deciding if the user finished the run.
        void CheckLevelProgress()
        {
			var distance = Vector3.Distance( playerRigidbody.position, levelFinishLine.position );
			var progress = distance / playerFinishLineDistance;

			if( distance <= GameSettings.Instance.finishLineDistanceThreshold )
				progress = 0;

			levelProgress.SetValue( 1 - progress );
		}

		void CheckEntityRanks()
		{
			currentRanks.Clear();

			for( var i = 0; i < raceParticipants.Count; i++ )
			{
				var entity = raceParticipants[ i ];
				entity.finishLineDistance = Vector3.Distance( levelFinishLine.position, entity.transform.position );

				currentRanks.Add( entity );
			}

			currentRanks.Sort(( x, y ) => x.finishLineDistance.CompareTo( y.finishLineDistance ) );

			for( var i = 0; i < currentRanks.Count; i++ )
			{
				currentRanks[ i ].Rank = finishedRanks.Count + i + 1;
			}
		}
		#endregion
	}
}