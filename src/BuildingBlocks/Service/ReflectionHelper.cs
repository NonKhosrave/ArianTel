using System;
using System.Linq;
using System.Reflection;

namespace BuildingBlocks.Service;

public sealed class ReflectionHelper
{
    private static readonly Type FunctionType = typeof(Func<,>);
    private static readonly Type ReflectionHelperType = typeof(ReflectionHelper);
    private static readonly LazyConcurrent<Type, ReflectionHelper[]> Cache = new();

    private static readonly MethodInfo CallInnerDelegateMethod =
        ReflectionHelperType.GetMethod(nameof(CallInnerDelegate), BindingFlags.Public | BindingFlags.Static);

    public PropertyInfo Property { get; set; }
    public Func<object, object> Getter { get; set; }

    // Called via reflection.
    private static Func<object, object> CallInnerDelegate<TClass, TResult>(
        Func<TClass, TResult> deleg)
    {
        return instance => deleg((TClass)instance);
    }

    public static ReflectionHelper[] GetProperties(Type type)
    {
        return Cache
            .GetOrAdd(type, _ => type
                .GetProperties()
                .Select(property =>
                {
                    var getMethod = property.GetMethod;
                    var declaringClass = property.DeclaringType;
                    var typeOfResult = property.PropertyType;
                    // Func<Type, TResult>
                    var getMethodDelegateType = FunctionType.MakeGenericType(declaringClass, typeOfResult);
                    // c => c.Data
                    var getMethodDelegate = getMethod.CreateDelegate(getMethodDelegateType);
                    // CallInnerDelegate<Type, TResult>
                    var callInnerGenericMethodWithTypes = CallInnerDelegateMethod
                        .MakeGenericMethod(declaringClass, typeOfResult);
                    // Func<object, object>
                    var result =
                        (Func<object, object>)callInnerGenericMethodWithTypes.Invoke(null, new[] { getMethodDelegate });
                    //var setter = property.GetSetMethod();
                    return new ReflectionHelper
                    {
                        Property = property,
                        Getter = result
                        //Setter = setter
                    };
                })
                .ToArray());
    }
}
