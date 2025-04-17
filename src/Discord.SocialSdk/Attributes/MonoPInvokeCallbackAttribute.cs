namespace AOT;

[AttributeUsage(AttributeTargets.Method)]
public partial class MonoPInvokeCallbackAttribute : Attribute
{
    public MonoPInvokeCallbackAttribute(Type type)
    {

    }
}