using System.Linq.Expressions;
using System.Reflection;

namespace PurpleKeys.ForFakeSake.Internal;

internal class PropertySetupVisitor<T> : ExpressionVisitor
    where T : class
{
    private readonly Type _declaringType;
    private readonly Dictionary<string, List<FakeSetup>> _properties = new();

    public PropertySetupVisitor()
    {
        _declaringType = typeof(T);
    }

    public IDictionary<string, List<FakeSetup>>PropertySetups { get { return _properties; } }

    public override Expression Visit(Expression? expr)
    {
        if (expr != null && expr is BinaryExpression binExp)
        {
            if (binExp.Left.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpr = (MemberExpression)binExp.Left;
                if (memberExpr.Member.DeclaringType == _declaringType && memberExpr.Member.MemberType == MemberTypes.Property)
                {
                    if (binExp.Right is ConstantExpression constExp)
                    {
                        var sig = ToKey(memberExpr);
                        var setup = new FakeSetup((_) => true, true, (_) => { }, (_) => constExp.Value);
                        _properties.Add(sig, new List<FakeSetup>{setup});
                    }
                    else
                    {
                        var helper = ((MethodCallExpression)binExp.Right).Method.GetCustomAttributes<FakeBuilderHelperAttribute>().SingleOrDefault();
                        if (helper is { ForPropertyGet: true })
                        {
                            var readExpression = (MethodCallExpression)binExp.Right;
                            var sig = ToKey(memberExpr);
                            var readPropertyValue = Expression.Lambda<Func<object>>(Expression.Convert(
                                    Expression.Invoke(readExpression.Arguments[1]),
                                    typeof(object)))
                                .Compile();
                            var setup = new FakeSetup((_) => true, true, (_) => { }, (_) => readPropertyValue());
                            _properties.Add(sig, new List<FakeSetup>{setup});
                        }
                         
                        Console.WriteLine("HERE");
                    }
                }
            }
        }

        return base.Visit(expr);
    }
    
    private string ToKey(MemberExpression expression)
    {
        var property = (PropertyInfo)expression.Member;
        return property.GetMethod!.ToString()!;
    }
}