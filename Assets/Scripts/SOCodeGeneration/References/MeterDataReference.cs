using UnityEngine;

namespace ScriptableObjectArchitecture
{
	[System.Serializable]
	public sealed class MeterDataReference : BaseReference<MeterData, MeterDataVariable>
	{
	    public MeterDataReference() : base() { }
	    public MeterDataReference(MeterData value) : base(value) { }
	}
}