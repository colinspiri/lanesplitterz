using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	public class FloatVariableEvent : UnityEvent<FloatVariable> { }

	[CreateAssetMenu(
	    fileName = "FloatVariableVariable.asset",
	    menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "Float Variable",
	    order = 120)]
	public class FloatVariableVariable : BaseVariable<FloatVariable, FloatVariableEvent>
	{
	}
}