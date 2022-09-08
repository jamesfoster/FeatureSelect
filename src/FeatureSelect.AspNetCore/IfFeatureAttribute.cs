namespace FeatureSelect.AspNetCore;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IfFeatureAttribute : Attribute
{
    public IfFeatureAttribute(string name, bool enabled = true)
    {
        Name = name;
        Enabled = enabled;
    }

    public string Name { get; }
    public bool Enabled { get; }
}