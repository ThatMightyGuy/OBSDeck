using DeckServer;
namespace DeckTests;

public class WidgetUnitTest : IDisposable
{
    [Fact]
    public void BadLuaThrowTest()
    {
        try
        {
            // CA1806: Do not ignore method results
            // The error *is* the expected result
            #pragma warning disable CA1806
            new Widget("errfor('This is definitely an error!')");
            #pragma warning restore CA1806
        }
        catch (NLua.Exceptions.LuaException)
        {
            return;
        }
        Assert.Fail("The code did not throw");
    }

    [Fact]
    public void NilUpdateTest()
    {
        try
        {
            Widget w = new ("function on_update()\nreturn nil\nend");
        }
        catch (NLua.Exceptions.LuaException)
        {
            return;
        }
        Assert.Fail("The code did not throw");
    }

    public void Dispose()
    {

    }
}