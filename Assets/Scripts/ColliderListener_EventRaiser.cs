using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using UnityEditor;

public class ColliderListener_EventRaiser : MonoBehaviour
{
    #region Fields
    public event TriggerEnter triggerEnter;
    #endregion

    #region UnityAPI
    private void OnTriggerEnter(Collider other) 
    {
        triggerEnter?.Invoke( other );
    }
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
