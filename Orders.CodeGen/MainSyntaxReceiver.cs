using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Orders.CodeGen;

public class AttributeAggregate : ISyntaxReceiver
{
    public List<Capture> CapturesClasses { get; } = new();
    public List<CaptureRec> CapturesRecords { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax attr || !attr.Name.ToString().Equals("RequestAttribute"))
        {
            return;
        }

        if (attr.GetParent<ClassDeclarationSyntax>() is { } clas)
        {
            CapturesClasses.Add(new Capture(clas, attr));
        }

        if (attr.GetParent<RecordDeclarationSyntax>() is { } rec)
        {
            CapturesRecords.Add(new CaptureRec(rec, attr));
        }
    }

    public record Capture(ClassDeclarationSyntax Class, AttributeSyntax Attr);

    public record CaptureRec(RecordDeclarationSyntax Record, AttributeSyntax Attr);
}

public static class NodeExtensions
{
    private static int ccc = 1;

    public static T GetParent<T>(this SyntaxNode node)
    {
        var parent = node.Parent;
        while (true)
        {
            if (parent == null)
            {
                return default;
            }

            if (parent is T t)
            {
                return t;
            }

            parent = parent.Parent;
        }
    }

    public static IEnumerable<string> GetNodeNames(this IEnumerable<SyntaxNode> nodes)
    {
        return nodes.Select(GetName);
    }

    public static string GetName(this SyntaxNode syntaxNode)
    {
        return syntaxNode switch
               {
                   ClassDeclarationSyntax cl  => cl.Identifier.Text
                 , TypeDeclarationSyntax tp   => tp.Identifier.Text
                 , PropertyDeclarationSyntax tp   => tp.Identifier.Text
                 , MemberDeclarationSyntax mb => mb.ToString()
                 , ParameterSyntax mb         => mb.Identifier.ToString()
                 , _                          => syntaxNode.ToString().Split(" ").First()
               };
    }

    

    public static void Log(this object data, string fileName = "debug.log")
    {
        try
        {
            var logData = new StringBuilder();
            if (data is IEnumerable list and not string)
            {
                foreach (var e in list)
                {
                    logData.Append(e.ToString() + "\n");
                }
            } else
            {
                logData.Append(data.ToString());
            }

            using var str = new StreamWriter(@$"F:\REPOS\Orders\Orders.CodeGen\Output\{fileName}", append: true);
            str.WriteLine(logData.ToString());
        } catch (Exception e)
        { }
    }
}