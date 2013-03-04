using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DontTryThisEmit
{
    public interface ITwitterService
    {
        void Tweet(string message);
    }

    public class MyTwitterService : ITwitterService
    {
        public void Tweet(string message)
        {
            Console.WriteLine("Tweeting: {0}", message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var type = CreateMyTwitterServiceProxyType();

            var dynamicProxy = (ITwitterService)Activator.CreateInstance(
                type, new object[] { new MyTwitterService() });

            dynamicProxy.Tweet("This is bananas, B-A-N-A-N-A-S");
        }

        static Type CreateMyTwitterServiceProxyType()
        {
            // assembly
            var assemblyName = new AssemblyName("MyProxies");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                    assemblyName,
                    AssemblyBuilderAccess.Run);

            // module
            var modBuilder = assemblyBuilder.DefineDynamicModule("MyProxies");

            // public class MyTwitterServiceProxy : ITwitterService
            // {
            var typeBuilder = modBuilder.DefineType(
                "MyTwitterServiceProxy",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object),
                new[] { typeof(ITwitterService) });

            //      private MyTwitterService _realObject;
            var fieldBuilder = typeBuilder.DefineField(
                "_realObject",
                typeof(MyTwitterService),
                FieldAttributes.Private);

            //      public MyTwitterServiceProxy(MyTwitterService arg1)
            //      {
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                new[] { typeof(MyTwitterService) });
            //  _realObject = arg1
            var contructorIl = constructorBuilder.GetILGenerator();
            contructorIl.Emit(OpCodes.Ldarg_0);
            contructorIl.Emit(OpCodes.Ldarg_1);
            contructorIl.Emit(OpCodes.Stfld, fieldBuilder);
            contructorIl.Emit(OpCodes.Ret);
            //      }


            //      public void Tweet(string arg1)
            //      {
            var methodBuilder = typeBuilder.DefineMethod("Tweet",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(void),
                new[] { typeof(string) });
            typeBuilder.DefineMethodOverride(methodBuilder,
                typeof(ITwitterService).GetMethod("Tweet"));
            var tweetIl = methodBuilder.GetILGenerator();

            //          Console.WriteLine("Hello before!");
            tweetIl.Emit(OpCodes.Ldstr, "Hello before!");
            tweetIl.Emit(OpCodes.Call, typeof(Console)
                .GetMethod("WriteLine", new[] { typeof(string) }));

            //          _realObject.Tweet(arg1);
            tweetIl.Emit(OpCodes.Ldarg_0);
            tweetIl.Emit(OpCodes.Ldfld, fieldBuilder);
            tweetIl.Emit(OpCodes.Ldarg_1);
            tweetIl.Emit(OpCodes.Call,
                fieldBuilder.FieldType.GetMethod("Tweet"));

            //          Console.WriteLine("Hello after!");
            tweetIl.Emit(OpCodes.Ldstr, "Hello after!");
            tweetIl.Emit(OpCodes.Call, typeof(Console)
                .GetMethod("WriteLine", new[] { typeof(string) }));
            tweetIl.Emit(OpCodes.Ret);
            //      }


            // }

            return typeBuilder.CreateType();
        }
    }
}

