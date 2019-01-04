// https://github.com/KirillOsenkov/RoslynQuoter

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Proxy.Unit;
using DotEasy.Rpc.Core.Runtime.Client;
using DotEasy.Rpc.Core.Runtime.Communally.Convertibles;
using DotEasy.Rpc.Core.Runtime.Communally.IdGenerator;
using DotEasy.Rpc.Core.Runtime.Communally.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Core.Proxy.Impl
{
    public class ServiceProxyGenerater : IServiceProxyGenerater
    {
        private readonly IServiceIdGenerator _serviceIdGenerator;
        private readonly ILogger<ServiceProxyGenerater> _logger;

        public ServiceProxyGenerater(IServiceIdGenerator serviceIdGenerator, ILogger<ServiceProxyGenerater> logger)
        {
            _serviceIdGenerator = serviceIdGenerator;
            _logger = logger;
        }

        public IEnumerable<Type> GenerateProxys(IEnumerable<Type> interfaceTypes)
        {
            var assembles = DependencyContext.Default.RuntimeLibraries
                .SelectMany(i => i.GetDefaultAssemblyNames(DependencyContext.Default)
                    .Select(z => Assembly.Load(new AssemblyName(z.Name))));

            assembles = assembles.Where(i => i.IsDynamic == false).ToArray();

            var trees = interfaceTypes.Select(GenerateProxyTree).ToList();
            var stream = CompilationUnits.CompileClientProxy(trees,
                assembles
                    .Select(a => MetadataReference.CreateFromFile(a.Location))
                    .Concat(new[]
                    {
                        MetadataReference.CreateFromFile(typeof(Task).GetTypeInfo().Assembly.Location)
                    }),
                _logger);

            if (stream == null)
            {
                throw new ArgumentException("没有生成任何客户端代码", nameof(stream));
            }

            using (stream)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
                return assembly.GetExportedTypes();
            }
        }

        public SyntaxTree GenerateProxyTree(Type interfaceType)
        {
            var className = interfaceType.Name.StartsWith("I") ? interfaceType.Name.Substring(1) : interfaceType.Name;
            className += "ClientProxy";

            var members = new List<MemberDeclarationSyntax>
            {
                GetConstructorDeclaration(className)
            };

            var interf = interfaceType.GetInterfaces()[0];
            var mthods = interfaceType.GetMethods();
            if (interf.FullName.Contains("IDisposable"))
            {
                var m = interf.GetMethods()[0];
                var mm = mthods.ToList();
                mm.Add(m);
                mthods = mm.ToArray();
            }

            members.AddRange(GenerateMethodDeclarations(mthods));

            return SyntaxFactory.CompilationUnit().WithUsings(GetUsings()).WithMembers(
                SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                    SyntaxFactory.NamespaceDeclaration(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName("Rpc"),
                                SyntaxFactory.IdentifierName("Common")),
                            SyntaxFactory.IdentifierName("ClientProxys"))).WithMembers(
                        SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                            SyntaxFactory.ClassDeclaration(className).WithModifiers(
                                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword))).WithBaseList(
                                SyntaxFactory.BaseList(
                                    SyntaxFactory.SeparatedList<BaseTypeSyntax>(new SyntaxNodeOrToken[]
                                    {
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.IdentifierName("ServiceProxyBase")),
                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                        SyntaxFactory.SimpleBaseType(GetQualifiedNameSyntax(interfaceType))
                                    }))).WithMembers(
                                SyntaxFactory.List(members)))))).NormalizeWhitespace().SyntaxTree;
        }

        private static QualifiedNameSyntax GetQualifiedNameSyntax(Type type)
        {
            var fullName = type.Namespace + "." + type.Name;
            return GetQualifiedNameSyntax(fullName);
        }

        private static QualifiedNameSyntax GetQualifiedNameSyntax(string fullName)
        {
            return GetQualifiedNameSyntax(fullName.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries));
        }

        private static QualifiedNameSyntax GetQualifiedNameSyntax(IReadOnlyCollection<string> names)
        {
            var ids = names.Select(SyntaxFactory.IdentifierName).ToArray();

            var index = 0;
            QualifiedNameSyntax left = null;
            while (index + 1 < names.Count)
            {
                left = left == null
                    ? SyntaxFactory.QualifiedName(ids[index], ids[index + 1])
                    : SyntaxFactory.QualifiedName(left, ids[index + 1]);
                index++;
            }

            return left;
        }

        private static SyntaxList<UsingDirectiveSyntax> GetUsings()
        {
            return SyntaxFactory.List(
                new[]
                {
                    SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax("System.Threading.Tasks")),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax("System.Collections.Generic")),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax(typeof(ITypeConvertibleService).Namespace)),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax(typeof(IRemoteInvokeService).Namespace)),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax(typeof(ISerializer<>).Namespace)),
                    SyntaxFactory.UsingDirective(GetQualifiedNameSyntax(typeof(ServiceProxyBase).Namespace))
                });
        }

        private static ConstructorDeclarationSyntax GetConstructorDeclaration(string className)
        {
            return SyntaxFactory.ConstructorDeclaration(
                SyntaxFactory.Identifier(className)).WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword))).WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
                    {
                        SyntaxFactory.Parameter(
                            SyntaxFactory.Identifier("remoteInvokeService")).WithType(
                            SyntaxFactory.IdentifierName("IRemoteInvokeService")),
                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                        SyntaxFactory.Parameter(
                            SyntaxFactory.Identifier("typeConvertibleService")).WithType(
                            SyntaxFactory.IdentifierName("ITypeConvertibleService"))
                    }))).WithInitializer(
                SyntaxFactory.ConstructorInitializer(
                    SyntaxKind.BaseConstructorInitializer,
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                        {
                            SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName("remoteInvokeService")),
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                            SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName("typeConvertibleService"))
                        })))).WithBody(
                SyntaxFactory.Block());
        }

        private IEnumerable<MemberDeclarationSyntax> GenerateMethodDeclarations(IEnumerable<MethodInfo> methods)
        {
            var array = methods.ToArray();
            return array.Select(GenerateMethodDeclaration).ToArray();
        }

        private static TypeSyntax GetTypeSyntax(Type type)
        {
            if (type == null)
                return null;

            if (!type.GetTypeInfo().IsGenericType)
                return GetQualifiedNameSyntax(type.FullName);

            var list = new List<SyntaxNodeOrToken>();
            foreach (var genericTypeArgument in type.GenericTypeArguments)
            {
                list.Add(genericTypeArgument.GetTypeInfo().IsGenericType
                    ? GetTypeSyntax(genericTypeArgument)
                    : GetQualifiedNameSyntax(genericTypeArgument.FullName));
                list.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
            }

            var array = list.Take(list.Count - 1).ToArray();
            var typeArgumentListSyntax = SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(array));
            return SyntaxFactory.GenericName(type.Name.Substring(0, type.Name.IndexOf('`')))
                .WithTypeArgumentList(typeArgumentListSyntax);
        }

        private MemberDeclarationSyntax GenerateMethodDeclaration(MethodInfo method)
        {
            var serviceId = _serviceIdGenerator.GenerateServiceId(method);
            TypeSyntax returnDeclaration = GetTypeSyntax(method.ReturnType);

            var parameterList = new List<SyntaxNodeOrToken>();
            var parameterDeclarationList = new List<SyntaxNodeOrToken>();

            foreach (var parameter in method.GetParameters())
            {
                parameterDeclarationList.Add(SyntaxFactory.Parameter(
                        SyntaxFactory.Identifier(parameter.Name))
                    .WithType(GetQualifiedNameSyntax(parameter.ParameterType)));
                parameterDeclarationList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));

                parameterList.Add(
                    SyntaxFactory.InitializerExpression(SyntaxKind.ComplexElementInitializerExpression,
                        SyntaxFactory.SeparatedList<ExpressionSyntax>(new SyntaxNodeOrToken[]
                        {
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(parameter.Name)),
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                            SyntaxFactory.IdentifierName(parameter.Name)
                        })));
                parameterList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
            }

            if (parameterList.Any())
            {
                parameterList.RemoveAt(parameterList.Count - 1);
                parameterDeclarationList.RemoveAt(parameterDeclarationList.Count - 1);
            }

            MethodDeclarationSyntax declaration;
            if (method.ToString().Contains("Task"))
            {
                declaration = SyntaxFactory.MethodDeclaration(
                    returnDeclaration, SyntaxFactory.Identifier(method.Name)
                ).WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                ).WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(parameterDeclarationList)));
            }
            else
            {
                if (method.ToString().Contains("Void"))
                {
                    declaration = SyntaxFactory.MethodDeclaration(
                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                        SyntaxFactory.Identifier("Dispose")
                    ).WithModifiers(
                        SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
//                                    .WithBody(SyntaxFactory.Block())));
                }
                else
                {
                    declaration = SyntaxFactory.MethodDeclaration(
                        returnDeclaration,
                        SyntaxFactory.Identifier(method.Name)
                    ).WithModifiers(
                        SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                        )).WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(parameterDeclarationList)));
                }
            }

            ExpressionSyntax expressionSyntax;
            StatementSyntax statementSyntax = null;

            if (method.ReturnType.ToString().Contains("Task"))
            {
                expressionSyntax = SyntaxFactory.GenericName(SyntaxFactory.Identifier("InvokeAsync"))
                    .WithTypeArgumentList(((GenericNameSyntax) returnDeclaration).TypeArgumentList);
            }
            else
            {
                var list = new List<SyntaxNodeOrToken> {GetQualifiedNameSyntax(method.ReturnType.FullName)};
                var typeArgumentListSyntax = SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(list.ToArray()));
                expressionSyntax = SyntaxFactory.GenericName("Invoke").WithTypeArgumentList(typeArgumentListSyntax);
            }

            if (method.ReturnType.ToString().Contains("Task"))
            {
                expressionSyntax = SyntaxFactory.AwaitExpression(
                    SyntaxFactory.InvocationExpression(expressionSyntax).WithArgumentList(
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                        {
                            SyntaxFactory.Argument(
                                SyntaxFactory.ObjectCreationExpression(
                                    SyntaxFactory.GenericName(SyntaxFactory.Identifier("Dictionary")).WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]
                                            {
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                SyntaxFactory.PredefinedType(
                                                    SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
                                            })))).WithInitializer(
                                    SyntaxFactory.InitializerExpression(SyntaxKind.CollectionInitializerExpression,
                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(parameterList)))),
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                            SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(serviceId)))
                        }))));
            }
            else
            {
                expressionSyntax = SyntaxFactory.InvocationExpression(expressionSyntax).WithArgumentList(
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                        {
                            SyntaxFactory.Argument(
                                SyntaxFactory.ObjectCreationExpression(
                                        SyntaxFactory.GenericName(SyntaxFactory.Identifier("Dictionary"))
                                            .WithTypeArgumentList(
                                                SyntaxFactory.TypeArgumentList(
                                                    SyntaxFactory.SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]
                                                        {
                                                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
                                                        }
                                                    )
                                                )
                                            )
                                    )
                                    .WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.CollectionInitializerExpression,
                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(parameterList))
                                    )
                            ),
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                            SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(serviceId)))
                        }
                    )));
            }

            if (method.ReturnType != typeof(Task))
            {
                if (!method.ToString().Contains("Void"))
                {
                    statementSyntax = SyntaxFactory.ReturnStatement(expressionSyntax);
                }
            }
            else
            {
                statementSyntax = SyntaxFactory.ExpressionStatement(expressionSyntax);
            }

            declaration = declaration.WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.SingletonList(statementSyntax)));

            return declaration;
        }
    }
}