using System.Linq.Expressions;
// ReSharper disable HeapView.ClosureAllocation

namespace PurpleKeys.ForFakeSake.UnitTest;

public class FakingInterfaceMethods
{
    [Fact]
    public void GivenEmptyFake_FunctionsReturnDefaultValues()
    {
        var fake = Fake<ITestInterface>.Empty();
        
        Assert.IsAssignableFrom<ITestInterface>(fake);
        Assert.Null(fake.ReferenceFunc());
        Assert.Equal(0, fake.ValueFunc());
    }

    [Fact]
    public void GivenFakeBuilder_AllMethodOverloads_ReturnsExecutionOfSetup()
    {
        var count = 1;
        var fake = Fake<ITestInterface>.Builder()
            .AllMethodOverloads("ReferenceFunc", (_) => count.ToString())
            .Build();
        
        Assert.Equal("1", fake.ReferenceFunc());
        count = 2;
        Assert.Equal("2", fake.ReferenceFunc());
    }

    [Fact]
    public void GivenFakeBuilder_AllMethodOverloads_AllFunctionParametersAreAvailable()
    {
        var fake = Fake<ITestInterface>.Builder()
            .AllMethodOverloads("OverloadedFunction",
                (args) => string.Join('\n', args.Select(a => $"{a.Key}:{a.Value}")))
            .Build();
        
        var result = fake.OverloadedFunction("Arg1", 2);

        var expected = "arg1:Arg1\narg2:2"; 
        
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GivenFakeBuilder_AllMethodOverloads_AllActionParametersAreAvailable()
    {
        var result = ""; 
        var action = new Action<IDictionary<string, object?>>(a => result = string.Join('\n', a.Select(a => $"{a.Key}:{a.Value}")));
        
        var fake = Fake<ITestInterface>.Builder()
            .AllMethodOverloads("OverloadedAction", _ => action(_))
            .Build();
        
        fake.OverloadedAction("Arg1", 2);

        var expected = "arg1:Arg1\narg2:2"; 
        
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GivenFakeBuilder_AllMethodOverloads_AppliesToAllMethodsWithMatchingReturnType()
    {
        var fake = Fake<ITestInterface>.Builder()
            .AllMethodOverloads("OverloadedFunction", (_) => "StubValue")
            .AllMethodOverloads("OverloadedFunction", (_) => 123)
            .Build();
        
        
        Assert.Equal(123, fake.OverloadedFunction(1));
        Assert.Equal(123, fake.OverloadedFunction("A"));
        
        Assert.Equal("StubValue", fake.OverloadedFunction());
        Assert.Equal("StubValue", fake.OverloadedFunction(1, 1));
        Assert.Equal("StubValue", fake.OverloadedFunction("A", 2));
    }

    [Fact]
    public void GivenFakeBuilder_AllMethodOverloads_FunctionsCanThrowException()
    {
        var fake = Fake<ITestInterface>.Builder()
            .AllMethodOverloads("ValueFunc", FakeFunc<int>.Throws(new ArgumentException("args")))
            .Build();
        
        Assert.Throws<ArgumentException>(() => fake.ValueFunc());
    }
    
    [Fact]
    public void GivenFakeBuilder_AllMethodOverloads_ActionsCanThrowException()
    {
        var fake = Fake<ITestInterface>.Builder()
            .AllMethodOverloads("Action", FakeAction.Throws(new ArgumentException("args")))
            .Build();

        Assert.Throws<ArgumentException>(() => fake.Action());
    }

    [Fact]
    public void GivenFakeBuilder_MethodsWithNoSetupThrowNotImplementedException_CausesMethodsWithNoSetupToError()
    {
        var fake = Fake<ITestInterface>.Builder()
            .AllMethodOverloads("ReferenceFunc", (_) => "TestValue")
            .MethodsWithNoSetupThrowNotImplementedException()
            .Build();

        Assert.Equal("TestValue", fake.ReferenceFunc());
        Assert.Throws<NotImplementedException>(() => fake.ValueFunc());
    }
    
    public interface ITestInterface
    {
        string ReferenceFunc();
        int ValueFunc();
        void Action();
        
        void OverloadedAction();
        void OverloadedAction(string arg1);
        void OverloadedAction(string arg1, int arg2);
        
        string OverloadedFunction();
        int OverloadedFunction(string arg1);
        int OverloadedFunction(int arg1);
        
        string OverloadedFunction(string arg1, int arg2);
        string OverloadedFunction(int arg1, int arg2);
    }
}

public static class FakeFunc<TReturn>
{
    public static Expression<Func<IDictionary<string, object>,TReturn>> Throws(Exception exception)
    {
        var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
        var def =Expression.Convert(Expression.Constant(default(TReturn)), typeof(TReturn));
        var exp = Expression.Constant(exception);
        var body = Expression.Block(Expression.Throw(exp), def);
        return Expression.Lambda<Func<IDictionary<string, object>, TReturn>>(body, new []{parameters});
    }
}

public static class FakeAction
{
    public static Expression<Action<IDictionary<string, object>>> Throws(Exception exception)
    {
        var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
        var exp = Expression.Constant(exception);
        var body = Expression.Throw(exp);
        return Expression.Lambda<Action<IDictionary<string, object>>>(body, new []{parameters});
    }

    public static object Execute(Action<IDictionary<string, object>> action)
    {
        throw new NotImplementedException();
    }
}