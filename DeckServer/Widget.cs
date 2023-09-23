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
        start = GetFunction(state, "start");
        onUpdate = GetFunction(state, "on_update");
        Start();
    }
    public Widget(string src)
    {
        Id = Guid.NewGuid();
        state = new();
        state.DoString(src);
        start = GetFunction(state, "start");
        onUpdate = GetFunction(state, "on_update");
        Start();
    }

    public override string ToString() => $"{base.ToString()}: {Id}";
    
    public void Start() => start.Call(Id.ToString());
    public string OnUpdate() =>
            (onUpdate.Call().First() as string) ??
                throw new ArgumentNullException("on_update", "Expected str, got nil");

    public void Dispose()
    {
        start.Dispose();
        onUpdate.Dispose();
        state.Dispose();
    }

}