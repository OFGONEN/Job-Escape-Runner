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
    public Rigidbody[] transitionBodies;
    public TransitionBodySet bodySet;
#endregion

#region Unity API
    private void OnEnable()
    {
        for( var i = 0; i < transitionBodies.Length; i++ )
			bodySet.AddDictionary( i, transitionBodies[ i ] );
	}

    private void OnDisable()
    {
         for( var i = 0; i < transitionBodies.Length; i++ )
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

		for( var i = 0; i < transitionBodies.Length; i++ )
        {
			Handles.DrawWireCube( transitionBodies[ i ].position, Vector3.one / 2 );
		}
	}
    #endregion
#endif
}
