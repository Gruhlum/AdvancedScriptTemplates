using System;
using UnityEditor;

public static class _Menu
{
    public static void RemoveMenuItem(string name) => Menu.RemoveMenuItem(name);
    public static bool MenuItemExists(string menuPath) => Menu.MenuItemExists(menuPath);

    public static void AddMenuItem(string name, string shortcut, bool @checked, int priority,
        Action execute, Func<bool> validate)
    {
        Menu.AddMenuItem(name, shortcut, @checked, priority, execute, validate);
    }
}
