/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[CreateAssetMenu( fileName = "EntityInfoLibrary", menuName = "FF/Data/EntityInfoLibrary" )]
public class EntityInfoLibrary : ScriptableObject
{
	[SerializeField] private UIEntityInfo[] entityInfos;

	// Private Fields
	List<int> remainingInfos;

	#region API
	public UIEntityInfo GiveRandomInfo()
	{
		var randomIndex = Random.Range( 0, remainingInfos.Count );

		var infoIndex = remainingInfos[ randomIndex ];
		remainingInfos.RemoveAt( randomIndex );

		return entityInfos[ infoIndex ];
	}

	// Need to reset remainingInfo on every level change
	public void ResetRemainingInfo()
	{
		if( remainingInfos == null )
			remainingInfos = new List<int>( entityInfos.Length );

		remainingInfos.Clear();

		for( var i = 0; i < entityInfos.Length; i++ )
		{
			remainingInfos.Add( i );
		}
	}
	#endregion
}
