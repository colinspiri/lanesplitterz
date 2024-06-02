using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	[CreateAssetMenu(
	    fileName = "MeterDataGameEvent.asset",
	    menuName = SOArchitecture_Utility.GAME_EVENT + "Meter Data",
	    order = 120)]
	public sealed class MeterDataGameEvent : GameEventBase<MeterData>
	{
	}
}