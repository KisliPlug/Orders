using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Orders.CodeGen;

[Generator]
public class FillRangeGen : ISourceGenerator
{
#region Explicit interface implementation

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeAggregate());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        "Run".Log("debug.log");
        if (context.SyntaxReceiver is not AttributeAggregate receiver)
        {
            return;
        }

        try
        {
            foreach (var data in receiver.CapturesRecords.Select(CreateRecord))
            {
                if (data is ({ } source, { } fileName))
                {
                    context.AddSource($"{fileName}.g.cs", source);
                }
            }

            foreach (var clasz in receiver.CapturesClasses)
            {
                var data = CreateClass(clasz);
                if (data is ({ } source, { } fileName))
                {
                    context.AddSource($"{fileName}.g.cs", source);
                }
            }
        } catch (Exception e)
        {
            e.Log("error.log");
        }
    }

#endregion

    private (string? source, string? fileName)? CreateRecord(AttributeAggregate.CaptureRec generationData)
    {
        var attrValues = generationData.Attr.ArgumentList!.Arguments.Select(x => x.Expression.ToString())
                                       .Select(x => x.Replace("nameof(", "").Replace(")", "").Replace($"\"", ""))
                                       .ToList();
        if (attrValues.Count < 1)
        {
            return (null, null);
        }

        var newName = $"{attrValues.First()}{generationData.Record.Identifier}Dto";
        var namecpaceName = GetNamespaceName(generationData.Record);
        List<ParameterSyntax> fields = null;
        var (membersData, dd) = CollectRecordFields(generationData, attrValues.Skip(1).ToList());
        if (membersData is null || dd is null)
        {
            return ("", "");
        }

        AddOperators(membersData.GetNodeNames(), newName, generationData.Record.Identifier.ToString(), true).Log();
        // {AddOperators(membersData, newName, generationData.Record.Identifier.ToString(), false)}

        // {AddProps(membersData, namecpaceName, false)}
        var names = membersData.GetNodeNames().ToArray();
       
        var body = $@"
using System.ComponentModel;
namespace {namecpaceName}
{{
  public record {newName} 
  {{
  {string.Join("\n", dd)} 
   {AddOperators(names, newName, generationData.Record.Identifier.ToString(), true)}
    {AddPropsRecord(names, newName, generationData.Record.Identifier.ToString(), true)}
    {AddPropsRecord(names, newName, generationData.Record.Identifier.ToString(), false)}
  }}
}}";
        return (body, newName);
    }

    public ( List<SyntaxNode>? nodes, List<string>? props) CollectRecordFields(AttributeAggregate.CaptureRec generationData, List<string> ignorProps)
    {
        List<string> dd = new();
        List<SyntaxNode> membersData = new();
        ;
        if (generationData.Record.Members is { Count: > 0 } members)
        {
            membersData = members
                         .Where(x => !ignorProps.Any(y => x.ToFullString().Split(" ").Any(z => z.Equals(y))))
                         .ToList<SyntaxNode>();
            dd = membersData.Select(x => x.ToString())
                            .ToList();
            if (dd.Count < 1)
            {
                return (null, null);
            }
        }

        if (generationData.Record.ParameterList is { } pars)
        {
            membersData = pars.Parameters
                              .Where(x => !ignorProps.Any(y => x.ToFullString().Split(" ").Any(z => z.Equals(y))))
                              .ToList<SyntaxNode>();
            dd = membersData.Select(x => $"{x.GetAttributes()} public {x.GetName()} {{get;init;}}")
                            .ToList();
        }

        return (membersData, dd);
    }

    private (string? source, string? fileName)? CreateClass(AttributeAggregate.Capture generationData)
    {
        var attrValues = generationData.Attr.ArgumentList!.Arguments.Select(x => x.Expression.ToString())
                                       .Select(x => x.Replace("nameof(", "").Replace(")", "").Replace($"\"", ""))
                                       .ToList();
        if (attrValues.Count < 1)
        {
            return (null, null);
        }

        var ignorProps = attrValues.Skip(1);
        var newName = $"{attrValues.First()}{generationData.Class.Identifier}Dto";
        var namespaceName = GetNamespaceName(generationData.Class);
        var fields = generationData.Class
                                   .Members
                                   .Where(x => !ignorProps.Any(y => x.ToFullString().Split(" ").Any(z => z.Equals(y))))
                                   .ToList();
        if (fields.Count < 1)
        {
            return ("", "");
        }

        var hostClassName = generationData.Class.Identifier.ToString();
        var names = fields.GetNodeNames().ToArray();
        var body = $@"
using System.ComponentModel;
namespace {namespaceName}
{{
  public class {newName}
  {{
     {string.Join("\n\t", fields)}
       {AddOperators(names, newName, hostClassName, true)}
    {AddOperators(names, newName, hostClassName, false)}
    {AddProps(names, hostClassName, true)}
    {AddProps(names, hostClassName, false)}
   
  }}
}}";
        return (body, newName);
    }

    private string GetNamespaceName(SyntaxNode classDeclarationSyntax)
    {
        var retVal = "Default";
        if (classDeclarationSyntax.GetParent<NamespaceDeclarationSyntax>() is { } sp)
        {
            retVal = sp.Name.ToString();
        } else if (classDeclarationSyntax.GetParent<FileScopedNamespaceDeclarationSyntax>() is { } scp)
        {
            retVal = scp.Name.ToString();
        }

        return retVal;
    }

    private string AddOperators(IEnumerable<string> fields, string from, string to, bool isImplicit)
    {
        var dataFields = fields
                        .Select(x => $"{x}=b.{x}")
                        .ToList();
        var toStr = isImplicit ? from : to;
        var fromStr = isImplicit ? to : from;
        return $@"
     public static explicit operator {toStr}({fromStr} b)
    {{
      return new {toStr}
        {{
          {string.Join("\t\n,", dataFields)}
        }};
    }}";
    }

    private string AddPropsRecord(IEnumerable<string> fields, string pocoName, string dtoName, bool set)
    {
        var retInditefer = set ? "dto" : "this";

        string setProp(string propName)
        {
            return $"{propName}=this.{propName}";
        }

        string getProp(string propName)
        {
            return $"{propName}=dto.{propName}";
        }

        var funcPrefix = set ? "Set" : "Get";
        Func<string, string> func = set ? setProp : getProp;
        var dataFields = fields
           .Select(x => func(x));
        var retType = set ? dtoName : pocoName;
        return $@"
     public {retType} {funcPrefix}Props({dtoName} dto)
    {{
     
           return  {retInditefer} with {{{string.Join("\t\n,", dataFields)}}};
         
    }}";
    }

    private string AddProps(IEnumerable<string> fields, string name, bool set)
    {
        string setProp(string propName)
        {
            return $"b.{propName}={propName};";
        }

        string getProp(string propName)
        {
            return $"{propName}=b.{propName};";
        }

        var funcPrefix = set ? "Set" : "Get";
        Func<string, string> func = set ? setProp : getProp;
        var dataFields = fields
           .Select(x => func(x));
        return $@"
     public   void  {funcPrefix}Props({name} b)
    {{
     
            {string.Join("\t\n", dataFields)}
         
    }}";
    }
}