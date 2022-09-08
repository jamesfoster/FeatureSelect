namespace FeatureSelect;

public interface Feature
{
    T Execute<T>(Func<T> ifEnabled, Func<T> ifDisabled);

    public static readonly Feature Enabled = new EnabledImpl();
    public static readonly Feature Disabled = new DisabledImpl();

    private class EnabledImpl : Feature
    {
        public T Execute<T>(Func<T> ifEnabled, Func<T> ifDisabled) => ifEnabled();
    }

    private class DisabledImpl : Feature
    {
        public T Execute<T>(Func<T> ifEnabled, Func<T> ifDisabled) => ifDisabled();
    }
}

public static class FeatureExtensions
{
    public static void Execute(this Feature feature, Action ifEnabled, Action ifDisabled)
    {
        feature.Execute(
            () => { ifEnabled(); return 0; },
            () => { ifDisabled(); return 0; }
        );
    }
}