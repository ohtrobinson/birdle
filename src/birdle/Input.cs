using System.Collections.Generic;
using System.Numerics;
using Pie.Windowing;

namespace birdle;

public static class Input
{
    private static HashSet<Key> _keysDown;
    private static HashSet<Key> _newKeysDown;
    
    private static HashSet<MouseButton> _buttonsDown;
    private static HashSet<MouseButton> _newButtonsDown;

    public static bool KeyDown(Key key) =>
        _keysDown.Contains(key);

    public static bool KeyPressed(Key key) =>
        _newKeysDown.Contains(key);
    
    public static bool MouseButtonDown(MouseButton button) =>
        _buttonsDown.Contains(button);

    public static bool MouseButtonPressed(MouseButton button) =>
        _newButtonsDown.Contains(button);

    public static Vector2 MousePosition;

    static Input()
    {
        _keysDown = new HashSet<Key>();
        _newKeysDown = new HashSet<Key>();

        _buttonsDown = new HashSet<MouseButton>();
        _newButtonsDown = new HashSet<MouseButton>();
    }

    public static void Update()
    {
        _newKeysDown.Clear();
        _newButtonsDown.Clear();
    }

    public static void RegisterKeyDown(Key key)
    {
        _keysDown.Add(key);
        _newKeysDown.Add(key);
    }

    public static void RegisterKeyUp(Key key)
    {
        _keysDown.Remove(key);
        _newKeysDown.Remove(key);
    }

    public static void RegisterButtonDown(MouseButton button)
    {
        _buttonsDown.Add(button);
        _newButtonsDown.Add(button);
    }

    public static void RegisterButtonUp(MouseButton button)
    {
        _buttonsDown.Remove(button);
        _newButtonsDown.Remove(button);
    }
}