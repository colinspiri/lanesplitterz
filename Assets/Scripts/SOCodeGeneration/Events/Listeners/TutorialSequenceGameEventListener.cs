using ScriptableObjectArchitecture;
using SOCodeGeneration.CODE_GENERATION.Events.Game_Events;
using SOCodeGeneration.CODE_GENERATION.Events.Responses;
using UnityEngine;

namespace SOCodeGeneration.CODE_GENERATION.Events.Listeners
{
	[AddComponentMenu(SOArchitecture_Utility.EVENT_LISTENER_SUBMENU + "TutorialSequence")]
	public sealed class TutorialSequenceGameEventListener : BaseGameEventListener<TutorialSequence, TutorialSequenceGameEvent, TutorialSequenceUnityEvent>
	{
	}
}