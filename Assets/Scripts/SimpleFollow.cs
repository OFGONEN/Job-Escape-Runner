/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class SimpleFollow : MonoBehaviour
{
#region Fields
    public Transform targetToFollow;
    public float followingOffset;
	public float lookingSpeed;
#endregion

	#region Unity API
	private void Update()
    {
		transform.position = transform.position.SetZ( targetToFollow.transform.position.z - followingOffset );
		transform.LookAtOverTimeAxis( targetToFollow.position, Vector3.up, lookingSpeed );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion
}
