using ScriptableObjectArchitecture;
using SOCodeGeneration.CODE_GENERATION.Variables;

namespace SOCodeGeneration.CODE_GENERATION.References
{
	[System.Serializable]
	public sealed class TutorialSequenceReference : BaseReference<TutorialSequence, TutorialSequenceVariable>
	{
	    public TutorialSequenceReference() : base() { }
	    public TutorialSequenceReference(TutorialSequence value) : base(value) { }
	}
}