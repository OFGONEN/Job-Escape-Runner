/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class SimpleCameraFollow : MonoBehaviour
{
#region Fields
    public Transform targetToFollow;
    public float followingOffset;
#endregion

#region Unity API
    private void Update()
    {
		transform.position = transform.position.SetZ( targetToFollow.transform.position.z - followingOffset );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion
}
