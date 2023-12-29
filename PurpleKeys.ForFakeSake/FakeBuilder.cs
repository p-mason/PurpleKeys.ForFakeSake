using System.Linq.Expressions;
using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Internal;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake;

public class FakeBuilder<T>
    where T : class
{
    private readonly Dictionary<string, List<FakeSetup>> _setups = new();
    private readonly List<IInterceptor> _interceptors = new();
    
    public T Build()
    {
        var proxyGenerator = new ProxyGenerator();
        var interceptor = new FakeInterceptor<T>(_setups);
        var interceptors = _interceptors.Union(new[] { interceptor }).ToArray();
        
        return typeof(T).IsInterface 
            ? proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(ProxyGenerationOptions.Default, interceptors) 
            : proxyGenerator.CreateClassProxy<T>(ProxyGenerationOptions.Default, interceptors);
    }

    public FakeBuilder<T> PropertyAlwaysReturns(Expression<Func<T, bool>> propertyExpression)
    {
        if (propertyExpression.NodeType != ExpressionType.Lambda)
        {
            throw new InvalidOperationException();
        }
        
        AddPropertyReadSetupsFromExpressions(propertyExpression);

        return this;
    }

    public FakeBuilder<T> PropertySetAlways(string propertyName, Action<IDictionary<string, object>> execute)
    {
        var key = ReflectionUtility.PropertySetSignature<T>(propertyName);
        var setup = new FakeSetup(_ => true, true, execute, null);
        _setups.Add(key, new List<FakeSetup> { setup });
        
        return this;
    }

    public FakeBuilder<T> StubProperties(Expression<Func<T, bool>> setup)
    {
        AddPropertyReadSetupsFromExpressions(setup);

        return this;
    }
    
    public FakeBuilder<T> StubProperties(Expression<Func<T, FakeBuilder<T>, bool>> setup)
    {
        AddPropertyReadSetupsFromExpressions(setup);

        return this;
    }

    public FakeBuilder<T> StubMethod(string methodSignature, Expression<Action<IDictionary<string, object>>> stubAction)
    {
        var stubSetup = new FakeSetup((_) => true, false, stubAction.Compile(), null);
        _setups.Add(methodSignature, new List<FakeSetup> { stubSetup });
        
        return this;
    }
    
    public FakeBuilder<T> StubMethod<TReturn>(string methodSignature, Expression<Func<IDictionary<string, object>, TReturn>> stubFunction)
    {
        Expression<Func<IDictionary<string, object>, object>> expression;
        if (typeof(T) == typeof(object))
        {
            var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
            expression = Expression.Lambda<Func<IDictionary<string, object>, object>>(stubFunction, new[] { parameters });
        }
        else 
        {
            var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
            var body = Expression.Convert(Expression.Invoke(stubFunction, parameters), typeof(object));
            expression = Expression.Lambda<Func<IDictionary<string, object>, object>>(body, new[] { parameters });
        }
        
        var stubSetup = new FakeSetup((_) => true, true, null, expression.Compile());
        if (!_setups.TryGetValue(methodSignature, out var memberSetups))
        {
            memberSetups = new List<FakeSetup>();
            _setups.Add(methodSignature, memberSetups);
        }
        memberSetups.Add(stubSetup);
        
        return this;
    }

    private void AddPropertyReadSetupsFromExpressions(Expression<Func<T, FakeBuilder<T>, bool>> setup)
    {
        var visitor = new PropertySetupVisitor<T>();
        visitor.Visit(setup);
        
        foreach (var prop in visitor.PropertySetups)
        {
            if (_setups.TryGetValue(prop.Key, out var existing))
            {
                existing.Add(prop.Value.First());
            }
            else
            {
                _setups.Add(prop.Key, prop.Value);
            }
        }
    }

    private void AddPropertyReadSetupsFromExpressions(Expression<Func<T, bool>> setup)
    {
        var visitor = new PropertySetupVisitor<T>();
        visitor.Visit(setup);
        
        foreach (var prop in visitor.PropertySetups)
        {
            if (_setups.TryGetValue(prop.Key, out var existing))
            {
                existing.Add(prop.Value.First());
            }
            else
            {
                _setups.Add(prop.Key, prop.Value);
            }
        }
    }

    public FakeBuilder<T> AllMethodOverloads<TReturn>(string functionName, Expression<Func<IDictionary<string, object>, TReturn>> func)
    {
        foreach(var stub in ReflectionUtility.GetFunctionOverloads<T, TReturn>(functionName))
        {
            StubMethod(stub.ToString()!, func);    
        }
        return this;
    }

    public FakeBuilder<T> MethodsWithNoSetupThrowNotImplementedException()
    {
        _setups.Add(FakeInterceptor<T>.MethodsNoSetup, new List<FakeSetup>
        {
            new (_ => true, false, _ => throw new NotImplementedException(), null)
        });
        
        return this;
    }
    
    public FakeBuilder<T> AllMethodOverloads(string actionName, Expression<Action<IDictionary<string, object>>> action)
    {
        foreach(var stub in ReflectionUtility.GetActionOverloads<T>(actionName))
        {
            StubMethod(stub.ToString()!, action);    
        }
        
        return this;
    }

    public FakeBuilder<T> WithSpy(SpyInterceptor spy)
    {
        _interceptors.Add(spy);       
        return this;
    }

    public FakeBuilder<T> FakeMethod(string memberName, FakeSetup setup)
    {
        foreach (var member in ReflectionUtility.GetMethodOverloads<T>(memberName))
        {
            if (!_setups.TryGetValue(member.ToString()!, out var memberSetups))
            {
                memberSetups = new List<FakeSetup>();
                _setups.Add(member.ToString()!, memberSetups);
            }
            memberSetups.Add(setup);    
        }
        
        return this;
    }

    public FakeBuilder<T> FakeMethodDefault<TReturn>(
        string memberName, 
        Func<IDictionary<string, object>, TReturn> defaultReturn)
    {
        foreach (var member in ReflectionUtility.GetFunctionOverloads<T, TReturn>(memberName))
        {
            StubMethod(member.ToString(), args => defaultReturn(args));
        }

        return this;
    }

    public FakeBuilder<T> FakeProperty(string propertyName, FakeSetup fakeSetup)
    {
        foreach (var member in ReflectionUtility.GetProperties<T>(propertyName))
        { 
            StubProperty(member.Name, fakeSetup);
        }
        return this;
    }

    private void StubProperty(string propertyName, FakeSetup fakeSetup)
    {
        foreach (var prop in ReflectionUtility.GetProperties<T>(propertyName))
        {
            string key;
            if (fakeSetup is PropertyGetFakeSetup && prop.CanRead)
            {
                key = prop.GetMethod!.ToString()!;
            }
            else if (fakeSetup is PropertySetFakeSetup && prop.CanWrite)
            {
                key = prop.SetMethod!.ToString()!;    
            }
            else
            {
                throw new Exception();
            }
            
            if (!_setups.TryGetValue(key, out var memberSetups))
            {
                memberSetups = new List<FakeSetup>();
                _setups.Add(key, memberSetups);
            }
            memberSetups.Add(fakeSetup); 
        }
    }
}