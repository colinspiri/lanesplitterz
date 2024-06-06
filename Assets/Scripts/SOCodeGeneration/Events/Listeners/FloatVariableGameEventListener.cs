using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[AddComponentMenu(SOArchitecture_Utility.EVENT_LISTENER_SUBMENU + "FloatVariable")]
	public sealed class FloatVariableGameEventListener : BaseGameEventListener<FloatVariable, FloatVariableGameEvent, FloatVariableUnityEvent>
	{
	}
}