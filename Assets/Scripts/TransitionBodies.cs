/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FFStudio;
using NaughtyAttributes;

public class TransitionBodies : MonoBehaviour
{
#region Fields
    [InfoBox("Order of the array is the order of the rank")]
    public Transform[] transitionTargets;
    public TransitionBodySet bodySet;
#endregion

#region Unity API
    private void OnEnable()
    {
        for( var i = 0; i < transitionTargets.Length; i++ )
			bodySet.AddDictionary( i, transitionTargets[ i ] );
	}

    private void OnDisable()
    {
         for( var i = 0; i < transitionTargets.Length; i++ )
			bodySet.RemoveDictionary( i );
    }
#endregion

#region API
#endregion

#region Implementation
#endregion

#if UNITY_EDITOR
	#region EditorOnly 
	private void OnDrawGizmos()
    {
		Handles.color = Color.blue;

		for( var i = 0; i < transitionTargets.Length; i++ )
        {
			Handles.DrawWireCube( transitionTargets[ i ].position, Vector3.one / 2 );
		}
	}

	public Transform[] dollSpines;

	[Button]
    private void SetTargetPositions()
    {
        for( var i = 0; i < transitionTargets.Length; i++ )
        {
			transitionTargets[ i ].position = dollSpines[ i ].position;
			transitionTargets[ i ].rotation = dollSpines[ i ].rotation;
		}
	}
    #endregion
#endif
}
