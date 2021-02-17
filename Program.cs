//Define conditions
#define CONDITION1
#define CONDITION2

using System;
using System.Diagnostics;
using Xunit;
using Xunit.Sdk;
using System.Reflection;
using System.Linq;
using System.CodeDom;
using System.Linq.Expressions;

namespace Reflection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Using Serializable attribute: Line 44");
            Console.WriteLine("Creating custom attribute: Line 56");
            Console.WriteLine("Conditional Attribute:");

            //Condition1 defined in line 1
            MyMethod();

            //Reading attributes using reflection
            if (Attribute.IsDefined(typeof(Person), typeof(SerializableAttribute))) {
                Console.WriteLine("The Person is using the Serializable attribute");
             }

            //You can retrieve the specific instance of an attribute so that you can look at its properties. 
            ConditionalAttribute conditionalAttribute =
            (ConditionalAttribute)Attribute.GetCustomAttribute(
            typeof(Person),
            typeof(ConditionalAttribute));

            //Will return CONDITION1
            //string condition = conditionalAttribute.ConditionString; 
            //Console.WriteLine(condition);
            
            Console.WriteLine("Using Reflection: Line 92");
            GetObjectType();

            //Using reflection to inspect field/value
            int i = 45;
            object o = i;
            DumpObject(o);

            //using Reflection to execute a method
            ExecuteMethod();

            //Lambda Expressions
            lambda();

            //Expression trees
            Console.WriteLine("Creating a 'Hello World!' app through expression trees!");
            ExpressionTrees();
            
        }

        //Using attributes is a powerful way to add metadata to an application. Attributes can be added to all kinds of types: assemblies, types, methods, parameters, and properties. 
        //At runtime, you can query for the existence of an attribute and its settings and then take appropriate action
        //
        //Serializable attribute 
        //Serialization is the process of translating data strutures into format that can be stored, reconstructed or transmitted later
        [Serializable]
        class Person {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        //Conditional attribute
        //Indicates to compiler that the method call should be ignored if these conditions are true
        [Conditional("CONDITION1"), Conditional("CONDITION2")]
        static void MyMethod(){
            Console.WriteLine("Conditions met successfully!");
        }

        //Custom Attribute
        //Custom attribute class has to derive from System.Attribute 
        //xUnit (a popular unit testing framework) enables you to categorize your unit tests by applying an attribute to them
        public class CategoryAttribute : ITraitAttribute
        {
            public CategoryAttribute(string value)
            { }
        }

        public class UnitTestAttribute : CategoryAttribute
        {
            public UnitTestAttribute()
                : base("Unit Test")
            { }
        }


        //Adding AllowMultiple = true, applying multiple attribute targets and properties to attribute
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true)]
        class CompleteCustomAttribute : Attribute
        {
            public CompleteCustomAttribute(string description)
            {
                Description = description;
            }
            public string Description { get; set; }
        }

        //Using Reflection
        //Reflection enables an application to collect information about itself and act on this information
        //Most basic form of reflection: getting the current type of an object you have
        public static void GetObjectType(){
            int i = 42;
            System.Type type = i.GetType();
            Console.WriteLine("Type: {0}", type);
        }

        //Creating own plugin
        //Create interface that can be found through system
        public interface IPlugin
        {
            string Name { get; }
            string Description { get; }
            bool Load(Program application);
        }

        //You can now inherit from this interface to create a plugin:
        public class MyPlugin : IPlugin
        {
            public string Name
            {
                get { return "MyPlugin"; }
            }

            public string Description
            {
                get { return "My Sample Plugin"; }
            }

            public bool Load(Program application)
            {
                return true;           
            }
        }

        //Using reflection, you can now inspect an assembly and check it for any available plug-ins.
        //The types you get back can then be used to create an instance of the plug-in and use it. The System.Reflection namespace defines the elements you need for reflection
        public static void InspectAssembly() {
            Assembly pluginAssembly = Assembly.Load("assemblyname");

            var plugins = from type in pluginAssembly.GetTypes()
                        where typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface
                        select type;

            foreach (Type pluginType in plugins)
            {
                IPlugin plugin = Activator.CreateInstance(pluginType) as IPlugin;
            }
        }

        //Reflection can also be used to inspect the value of a property or a field
        static void DumpObject(object obj)
        {
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        int count = 1;

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(int))
                {
                    Console.WriteLine("Field value {0}: {1}", count, field.GetValue(obj));
                    count++;
                }
            }
        }

        //Reflection can also be used to execute a method on a type. You can specify the parameters the method needs
        static void ExecuteMethod(){
            int i = 42;
            MethodInfo compareToMethod = i.GetType().GetMethod("CompareTo",
                new Type[] { typeof(int) });
            int result = (int)compareToMethod.Invoke(i, new object[] { 41 });
            Console.WriteLine("Result of method: {0}", result);
        }

        //Lambda Expressions
        //Used to create anonymous functions/methods(=>)
        //Can use built-in expression Func<> and Action<> to add functionality to method
        static void lambda(){
            Func<int, int, int> addFunc = (x, y) => x + y;
            Console.WriteLine("Lambda Func<> addFunc Results: {0}", addFunc(2, 3));
        }

        //Expression trees
        //An expression tree describes code instead of being the code itself. Expression trees are heavily used in LINQ
        //CReating "Hello World" app through expression trees
        static void ExpressionTrees(){
            BlockExpression blockExpr = Expression.Block(
            Expression.Call(
                null,
                typeof(Console).GetMethod("Write", new Type[] { typeof(String) }),
                Expression.Constant("Hello ")
                ),
            Expression.Call(
                null,
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(String) }),
                Expression.Constant("World!")
                )
            );

            Expression.Lambda<Action>(blockExpr).Compile()();
        }
    }
}
