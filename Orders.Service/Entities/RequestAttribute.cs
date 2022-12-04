namespace Orders.Service.Entities;


[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class RequestAttribute : Attribute
{

    public RequestAttribute(string prefix,params string[] hideProps)
    {
        
    }
}