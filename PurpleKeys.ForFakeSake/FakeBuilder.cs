using System.Linq.Expressions;
using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Internal;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake;

/// <summary>
///     Builder for creating Fakes
/// </summary>
/// <typeparam name="T">Build Fake of type.</typeparam>
public class FakeBuilder<T>
    where T : class {

    private readonly List<IInterceptor> _interceptors = new();
    private readonly SetupDictionary _setups = new();

    /// <summary>
    ///     Builds a Fake from the configuration of this Builder.
    /// </summary>
    /// <returns>An instance of type <typeparamref name="T" /></returns>
    public T Build() {
        var proxyGenerator = new ProxyGenerator();
        var interceptor = new FakeInterceptor<T>(_setups);
        var interceptors = _interceptors.Union(new[] { interceptor }).ToArray();

        return typeof(T).IsInterface
            ? proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(ProxyGenerationOptions.Default, interceptors)
            : proxyGenerator.CreateClassProxy<T>(ProxyGenerationOptions.Default, interceptors);
    }

    /// <summary>
    /// Setup a property to execute an action when it's value is set.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="execute"></param>
    /// <returns></returns>
    public FakeBuilder<T> PropertySetExecutes(string propertyName, Action<IDictionary<string, object>> execute) {
        var key = ReflectionUtility.PropertySetSignature<T>(propertyName);
        var setup = new FakeSetup(_ => true, true, execute, null);
        _setups.AddSetup(key, setup);

        return this;
    }

    /// <summary>
    /// Stubs Properties to always return the setup value.
    /// </summary>
    /// <param name="setup">Expression defining property Values</param>
    /// <returns>The builder.</returns>
    public FakeBuilder<T> StubProperties(Expression<Func<T, bool>> setup) {
        VisitPropertiesInExpression(setup);

        return this;
    }

    /// <summary>
    /// Stubs Properties to always return the setup value, allowing access to the current builder.
    /// </summary>
    /// <param name="setup">Expression defining property Values</param>
    /// <returns>The builder.</returns>
    public FakeBuilder<T> StubProperties(Expression<Func<T, FakeBuilder<T>, bool>> setup) {
        VisitPropertiesInExpression(setup);

        return this;
    }

    /// <summary>
    /// Stubs an Action to execute a function when called.
    /// </summary>
    /// <param name="methodSignature">The method signature <see cref="System.Reflection.MemberInfo.ToString"/></param>
    /// <param name="stubAction">Action to execute.</param>
    /// <returns>The builder.</returns>
    public FakeBuilder<T> StubAction(
        string methodSignature,
        Expression<Action<IDictionary<string, object>>> stubAction) 
    {
        var stubSetup = new FakeSetup(_ => true, false, stubAction.Compile(), null);
        _setups.AddSetup(methodSignature, stubSetup);

        return this;
    }

    /// <summary>
    /// Stubs a Function to execute and return a function when called.
    /// </summary>
    /// <param name="methodSignature">The method signature <see cref="System.Reflection.MemberInfo.ToString"/></param>
    /// <param name="stubFunction">Function to execute.</param>
    /// <typeparam name="TReturn">The return type of the function</typeparam>
    /// <returns>The builder.</returns>
    public FakeBuilder<T> StubFunction<TReturn>(
        string methodSignature,
        Expression<Func<IDictionary<string, object>, TReturn>> stubFunction) 
    {
        Expression<Func<IDictionary<string, object>, object>> expression;
        if (typeof(T) == typeof(object)) {
            var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
            expression = Expression.Lambda<Func<IDictionary<string, object>, object>>(stubFunction, parameters);
        }
        else {
            var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
            var body = Expression.Convert(Expression.Invoke(stubFunction, parameters), typeof(object));
            expression = Expression.Lambda<Func<IDictionary<string, object>, object>>(body, parameters);
        }

        var stubSetup = new FakeSetup(_ => true, true, null, expression.Compile());
        _setups.AddSetup(methodSignature, stubSetup);

        return this;
    }

    /// <summary>
    /// Stub all overloads with the return type <typeparamref name="TReturn"/> to execute and return the same function.
    /// </summary>
    /// <param name="functionName">Name of the function to fake.</param>
    /// <param name="func">The function to execute.</param>
    /// <typeparam name="TReturn">The return type of the function being faked.</typeparam>
    /// <returns>The builder.</returns>
    public FakeBuilder<T> AllFunctionOverloads<TReturn>(
        string functionName,
        Expression<Func<IDictionary<string, object>, TReturn>> func) {
        foreach (var stub in ReflectionUtility.GetFunctionOverloads<T, TReturn>(functionName)) {
            StubFunction(stub.ToString()!, func);
        }

        return this;
    }

    /// <summary>
    /// Setup all methods with no setup to throw an <see cref="System.NotImplementedException"/> when called 
    /// </summary>
    /// <returns>The Bulider</returns>
    /// <exception cref="NotImplementedException">Causes invocations to throw an
    /// instance of <see cref="System.NotImplementedException"/> </exception>
    public FakeBuilder<T> MethodsWithNoSetupThrowNotImplementedException() {
        _setups.Add(FakeInterceptor<T>.MethodsNoSetup, new List<FakeSetup> {
            new(_ => true, false, _ => throw new NotImplementedException("Fake Member has not been setup"), null)
        });

        return this;
    }

    /// <summary>
    /// Causes all actions with no previous matching setup to execute a method.
    /// </summary>
    /// <param name="actionName">Action to setup</param>
    /// <param name="action">Action to execute</param>
    /// <returns>The builder.</returns>
    public FakeBuilder<T> AllActionOverloads(
        string actionName,
        Expression<Action<IDictionary<string, object>>> action) {
        foreach (var stub in ReflectionUtility.GetActionOverloads<T>(actionName)) {
            StubAction(stub.ToString()!, action);
        }

        return this;
    }
    
    /// <summary>
    /// Adds an interceptor to the Builder.
    /// </summary>
    /// <param name="interceptor">An interceptor.</param>
    /// <returns>The builder.</returns>
    public FakeBuilder<T> WithInterceptor(IInterceptor interceptor) {
        _interceptors.Add(interceptor);
        return this;
    }

    public FakeBuilder<T> FakeMethod(string memberName, FakeSetup setup) {
        foreach (var member in ReflectionUtility.GetMethodOverloads<T>(memberName)) {
            _setups.AddSetup(member.ToString()!, setup);
        }

        return this;
    }

    public FakeBuilder<T> FakeMethodDefault<TReturn>(
        string memberName,
        Func<IDictionary<string, object>, TReturn> defaultReturn) {
        foreach (var member in ReflectionUtility.GetFunctionOverloads<T, TReturn>(memberName)) {
            StubFunction(member.ToString()!, args => defaultReturn(args));
        }

        return this;
    }

    /// <summary>
    /// Fake a property to return the results of a <see cref="FakeSetup"/>.
    /// </summary>
    /// <param name="propertyName">Property name to fake</param>
    /// <param name="fakeSetup">Setup to use</param>
    /// <returns>The builder.</returns>
    public FakeBuilder<T> FakeProperty(string propertyName, FakeSetup fakeSetup) {
        foreach (var member in ReflectionUtility.GetProperties<T>(propertyName)) {
            StubProperty(member.Name, fakeSetup);
        }

        return this;
    }

    private void VisitPropertiesInExpression(Expression expression) {
        var visitor = new PropertySetupVisitor<T>();
        visitor.Visit(expression);

        foreach (var prop in visitor.PropertySetups) {
            _setups.AddSetups(prop.Key, prop.Value);
        }
    }

    private void StubProperty(string propertyName, FakeSetup fakeSetup) {
        foreach (var prop in ReflectionUtility.GetProperties<T>(propertyName)) {
            string key;
            if (fakeSetup is PropertyGetFakeSetup && prop.CanRead) {
                key = prop.GetMethod!.ToString()!;
            }
            else if (fakeSetup is PropertySetFakeSetup && prop.CanWrite) {
                key = prop.SetMethod!.ToString()!;
            }
            else {
                throw new Exception();
            }

            _setups.AddSetup(key, fakeSetup);
        }
    }
}
