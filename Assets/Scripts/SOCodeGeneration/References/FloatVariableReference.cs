using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	public sealed class FloatVariableReference : BaseReference<FloatVariable, FloatVariableVariable>
	{
	    public FloatVariableReference() : base() { }
	    public FloatVariableReference(FloatVariable value) : base(value) { }
	}
}