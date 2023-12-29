using PurpleKeys.ForFakeSake.Extensions;
namespace PurpleKeys.ForFakeSake.UnitTest.Extensions;

public class DictionaryExtensionUnitTests
{
    private readonly object _key1 = new();
    private readonly object _key2 = new();
    
    [Fact]
    public void PocoReferenceTypes_NotContains_UsesReferenceEquals()
    {
        var d = CreateInitialDictionary();
        
        Assert.True(d.NotContaining("Key1", _key2));
    }
    
    [Fact]
    public void PocoReferenceTypesUsingEqualityComparer_NotContaining_UsesEqualityComparer()
    {
        var d = CreateInitialDictionary();
        var t = new ClassWithEqualityMembers { Property = "DifferentProperty" };
        Assert.True(d.NotContaining("ClassWithEqualityMembersKey", t));
    }
    
    [Fact]
    public void ReferenceTypesWithEqualityMembers_NotContaining_UsesEqualityMethods()
    {
        var d = CreateInitialDictionary();
        var match = new EqualityComparableClass { Property = "Property" };
        
        Assert.True(d.NotContaining("EqualityComparableClassKey", match));
        Assert.False(d.NotContaining("EqualityComparableClassKey", match, EqualityComparableClass.PropertyComparer));
    }
    
    [Fact]
    public void PocoReferenceTypes_Contains_UsesReferenceEquals()
    {
        var d = CreateInitialDictionary();
        
        Assert.True(d.Contains("Key1", _key1));
        Assert.False(d.Contains("Key1", _key2));
    }
    
    [Fact]
    public void PocoReferenceTypesUsingEqualityComparer_Contains_UsesEqualityComparer()
    {
        var d = CreateInitialDictionary();
        var t = new ClassWithEqualityMembers { Property = "Property" };
        Assert.True(d.Contains("ClassWithEqualityMembersKey", t));
    }
    
    [Fact]
    public void ReferenceTypesWithEqualityMembers_Contains_UsesEqualityMethods()
    {
        var d = CreateInitialDictionary();
        var match = new EqualityComparableClass { Property = "Property" };
        
        Assert.False(d.Contains("EqualityComparableClassKey", match));
        Assert.True(d.Contains("EqualityComparableClassKey", match, EqualityComparableClass.PropertyComparer));
    }

    private Dictionary<string, object> CreateInitialDictionary()
    {
        return new Dictionary<string, object>
        {
            { "Key1", _key1 },
            { "Key2", _key2 },
            { "Key3", 3 },
            { "ClassWithEqualityMembersKey", new ClassWithEqualityMembers { Property = "Property" } },
            { "EqualityComparableClassKey", new EqualityComparableClass { Property = "Property" } }
        };
    }

    private class ClassWithEqualityMembers
    {
        public string Property { get; set; }

        protected bool Equals(ClassWithEqualityMembers other)
        {
            return Property == other.Property;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ClassWithEqualityMembers)obj);
        }

        public override int GetHashCode()
        {
            return Property.GetHashCode();
        }
    }

    public class EqualityComparableClass
    {
        public string Property { get; set; }

        private sealed class PropertyEqualityComparer : IEqualityComparer<EqualityComparableClass>
        {
            public bool Equals(EqualityComparableClass x, EqualityComparableClass y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.Property == y.Property;
            }

            public int GetHashCode(EqualityComparableClass obj)
            {
                return obj.Property.GetHashCode();
            }
        }

        public static IEqualityComparer<EqualityComparableClass> PropertyComparer { get; } = new PropertyEqualityComparer();
    }
}