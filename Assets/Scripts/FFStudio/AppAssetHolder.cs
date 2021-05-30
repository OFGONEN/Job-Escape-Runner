using UnityEngine;
using FFStudio;

/* This class holds referance to ScriptableObject assets. These ScriptableObjects are singleton so they need to load before a 'Scene' does.
*  Using this class unsures at least one script from a scene hold referance to these important ScriptableObjects.
*/
public class AppAssetHolder : MonoBehaviour
{

	#region Fields
	[Header( "Event Listeners" )]
	public MultipleEventListenerDelegateResponse levelChangesListener;

	public GameSettings gameSettings;
	public CurrentLevelData currentLevelData;
	public EntityInfoLibrary entityInfoLibrary;
	#endregion

	#region UnityAPI
	private void OnEnable()
	{
		levelChangesListener.OnEnable();
	}

	private void OnDisable()
	{
		levelChangesListener.OnDisable();
	}

	private void Awake()
	{
		entityInfoLibrary.ResetRemainingInfo();

		levelChangesListener.response = entityInfoLibrary.ResetRemainingInfo;
	}
	#endregion
}
