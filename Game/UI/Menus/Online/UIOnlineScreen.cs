/* ----- ----- ----- ----- */
// UIOnlineScreen.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Linq;

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Platform;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Menus.Online
{
    /// <summary>
    /// Shared blocks for the Multiplayer panel and room screen. The default layout is a
    /// screen-sized menu whose panel is the left column the main menu and a submenu take together
    /// (<see cref="UILayoutSheet.OnlineMenu"/>), with one scrolling column of blocks. It only
    /// builds and shows: every text comes finished from the screen's presenter (the logic layer),
    /// which its handler binds to; the panel overrides the default screen layout.
    /// <para>
    /// The helpers add a block to a parent (the scroll container or a <see cref="UIOnlineGroup"/>);
    /// buttons are also listed in the menu's buttons, which the menu renderer draws.
    /// </para>
    /// </summary>
    public abstract class UIOnlineScreen<TElement, THandler, TRenderer> : UIMenu<TElement, THandler, TRenderer>
        where TElement : UIOnlineScreen<TElement, THandler, TRenderer>
        where THandler : UIMenuHandler<TElement, THandler, TRenderer>
        where TRenderer : UIMenuRenderer<TElement, THandler, TRenderer>
    {
        protected UIOnlineScreen() { }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            // Only used by UIMenu's legacy stacking; the blocks are laid out by the flex gaps.
            ButtonSpacing = UILayoutConstants.OnlineMenu.RowGap;
        }

        protected override void OnInit(IUiFactory factory)
        {
            // Declared sizes (also the pre-layout fallback), then the layout rules that
            // actually place the screen, its panel and its scroll container.
            Layout = UILayoutConstants.OnlineMenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.OnlineMenu.ScrollContainer.Layout;

            LayoutRules.Apply(UILayoutSheet.OnlineMenu.Screen);
            PanelWidth = UILayoutSheet.OnlineMenu.PanelWidth;
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.OnlineMenu.ScrollContainer);
        }

        /// <summary>Re-lays out the column now and updates its scroll height (after blocks were added, removed, shown or hidden).</summary>
        public void RefreshLayout() => Handler.UpdateScrollContentHeight();

        #region Blocks

        /// <summary>A section header (a button box that does nothing when clicked).</summary>
        protected UIButton AddHeader(UIElementBase parent, string text)
        {
            var header = AddButton(parent, UILayoutSheet.OnlineMenu.Header, text, null);
            header.Style = UILayoutStyles.OnlineMenu.HeaderStyle;
            return header;
        }

        /// <summary>A text line in <paramref name="color"/>, hidden until it gets a text (<see cref="SetLine"/>).</summary>
        protected UILabel AddLine(UIElementBase parent, Color color)
        {
            var line = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
            line.Font = UILayoutStyles.OnlineMenu.LineFont;
            line.ForeColor = color;
            line.TextAlign = ContentAlign.MiddleLeft;
            line.WordWrap = true;
            line.LayoutRules.Apply(UILayoutSheet.OnlineMenu.Line);
            line.LayoutRules.Display = DisplayMode.None;
            parent.AddChild(line);
            return line;
        }

        /// <summary>A row: <paramref name="text"/> at its left, its controls (added by the caller) at its right.</summary>
        protected UILabeledRow AddRow(UIElementBase parent, string text)
        {
            var row = _factory.CreateElement<UILabeledRow, UILabeledRowHandler, UILabeledRowRenderer>();
            row.Font = UILayoutStyles.OnlineMenu.RowFont;
            row.TextBrush = UILayoutStyles.OnlineMenu.RowTextBrush;
            row.Text = text ?? string.Empty;
            row.LayoutRules.Apply(UILayoutSheet.OnlineMenu.Row);
            parent.AddChild(row);
            return row;
        }

        /// <summary>A text field in <paramref name="row"/> (masked with the password mask when <paramref name="masked"/>).</summary>
        protected UITextField AddField(UILabeledRow row, bool masked = false)
        {
            var field = _factory.CreateElement<UITextField, UITextFieldHandler, UITextFieldRenderer>();
            field.Style = UILayoutStyles.OnlineMenu.FieldStyle;
            field.Font = UILayoutStyles.OnlineMenu.FieldFont;
            if (masked)
                field.MaskChar = UILayoutStyles.OnlineMenu.PasswordMask;
            field.LayoutRules.Apply(UILayoutSheet.OnlineMenu.Field);
            row.AddChild(field);
            return field;
        }

        /// <summary>A switch in <paramref name="row"/>.</summary>
        protected UIToggleSwitch AddToggle(UILabeledRow row)
        {
            var toggle = _factory.CreateElement<UIToggleSwitch, UIToggleSwitchHandler, UIToggleSwitchRenderer>();
            toggle.Style = UILayoutStyles.OnlineMenu.ToggleStyle;
            toggle.LayoutRules.Apply(UILayoutSheet.OnlineMenu.Toggle);
            row.AddChild(toggle);
            return toggle;
        }

        /// <summary>A group of blocks laid out by <paramref name="rules"/> (<see cref="UILayoutSheet.OnlineMenu.Group"/>, <c>ButtonRow</c>, <c>LineGroup</c>).</summary>
        protected UIOnlineGroup AddGroup(UIElementBase parent, UILayoutStyle rules)
        {
            var group = _factory.CreateElement<UIOnlineGroup, UIOnlineGroupHandler, UIOnlineGroupRenderer>();
            group.LayoutRules.Apply(rules);
            parent.AddChild(group);
            return group;
        }

        /// <summary>A button laid out by <paramref name="rules"/> that calls <paramref name="onClick"/>, drawn by the menu.</summary>
        protected UIButton AddButton(UIElementBase parent, UILayoutStyle rules, string text, Action onClick)
        {
            var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
            button.Text = text ?? string.Empty;
            button.Style = UILayoutStyles.OnlineMenu.ButtonStyle;
            button.Handler.Action = onClick;
            button.LayoutRules.Apply(rules);
            parent.AddChild(button);
            Buttons.Add(button);
            return button;
        }

        /// <summary>Disposes every block inside <paramref name="group"/> (its buttons leave the menu's buttons too), keeping the group.</summary>
        protected void ClearGroup(UIOnlineGroup group)
        {
            Buttons.RemoveAll(button => IsInside(button, group));
            foreach (var child in group.Children.ToArray())
                child.Dispose();  // also detaches it
        }

        private static bool IsInside(UIElementBase element, UIElementBase ancestor)
        {
            for (var current = element.Parent; current != null; current = current.Parent)
                if (current == ancestor)
                    return true;
            return false;
        }

        #endregion

        #region Updates

        /// <summary>Shows <paramref name="text"/> on <paramref name="line"/>; null or empty hides the line.</summary>
        public static void SetLine(UILabel line, string text)
        {
            bool shown = !string.IsNullOrEmpty(text);
            line.Text = shown ? text : string.Empty;
            SetShown(line, shown);
        }

        /// <summary>Takes <paramref name="element"/> into the layout or out of it (and out of drawing and input).</summary>
        public static void SetShown(UIElementBase element, bool shown) =>
            element.LayoutRules.Display = shown ? DisplayMode.Normal : DisplayMode.None;

        /// <summary>
        /// Whether <paramref name="button"/> can be pressed: a button that cannot is dimmed
        /// (<see cref="UILayoutStyles.OnlineMenu.DisabledButtonStyle"/>) and ignores clicks.
        /// </summary>
        public static void SetEnabled(UIButton button, bool enabled)
        {
            button.IsEnabled = enabled;
            button.Style = enabled ? UILayoutStyles.OnlineMenu.ButtonStyle : UILayoutStyles.OnlineMenu.DisabledButtonStyle;
        }

        #endregion
    }
}
