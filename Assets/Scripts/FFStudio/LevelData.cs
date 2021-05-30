using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "LevelData", menuName = "FF/Data/LevelData" )]
	public class LevelData : ScriptableObject
    {
        [Scene()]
		public int sceneIndex;
        
        [ Foldout( "AI Agent" ) ]
        public float horizontalWaypointOffset = 0.5f;

		public float RandomHorizontalWaypointOffset()
        {
			return Random.Range( -horizontalWaypointOffset, +horizontalWaypointOffset );
		}
	}
}
