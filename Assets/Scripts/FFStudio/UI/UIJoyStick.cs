/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using NaughtyAttributes;

public class UIJoyStick : UIEntity
{
	#region Fields
	public JoyStickMethod joyStickMethod;
	public float joyStick_InputCofactor = 1;

	[ShowIf( "IfJoyStickV2" )]
	public SharedVector2 input_V2;

	[ShowIf( "IfJoyStickV3" )]
	public SharedVector3 input_V3;

	[Header("UI Elements")]
	public RectTransform joyStick;


	// Private Fields
	private Vector2 joyStick_startPosition; // Anchored
	private UnityMessage update;
	#endregion

	#region Unity API

	private void Awake()
    {
		joyStick_startPosition = joyStick.anchoredPosition;

        if(joyStickMethod == JoyStickMethod.Vector2)
			update = UpdateJoyStick_V2;
        else if(joyStickMethod == JoyStickMethod.Vector3Y)
			update = UpdateJoyStick_V3Y;
        else if(joyStickMethod == JoyStickMethod.Vector3Z)
			update = UpdateJoyStick_V3Z;
	}

    private void Update()
    {
		update();
	}

	#endregion

	#region API
	#endregion

	#region Implementation

    void UpdateJoyStick_V2()
    {
		var position = joyStick_startPosition + input_V2.sharedValue * joyStick_InputCofactor;
		joyStick.anchoredPosition = position;
	}

    void UpdateJoyStick_V3Y()
    {
 		var position = joyStick_startPosition + new Vector2(input_V3.sharedValue.x , input_V3.sharedValue.y) * joyStick_InputCofactor;
		joyStick.anchoredPosition = position;
    }

    void UpdateJoyStick_V3Z()
    {
 		var position = joyStick_startPosition + new Vector2(input_V3.sharedValue.x , input_V3.sharedValue.z) * joyStick_InputCofactor;
		joyStick.anchoredPosition = position;
    }

    bool IfJoyStickV2()
    {
		return joyStickMethod == JoyStickMethod.Vector2;
	}

    bool IfJoyStickV3()
    {
		return joyStickMethod == JoyStickMethod.Vector3Y || joyStickMethod == JoyStickMethod.Vector3Z;
	}

	#endregion
}
