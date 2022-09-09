namespace FeatureSelect.NUnit;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class IgnoreIfDisabledAttribute : IgnoreIfFeatureAttribute
{
    public IgnoreIfDisabledAttribute(string name) : base(name, ignoreIfDisabled: true)
    {
    }
}