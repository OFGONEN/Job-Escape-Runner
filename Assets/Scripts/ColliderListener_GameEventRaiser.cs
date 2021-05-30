/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using UnityEditor;

public class ColliderListener_GameEventRaiser : MonoBehaviour
{
	#region Fields
	public ReferenceGameEvent gameEvent;
	#endregion

	#region Unity API
    private void OnTriggerEnter( Collider other )
    {
		gameEvent.eventValue = other;
		gameEvent.Raise();
	}
	#endregion

	#region API
	#endregion

	#region Implementation
	#endregion

#if UNITY_EDITOR
	#region EditorOnly 
	BoxCollider boxCollider;
	private void OnDrawGizmos()
    {
        if( boxCollider == null )
		    boxCollider = GetComponent< BoxCollider >();

		Handles.color = Color.green;
		Handles.DrawWireCube( boxCollider.bounds.center, boxCollider.bounds.size );
	}
    #endregion
#endif
}
