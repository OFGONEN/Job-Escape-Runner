/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class ApplyForceToPlayer : MonoBehaviour
{
#region Fields
    public Vector3 forceToApply;
#endregion

#region Unity API
    private void OnTriggerEnter( Collider other )
    {
        if( other.gameObject.layer != 6 )
			return;

		other.GetComponent< Rigidbody >().AddForce( forceToApply );
        FFLogger.Log( "Collided with " + other.name );
    }
    
    private void OnDrawGizmos()
    {
		Gizmos.DrawRay( transform.position, forceToApply.normalized * 3 );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion
}