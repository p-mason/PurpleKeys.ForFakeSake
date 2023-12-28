namespace PurpleKeys.ForFakeSake.UnitTest.SpyInterceptorTests;

public interface ITestInterface
{
    public string Property { get; set; }
    
    public string OverloadFunc(string arg1);
    public string OverloadFunc(string arg1, int arg2);
    
    public void OverloadAction(string arg1);
    public void OverloadAction(string arg1, int arg2);
}