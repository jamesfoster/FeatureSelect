namespace FeatureSelect.AspNetCore;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IfDisabledAttribute : IfFeatureAttribute
{
    public IfDisabledAttribute(string name) : base(name, false) { }
}
