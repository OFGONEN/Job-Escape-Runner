/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

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
}
