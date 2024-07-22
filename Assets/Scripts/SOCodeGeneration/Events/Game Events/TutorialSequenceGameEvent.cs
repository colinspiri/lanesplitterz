using ScriptableObjectArchitecture;
using UnityEngine;

namespace SOCodeGeneration.CODE_GENERATION.Events.Game_Events
{
	[System.Serializable]
	[CreateAssetMenu(
	    fileName = "TutorialSequenceGameEvent.asset",
	    menuName = SOArchitecture_Utility.GAME_EVENT + "TutorialSequence",
	    order = 120)]
	public sealed class TutorialSequenceGameEvent : GameEventBase<TutorialSequence>
	{
	}
}