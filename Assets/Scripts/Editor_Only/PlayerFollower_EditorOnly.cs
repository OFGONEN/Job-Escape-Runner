/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class PlayerFollower_EditorOnly : MonoBehaviour
{
#region Fields
    public SharedReferenceProperty transformToFollow;
    public float followingOffset;
#endregion

#region Unity API
    private void Update()
    {
        if( transformToFollow.sharedValue == null )
			return;
            
		transform.position = transform.position.SetZ( ( transformToFollow.sharedValue as Rigidbody ).transform.position.z - followingOffset );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion
}
