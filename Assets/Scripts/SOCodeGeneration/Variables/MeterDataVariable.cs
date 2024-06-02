using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	public class MeterDataEvent : UnityEvent<MeterData> { }

	[CreateAssetMenu(
	    fileName = "MeterDataVariable.asset",
	    menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "Meter Data",
	    order = 120)]
	public class MeterDataVariable : BaseVariable<MeterData, MeterDataEvent>
	{
	}
}