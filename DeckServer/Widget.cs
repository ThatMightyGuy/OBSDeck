using DeckUtils;
using NLua;

namespace DeckServer;
public readonly struct Widget : IWidget, IDisposable
{
    public readonly Guid Id; 
    private readonly Lua state;
    private readonly LuaFunction start;
    private readonly LuaFunction onUpdate;

    private static LuaFunction GetFunction(Lua state, string name)
    {
        object fn = state[name] ?? throw new ArgumentNullException(nameof(name), "Expected LuaFunction, got nil");
        return (LuaFunction)fn;
    }

    public Widget(Lua state)
    {
        Id = Guid.NewGuid();
        this.state = state;
        try
        {
            start = GetFunction(state, "start");
            onUpdate = GetFunction(state, "on_update");
        }
        catch (ArgumentNullException ex)
        {
            throw new InterfaceNotImplementedException("Widget does not implement IWidget", ex);
        }
        Start();
    }
    public Widget(string src)
    {
        Id = Guid.NewGuid();
        state = new();
        state.DoString(src);
        try
        {
            start = GetFunction(state, "start");
            onUpdate = GetFunction(state, "on_update");
        }
        catch (ArgumentNullException ex)
        {
            throw new InterfaceNotImplementedException("Widget does not implement IWidget", ex);
        }
        Start();
    }

    public override string ToString() => $"{base.ToString()}: {Id}";
    
    public void Start() => start.Call(Id.ToString());
    public string OnUpdate() {
        object[] result = onUpdate.Call();
        if(result.Length > 0)
            return (string)result.First();
        else
            throw new InvalidResultException("on_update", "str", "nil");
    }

    public void Dispose()
    {
        start.Dispose();
        onUpdate.Dispose();
        state.Dispose();
    }

}