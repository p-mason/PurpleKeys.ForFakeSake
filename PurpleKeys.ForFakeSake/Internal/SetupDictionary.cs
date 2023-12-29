namespace PurpleKeys.ForFakeSake.Internal;

internal class SetupDictionary : Dictionary<string, List<FakeSetup>>
{
    public void AddSetup(string key, FakeSetup setup)
    {
        if (!TryGetValue(key, out var items))
        {
            items = new List<FakeSetup>();
            Add(key, items);
        }
        items.Add(setup);
    }
    
    public void AddSetups(string key, IEnumerable<FakeSetup> setup)
    {
        if (!TryGetValue(key, out var items))
        {
            items = new List<FakeSetup>();
            Add(key, items);
        }
        items.AddRange(setup);
    }
}