using System.Collections.Generic;
using System.Numerics;
using Pie.Windowing;

namespace birdle;

public class Input
{
    private HashSet<Key> _keysDown;
    private HashSet<Key> _newKeysDown;
    
    private HashSet<MouseButton> _buttonsDown;
    private HashSet<MouseButton> _newButtonsDown;

    public bool KeyDown(Key key) =>
        _keysDown.Contains(key);

    public bool KeyPressed(Key key) =>
        _newKeysDown.Contains(key);
    
    public bool MouseButtonDown(MouseButton button) =>
        _buttonsDown.Contains(button);

    public bool MouseButtonPressed(MouseButton button) =>
        _newButtonsDown.Contains(button);

    public Vector2 MousePosition;

    public Input()
    {
        _keysDown = new HashSet<Key>();
        _newKeysDown = new HashSet<Key>();

        _buttonsDown = new HashSet<MouseButton>();
        _newButtonsDown = new HashSet<MouseButton>();
    }

    public void Update()
    {
        _newKeysDown.Clear();
        _newButtonsDown.Clear();
    }

    public void RegisterKeyDown(Key key)
    {
        _keysDown.Add(key);
        _newKeysDown.Add(key);
    }

    public void RegisterKeyUp(Key key)
    {
        _keysDown.Remove(key);
        _newKeysDown.Remove(key);
    }

    public void RegisterButtonDown(MouseButton button)
    {
        _buttonsDown.Add(button);
        _newButtonsDown.Add(button);
    }

    public void RegisterButtonUp(MouseButton button)
    {
        _buttonsDown.Remove(button);
        _newButtonsDown.Remove(button);
    }
}