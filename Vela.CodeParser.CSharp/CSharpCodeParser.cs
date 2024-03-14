using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace Vela.CodeParser.CSharp
{
    public class CSharpCodeParser : ICodeParser
    {
        public string Language => "CSharp";

        public BaseCodeNodeInfo Parser(string code)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
            BaseCodeNodeInfo codeParserResult = new BaseCodeNodeInfo();
            parserChildNodes(root.ChildNodes(), codeParserResult);

            return codeParserResult;
        }

        void parserChildNodes(IEnumerable<SyntaxNode> childNodes, BaseCodeNodeInfo output)
        {
            foreach (var node in childNodes)
            {
                if (node.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    #region NamespaceDeclaration
                    var ns = (NamespaceDeclarationSyntax)node;
                    NameSpaceInfo nameSpaceInfo = new NameSpaceInfo();

                    if (output.Items == null)
                        output.Items = new List<BaseCodeNodeInfo>();
                    ((IList)output.Items).Add(nameSpaceInfo);

                    nameSpaceInfo.Name = ns.Name.ToString();
                    parserChildNodes(ns.ChildNodes(), nameSpaceInfo);
                    #endregion
                }
                else if (node.IsKind(SyntaxKind.ClassDeclaration))
                {
                    #region ClassDeclaration
                    var classDeclaration = (ClassDeclarationSyntax)node;
                    ClassInfo classInfo = new ClassInfo();
                    if (output.Items == null)
                        output.Items = new List<BaseCodeNodeInfo>();
                    ((IList)output.Items).Add(classInfo);

                    classInfo.Name = classDeclaration.Identifier.ToString();

                    ((List<string>)(classInfo.Modifiers??= new List<string>())).AddRange(classDeclaration.Modifiers.Select(m => m.ToString()));
                    classInfo.Caption = GetDocumentationComment(classDeclaration);

                    foreach (var attrlist in classDeclaration.AttributeLists)
                    {
                        foreach (var attr in attrlist.Attributes)
                        {
                            var attrInfo = new AttributeInfo();
                            ((IList)(classInfo.Attributes ??= new List<AttributeInfo>())).Add(attrInfo);

                            attrInfo.Name = attr.Name.ToString();
                            attrInfo.Arguments = attr.ArgumentList.Arguments.Select(m => m.ToString()).ToArray();
                        }
                    }

                    parserChildNodes(classDeclaration.ChildNodes(), classInfo);
                    #endregion
                }
                else if (node.IsKind(SyntaxKind.InterfaceDeclaration))
                {
                    #region ClassDeclaration
                    var interfaceDeclaration = (InterfaceDeclarationSyntax)node;
                    var interfaceInfo = new InterfaceInfo();
                    if (output.Items == null)
                        output.Items = new List<BaseCodeNodeInfo>();
                    ((IList)output.Items).Add(interfaceInfo);

                    interfaceInfo.Name = interfaceDeclaration.Identifier.ToString();
                    ((List<string>)(interfaceInfo.Modifiers ??= new List<string>())).AddRange(interfaceDeclaration.Modifiers.Select(m => m.ToString()));
                    interfaceInfo.Caption = GetDocumentationComment(interfaceDeclaration);

                    foreach (var attrlist in interfaceDeclaration.AttributeLists)
                    {
                        foreach (var attr in attrlist.Attributes)
                        {
                            var attrInfo = new AttributeInfo();
                            ((IList)(interfaceInfo.Attributes ??= new List<AttributeInfo>())).Add(attrInfo);

                            attrInfo.Name = attr.Name.ToString();
                            attrInfo.Arguments = attr.ArgumentList.Arguments.Select(m => m.ToString()).ToArray();
                        }
                    }

                    parserChildNodes(interfaceDeclaration.ChildNodes(), interfaceInfo);
                    #endregion
                }
                else if (node.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    #region PropertyDeclaration
                    var propertyDeclaration = (PropertyDeclarationSyntax)node;

                    PropertyInfo propertyInfo = new PropertyInfo();
                    if (output.Items == null)
                        output.Items = new List<BaseCodeNodeInfo>();
                    ((IList)output.Items).Add(propertyInfo);

                    propertyInfo.Name = propertyDeclaration.Identifier.ToString();
                    ((List<string>)(propertyInfo.Modifiers ??= new List<string>())).AddRange(propertyDeclaration.Modifiers.Select(m => m.ToString()));
                    propertyInfo.Type = propertyDeclaration.Type.ToString();
                    propertyInfo.Caption = GetDocumentationComment(propertyDeclaration);

                    foreach (var attrlist in propertyDeclaration.AttributeLists)
                    {
                        foreach (var attr in attrlist.Attributes)
                        {
                            var attrInfo = new AttributeInfo();
                            ((IList)(propertyInfo.Attributes ??= new List<AttributeInfo>())).Add(attrInfo);

                            attrInfo.Name = attr.Name.ToString();
                            attrInfo.Arguments = attr.ArgumentList.Arguments.Select(m => m.ToString()).ToArray();
                        }
                    }
                    #endregion
                }
                else if (node.IsKind(SyntaxKind.FieldDeclaration))
                {
                    #region FieldDeclaration
                    var fieldDeclaration = (FieldDeclarationSyntax)node;

                    foreach (var field in fieldDeclaration.Declaration.Variables)
                    {
                        FieldInfo fieldInfo = new FieldInfo();
                        if (output.Items == null)
                            output.Items = new List<BaseCodeNodeInfo>();
                        ((IList)output.Items).Add(fieldInfo);

                        fieldInfo.Name = field.Identifier.ToString();
                        ((List<string>)(fieldInfo.Modifiers ??= new List<string>())).AddRange(fieldDeclaration.Modifiers.Select(m => m.ToString()));
                        fieldInfo.Type = fieldDeclaration.Declaration.Type.ToString();
                        fieldInfo.Caption = GetDocumentationComment(fieldDeclaration);

                        foreach (var attrlist in fieldDeclaration.AttributeLists)
                        {
                            foreach (var attr in attrlist.Attributes)
                            {
                                var attrInfo = new AttributeInfo();
                                ((IList)(fieldInfo.Attributes ??= new List<AttributeInfo>())).Add(attrInfo);

                                attrInfo.Name = attr.Name.ToString();
                                attrInfo.Arguments = attr.ArgumentList.Arguments.Select(m => m.ToString()).ToArray();
                            }
                        }
                    }

                    #endregion
                }
                else if (node.IsKind(SyntaxKind.MethodDeclaration))
                {
                    #region MethodDeclaration
                    var methodDeclaration = (MethodDeclarationSyntax)node;

                    var methodInfo = new MethodInfo();
                    if (output.Items == null)
                        output.Items = new List<BaseCodeNodeInfo>();
                    ((IList)output.Items).Add(methodInfo);

                    ((List<string>)(methodInfo.Modifiers ??= new List<string>())).AddRange(methodDeclaration.Modifiers.Select(m => m.ToString()));
                    methodInfo.ReturnType = methodDeclaration.ReturnType.ToString();
                    methodInfo.Caption = GetDocumentationComment(methodDeclaration);

                    foreach (var parameter in methodDeclaration.ParameterList.Parameters)
                    {
                        var parameterInfo = new ParameterInfo();
                        ((List<ParameterInfo>)(methodInfo.Parameters ??= new List<ParameterInfo>())).Add(parameterInfo);

                        parameterInfo.Name = parameter.Identifier.ToString();
                        parameterInfo.Type = parameter.Type.ToString();
                    }

                    foreach (var attrlist in methodDeclaration.AttributeLists)
                    {
                        foreach (var attr in attrlist.Attributes)
                        {
                            var attrInfo = new AttributeInfo();
                            ((IList)(methodInfo.Attributes ??= new List<AttributeInfo>())).Add(attrInfo);

                            attrInfo.Name = attr.Name.ToString();
                            attrInfo.Arguments = attr.ArgumentList.Arguments.Select(m => m.ToString()).ToArray();
                        }
                    } 
                    #endregion
                }
                else if (node.IsKind(SyntaxKind.EnumDeclaration))
                {
                    #region EnumDeclaration
                    var enumDeclaration = (EnumDeclarationSyntax)node;

                    var enumInfo = new EnumInfo();
                    if (output.Items == null)
                        output.Items = new List<BaseCodeNodeInfo>();
                    ((IList)output.Items).Add(enumInfo);

                    enumInfo.Name = enumDeclaration.Identifier.ToString();
                    ((List<string>)(enumInfo.Modifiers ??= new List<string>())).AddRange(enumDeclaration.Modifiers.Select(m => m.ToString()));
                    enumInfo.Caption = GetDocumentationComment(enumDeclaration);

                    foreach (var attrlist in enumDeclaration.AttributeLists)
                    {
                        foreach (var attr in attrlist.Attributes)
                        {
                            var attrInfo = new AttributeInfo();
                            ((IList)(enumInfo.Attributes ??= new List<AttributeInfo>())).Add(attrInfo);

                            attrInfo.Name = attr.Name.ToString();
                            attrInfo.Arguments = attr.ArgumentList.Arguments.Select(m => m.ToString()).ToArray();
                        }
                    }

                    parserChildNodes(enumDeclaration.ChildNodes(), enumInfo); 
                    #endregion
                }
                else if (node.IsKind(SyntaxKind.EnumMemberDeclaration))
                {
                    #region FieldDeclaration
                    var enumMemberDeclaration = (EnumMemberDeclarationSyntax)node;

                    var fieldInfo = new EnumMemberInfo();
                    if (output.Items == null)
                        output.Items = new List<BaseCodeNodeInfo>();
                    ((IList)output.Items).Add(fieldInfo);

                    fieldInfo.Name = enumMemberDeclaration.Identifier.ToString();
                    fieldInfo.Value = enumMemberDeclaration.EqualsValue.Value.ToString();
                    fieldInfo.Caption = GetDocumentationComment(enumMemberDeclaration);

                    foreach (var attrlist in enumMemberDeclaration.AttributeLists)
                    {
                        foreach (var attr in attrlist.Attributes)
                        {
                            var attrInfo = new AttributeInfo();
                            ((IList)(fieldInfo.Attributes ??= new List<AttributeInfo>())).Add(attrInfo);

                            attrInfo.Name = attr.Name.ToString();
                            attrInfo.Arguments = attr.ArgumentList.Arguments.Select(m => m.ToString()).ToArray();
                        }
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="syntaxNode"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        static CaptionInfo GetDocumentationComment(SyntaxNode syntaxNode)
        {
            // 获取节点前面的所有Trivia
            var leadingTrivia = syntaxNode.GetLeadingTrivia();

            // 在Trivia中查找文档注释
            var docCommentTrivia = leadingTrivia.FirstOrDefault(trivia =>
                trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia)
            );

            if (docCommentTrivia.IsKind(SyntaxKind.None))
                return null;

            var comment = docCommentTrivia.ToString().Trim();
            if (comment.Length == 0)
                return null;

            // 返回文档注释的文本
            var content = comment.Split('\n').Select(m => m.Trim()).ToArray();
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].StartsWith("///"))
                {
                    if (content[i].Length > 3)
                    {
                        content[i] = content[i].Substring(3);
                    }
                    else
                    {
                        content[i] = "";
                    }
                }
            }

            comment = string.Join('\n', content);

            return new CSharpCaptionInfo(comment);
        }
    }
}
