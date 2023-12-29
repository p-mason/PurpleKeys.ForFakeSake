using System.Reflection;
using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Internal;

namespace PurpleKeys.ForFakeSake;

internal class FakeInterceptor<T> : IInterceptor
{
    public const string MethodsNoSetup = "*Methods*NoSetup";
    private readonly IDictionary<string, List<FakeSetup>> _setups;
    private readonly Dictionary<string, PropertyInterceptWrapper> _instanceProperties;

    public FakeInterceptor()
        : this(new Dictionary<string, List<FakeSetup>>())
    {
    }

    public FakeInterceptor(IDictionary<string, List<FakeSetup>> setups)
    {
        _instanceProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, PropertyInterceptWrapper.Automatic);
        _setups = setups;
    }

    public void Intercept(IInvocation invocation)
    {
        // ReSharper disable once HeapView.ClosureAllocation
        var argsDictionary = invocation.Method
            .GetParameters()
            .Zip(invocation.Arguments)
            .ToDictionary(
                k => k.First.Name ?? throw new NotSupportedException("Parameters Must Have a Name."), 
                v => v.Second);
        
        if (invocation.Method.IsSpecialName && invocation.Method.IsHideBySig)
        {
            var propertyName = invocation.Method.Name.Substring(4);
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property != null)
            {

                if (_setups.TryGetValue(invocation.Method.ToString()!, out var setup))
                {
                    var matchedSetup = setup.FirstOrDefault(s => s.MeetsCondition(argsDictionary));
                    if (property.CanRead && property.GetMethod == invocation.Method)
                    {
                        if (matchedSetup != null)
                        {
                            invocation.ReturnValue = matchedSetup.Func(argsDictionary);
                            return;
                        }
                    }

                    if (property.CanWrite && property.SetMethod == invocation.Method)
                    {
                        if (matchedSetup != null)
                        {
                            matchedSetup.Action(argsDictionary);
                            return;
                        }
                    }
                }
                else
                {
                    if (property.CanRead && property.GetMethod == invocation.Method)
                    {
                        if (_instanceProperties.TryGetValue(propertyName, out var instanceProperty))
                        {
                            invocation.ReturnValue = instanceProperty.Get();
                            return;
                        }
                    }
                    if (property.CanWrite && property.SetMethod == invocation.Method)
                    {
                        if (_instanceProperties.TryGetValue(propertyName, out var instanceProperty))
                        {
                            instanceProperty.Set(invocation.Arguments.First());
                            return;
                        }
                    }
                }
            }
        }

        if (_setups.TryGetValue(invocation.Method.ToString(), out var methodSetups))
        {
            var use = methodSetups.FirstOrDefault(s => s.MeetsCondition(argsDictionary));
            if (use != null)
            {
                if (use.HasReturnValue)
                {
                    invocation.ReturnValue = use.Func(argsDictionary);
                }
                else
                {
                    use.Action(argsDictionary);
                }

                return;
            }
        }
        
        if (_setups.TryGetValue(MethodsNoSetup, out var defaultAction))
        {
            defaultAction.Single().Action(argsDictionary);
        }
        
        if (invocation.Method.ReturnType != typeof(void))
        {
            invocation.ReturnValue = invocation.Method.ReturnType.IsValueType
                ? Activator.CreateInstance(invocation.Method.ReturnType)
                : null;
        }
    }
}