/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class AgentController : EntityController
{
#region Fields

#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
#endregion

#region EntityController Overrides
	protected override Vector3 InputSource()
	{
		throw new System.NotImplementedException();
        
        // TODO: Implement.
	}

	protected override float InputCofactor()
	{
		return 1.0f; // Won't use this for agents.
	}

	protected override float RigidbodyMass()
	{
		return GameSettings.Instance.aIAgent.rigidBody_Mass;
	}

	protected override float RigidbodyDrag()
	{
		return GameSettings.Instance.aIAgent.rigidBody_Drag;
	}
#endregion
}
