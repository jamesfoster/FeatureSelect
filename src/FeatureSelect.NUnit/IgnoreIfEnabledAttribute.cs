namespace FeatureSelect.NUnit;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class IgnoreIfEnabledAttribute : IgnoreIfFeatureAttribute
{
    public IgnoreIfEnabledAttribute(string name) : base(name, ignoreIfDisabled: false)
    {
    }
}