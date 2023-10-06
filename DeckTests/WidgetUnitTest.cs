using DeckServer;
using DeckUtils;
using NLua.Exceptions;

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
        catch (LuaException)
        {
            return;
        }
        Assert.Fail("The code did not throw");
    }

    [Fact]
    public void InterfaceNotImplementedThrowTest()
    {
        try
        {
            Widget w = new ("function on_update() end");
        }
        catch (ArgumentNullException)
        {
            return;
        }
        Assert.Fail("The code did not throw");
    }
    
    [Fact]
    public void NilUpdateThrowTest()
    {
        try
        {
            Widget w = new ("function start() end function on_update() end");
            w.OnUpdate();
        }
        catch (InvalidResultException)
        {
            return;
        }
        Assert.Fail("The code did not throw");
    }

    public void Dispose()
    {

    }
}