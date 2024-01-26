using System;

namespace birdle.GameModes;

public abstract class GameMode : IDisposable
{
    public virtual void Initialize() { }

    public virtual void Update(float dt) { }

    public virtual void Draw(float dt) { }

    public virtual void Dispose() { }
}