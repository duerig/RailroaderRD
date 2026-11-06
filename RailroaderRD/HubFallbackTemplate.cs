// Mod Settings Hub fallback window - OPTIONAL. Copy this file next to HubLink.cs, delete the #if and #endif lines, and change the namespace.
//
// It gives your settings page a window and a hotkey of its own for players who do NOT have the Mod Settings Hub installed.
// When the hub is installed, this does nothing: the hub (its window, hotkey and button) shows your page instead, so
// nothing is doubled. The hub is looked for each time the key is pressed, so load order does not matter.
//
// Usage (in your mod's OnModLoad / Load, after HubLink.Register):
//
//     HubFallback.Enable("yourname/yourmod", "My Mod", SettingsUi.BuildPage, "<Keyboard>/f9", ctrl: true);
//
// In OnModUnload:
//
//     HubFallback.Disable();
//
// The key is an Input System binding path such as "<Keyboard>/f9" or "<Keyboard>/m". Pass ctrl: true to require Ctrl.
// If you let players choose the key, read their choice and pass it here. BuildPage(UIPanelBuilder page) is the same method
// you give HubLink.Register; this window puts it inside a scroll view, exactly as the hub does.
//
// Needs references to UnityEngine.InputSystem.dll and the game's UI assemblies (the same ones your settings page uses).

using System;
using UI.Builder;
using UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RailroaderRD
{
    internal static class HubFallback
    {
        private const string HubAssembly = "ModSettingsHub";

        private static string _id;
        private static string _title;
        private static Action<UIPanelBuilder> _build;
        private static InputAction _action;

        internal static void Enable(string id, string title, Action<UIPanelBuilder> build, string keyBinding, bool ctrl)
        {
            Disable();
            if (string.IsNullOrWhiteSpace(id) || build == null || string.IsNullOrWhiteSpace(keyBinding)) return;
            _id = "fallback." + id.Replace('/', '.');
            _title = string.IsNullOrWhiteSpace(title) ? id : title;
            _build = build;
            try
            {
                _action = new InputAction("HubFallback." + id, InputActionType.Button);
                if (ctrl)
                {
                    _action.AddCompositeBinding("ButtonWithOneModifier")
                        .With("Modifier", "<Keyboard>/ctrl")
                        .With("Button", keyBinding);
                }
                else
                {
                    _action.AddBinding(keyBinding);
                }
                _action.performed += OnPerformed;
                _action.Enable();
            }
            catch (Exception)
            {
                Disable();   // a bad binding path must never stop the mod from loading
            }
        }

        internal static void Disable()
        {
            if (_action != null)
            {
                _action.performed -= OnPerformed;
                _action.Disable();
                _action.Dispose();
                _action = null;
            }
            try
            {
                var windows = WindowManager.Shared;
                if (windows != null && _id != null && windows.IsWindowOpen(_id)) windows.CloseWindow(_id);
            }
            catch (Exception)
            {
            }
            _build = null;
        }

        private static void OnPerformed(InputAction.CallbackContext context)
        {
            try
            {
                if (_build == null || HubInstalled()) return;
                var windows = WindowManager.Shared;
                if (windows == null) return;   // main menu: the window system is not up yet
                if (windows.IsWindowOpen(_id))
                {
                    windows.CloseWindow(_id);
                    return;
                }
                if (!windows.HasDefinition(_id))   // the game forgets custom windows when a map unloads
                {
                    windows.RegisterCustomWindow(new CustomWindowDefinition
                    {
                        Identifier = _id,
                        Title = _title,
                        Config = WindowConfig.Resizable(new Vector2Int(520, 480), new Vector2Int(380, 260)),
                        BuilderAction = BuildWindow,
                    });
                }
                windows.ShowWindow(_id);
            }
            catch (Exception)
            {
            }
        }

        private static void BuildWindow(UIPanelBuilder builder)
        {
            builder.VScrollView(page =>
            {
                try
                {
                    if (_build != null) _build(page);
                }
                catch (Exception e)
                {
                    page.AddLabel("This page failed to draw: " + e.Message);
                }
            });
        }

        private static bool HubInstalled()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == HubAssembly) return true;
            }
            return false;
        }
    }
}