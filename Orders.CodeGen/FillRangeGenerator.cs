using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var ignoredNames = attrValues.Skip(1).ToList();
        var (membersData, dd, ignoredFields) = CollectRecordFields(generationData, ignoredNames);
        if (membersData is null || dd is null)
        {
            return ("", "");
        }

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
   {AddOperators(names
               , ignoredFields
               , newName
               , generationData.Record.Identifier.ToString()
               , true )}
 
    {AddPropsRecord(names, newName, generationData.Record.Identifier.ToString(), true)}
    {AddPropsRecord(names, newName, generationData.Record.Identifier.ToString(), false)}
  }}
}}";
        return (body, newName);
    }

    public ( List<SyntaxNode>? nodes, List<string>? props, List<SyntaxNode>? ignored) CollectRecordFields(AttributeAggregate.CaptureRec generationData
                                                                                                        , List<string> ignorProps)
    {
        List<string> dd = new();
        List<SyntaxNode> membersData = new();
        List<SyntaxNode> ignored = new();
        ;
        if (generationData.Record.Members is { Count: > 0 } members)
        {
            ignored = members
                     .Where(x => ignorProps.Any(y => x.ToFullString().Split(" ").Any(z => z.Equals(y))))
                     .ToList<SyntaxNode>();
            membersData = members
                         .Where(x => !ignorProps.Any(y => x.ToFullString().Split(" ").Any(z => z.Equals(y))))
                         .ToList<SyntaxNode>();
            dd = membersData.Select(x => x.ToString())
                            .ToList();
            if (dd.Count < 1)
            {
                return (null, null, null);
            }
        }

        if (generationData.Record.ParameterList is { } pars)
        {
            ignored = pars.Parameters
                          .Where(x => ignorProps.Any(y => x.ToFullString().Split(" ").Any(z => z.Equals(y))))
                          .ToList<SyntaxNode>();
            membersData = pars.Parameters
                              .Where(x => !ignorProps.Any(y => x.ToFullString().Split(" ").Any(z => z.Equals(y))))
                              .ToList<SyntaxNode>();
            dd = membersData.Select(x => $"public {x.GetName()} {{get;init;}}")
                            .ToList();
        }

        return (membersData, dd, ignored);
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
        var allProps = generationData.Class
                                     .Members;
        var ignorPropsSyntax = allProps
                              .Where(x => ignorProps.Any(y => x.ToFullString().Split(" ").Any(z => z.Equals(y))))
                              .ToList();
        var fields = allProps
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
using System.ComponentModel.DataAnnotations;
namespace {namespaceName}
{{
  public class {newName}
  {{
     {string.Join("\n\t", fields)}
    {AddOperators(names
                , ignorPropsSyntax
                , newName
                , hostClassName
                , true
                   )}
    {AddOperators(names
                , ignorPropsSyntax
                , newName
                , hostClassName
                , false )}

 
    {AddProps(names, hostClassName, true)}
    {AddProps(names, hostClassName, false)}
   
  }}
  public static class {newName}Extensions
  {{
   public static {newName} As{newName}(this {hostClassName} entity)
   {{
        return ({newName})entity;
   }}

   public static {hostClassName} As{hostClassName}(this {newName} dto)
   {{
        return ({hostClassName})dto;
   }}

   public static IEnumerable<{newName}> As{newName}(this IEnumerable<{hostClassName}> entity)
   {{
        return  entity.Select(x=>x.As{newName}());
   }}

   public static IEnumerable<{hostClassName}> As{hostClassName}(this IEnumerable<{newName}> dtos)
   {{
        return dtos.Select(x=>x.As{hostClassName}());
   }}
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

    private string AddOperators(IEnumerable<string> fields, IEnumerable<SyntaxNode> ignored, string from, string to, bool fromDto )
    {
        var dataFields = fields
                        .Select(x => $"{x}=b.{x}")
                        .ToList();

        if (!fromDto)
        {
            foreach (var s in ignored.Select(AddIgnoredPropGenerator).Where(x => !string.IsNullOrEmpty(x)))
            {
                dataFields.Add(s);
            }
        }
       

        var toStr = fromDto ? from : to;
        var fromStr = fromDto ? to : from;
        
        return $@"
     public static explicit operator {toStr}({fromStr} b)
    {{
      return new {toStr}
        {{
          {string.Join("\t\n,", dataFields)}
        }};
    }}";
    }

    private string AddIgnoredPropGenerator(SyntaxNode s)
    {
        var propType = "";
        var name = "";
        if (s is PropertyDeclarationSyntax prop)
        {
            propType = prop.Type.ToFullString();
            name = prop.Identifier.Text;
        } else if (s is ParameterSyntax parameter)
        {
            propType = parameter.Type.ToString();
            name = parameter.Identifier.Text;
        }

        string getEnumerableCreation(string en)
        {
            return en.Replace($"{nameof(IEnumerable)}", "List");
        }

        var add = propType.Trim() switch
                  {
                      nameof(Guid)                                => $"{nameof(Guid)}.{nameof(Guid.NewGuid)}()"
                    , nameof(DateTimeOffset)                      => $"{nameof(DateTimeOffset)}.{nameof(DateTimeOffset.Now)}"
                    , nameof(DateTime)                            => $"{nameof(DateTime)}.{nameof(DateTime.Now)}"
                    , var str when str.StartsWith("List<")        => $"new()"
                    , var str when str.StartsWith("Dictionary<")  => $"new()"
                    , var str when str.StartsWith("IEnumerable<") => $"new {getEnumerableCreation(str)}()"
                    , _                                           => ""
                  };
        
       
        if (string.IsNullOrEmpty(add) || string.IsNullOrEmpty(name))
        {
            return "";
        }

        return $"{name}={add}";
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