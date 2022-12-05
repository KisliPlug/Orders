namespace Orders.Service.Entities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class RequestAttribute : Attribute
{
    public RequestAttribute(string prefix, params string[] hideProps)
    { }
}
