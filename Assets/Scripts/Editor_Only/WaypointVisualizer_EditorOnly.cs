/* Created by and for usage of FF Studios (2021). */

using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

[ ExecuteAlways ]
public class WaypointVisualizer_EditorOnly : MonoBehaviour
{
#region Fields
	public Transform waypointsParent;
	[ ReadOnly ] private Transform[] waypoints;
#endregion

#region Unity API
#if UNITY_EDITOR // Required when Handles API is used, as it causes issues on build.
	private void OnDrawGizmos()
	{
        if( enabled == false || waypoints == null || waypoints.Length == 0 )
			return;

		Handles.color = Color.black;
        
		for( var i = 0; i < waypoints.Length - 1; i++ )
		{
			Gizmos.color = Color.black;
			Handles.DrawSolidDisc( waypoints[ i     ].position, Vector3.up, 1.0f );
			Handles.DrawSolidDisc( waypoints[ i + 1 ].position, Vector3.up, 1.0f );

			Handles.DrawDottedLine( waypoints[ i ].position, waypoints[ i + 1 ].position, 10.0f );
		}
	}
    
    private void Update()
    {
		var newWaypoints = waypointsParent.GetComponentsInChildren< Transform >();

		waypoints = newWaypoints.Where( ( newWaypoint, index ) => newWaypoint != waypointsParent ).ToArray();
    }
#endif
#endregion

#region API
#endregion

#region Implementation
#endregion
}
