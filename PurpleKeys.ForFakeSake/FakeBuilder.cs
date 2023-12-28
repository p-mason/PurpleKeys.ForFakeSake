using System.Linq.Expressions;
using System.Reflection;
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
        var key = typeof(T).GetProperty(propertyName)!.ToString()!;
        var setup = new FakeSetup(_ => true, true, execute, null);
        var foo = new List<FakeSetup> { setup };
        _setups.Add(key, foo);
        
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
        _setups.Add(methodSignature, new List<FakeSetup> { stubSetup });
        
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
        foreach(var stub in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.Name.Equals(functionName) && m.ReturnType == typeof(TReturn)))
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
        foreach(var stub in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.Name.Equals(actionName) && m.ReturnType == typeof(void)))
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
}