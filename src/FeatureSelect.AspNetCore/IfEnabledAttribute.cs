namespace FeatureSelect.AspNetCore;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IfEnabledAttribute : IfFeatureAttribute
{
    public IfEnabledAttribute(string name) : base(name, true) { }
}
