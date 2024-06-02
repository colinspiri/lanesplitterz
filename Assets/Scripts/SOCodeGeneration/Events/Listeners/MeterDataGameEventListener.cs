using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[AddComponentMenu(SOArchitecture_Utility.EVENT_LISTENER_SUBMENU + "MeterData")]
	public sealed class MeterDataGameEventListener : BaseGameEventListener<MeterData, MeterDataGameEvent, MeterDataUnityEvent>
	{
	}
}