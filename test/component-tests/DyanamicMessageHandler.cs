using System;
using System.Reflection;
using System.Reflection.Emit;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Xunit;

namespace component_tests
{
    public class DynamicMessageHandlerTests
    {
        [Fact(Skip = "Unable to get this working due to lack of support for CreateMethodBody, parking for now")]
        public void Test()
        {
            var typeBuilder = GetTypeBuilder();
            typeBuilder.AddInterfaceImplementation(typeof(ICapSubscribe));
            var method = typeBuilder.DefineMethod("ProcessMessage", MethodAttributes.Public);

            // Unable to create method body
            // It looks not really to be supported in dotnet core: https://github.com/dotnet/runtime/issues/15704
            // This library might help: https://github.com/Lokad/ILPack

            // Alternative, could be to fork CAP and not use attributes to determain the name but extend ICapSubscribe (or something)
            // https://github.com/dotnetcore/CAP/blob/0c68fa4f19f514b4b6f7668a182d4d863579afe5/src/DotNetCore.CAP/Internal/IConsumerServiceSelector.Default.cs#L121
            //var d = method.CreateMethodBody(new byte[] {}, 100);

            var attributeTypeInfo = typeof(CapSubscribeAttribute).GetConstructors();
            var topicAttribute = new CustomAttributeBuilder(attributeTypeInfo[0], new object[] { "test", false });

            method.SetCustomAttribute(topicAttribute);
            var type = typeBuilder.CreateType();

            Assert.NotNull(type);
        }

        private static TypeBuilder GetTypeBuilder()
        {
            const string typeSignature = "DynamicMessageHandler";
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(typeSignature), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            return moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
        }
    }
}