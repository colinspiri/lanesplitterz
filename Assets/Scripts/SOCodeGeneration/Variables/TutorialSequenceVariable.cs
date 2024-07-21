using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace SOCodeGeneration.CODE_GENERATION.Variables
{
	[System.Serializable]
	public class TutorialSequenceEvent : UnityEvent<TutorialSequence> { }

	[CreateAssetMenu(
	    fileName = "TutorialSequenceVariable.asset",
	    menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "TutorialSequence",
	    order = 120)]
	public class TutorialSequenceVariable : BaseVariable<TutorialSequence, TutorialSequenceEvent>
	{
	}
}