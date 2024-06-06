using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	[CreateAssetMenu(
	    fileName = "FloatVariableGameEvent.asset",
	    menuName = SOArchitecture_Utility.GAME_EVENT + "Float Variable",
	    order = 120)]
	public sealed class FloatVariableGameEvent : GameEventBase<FloatVariable>
	{
	}
}