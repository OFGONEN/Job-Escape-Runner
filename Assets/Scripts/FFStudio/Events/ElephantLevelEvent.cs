using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public enum ElephantEvent
{
    LevelStarted,
    LevelCompleted,
    LevelFailed
}

[CreateAssetMenu( fileName = "ElephantEvent", menuName = "FF/Event/ElephantEvent" )]
public class ElephantLevelEvent : GameEvent
{
	public ElephantEvent elephantEventType;
	public int level;
}
