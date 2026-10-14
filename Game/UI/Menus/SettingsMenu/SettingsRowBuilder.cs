/* ----- ----- ----- ----- */
// SettingsRowBuilder.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/14
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>Where a settings row's control sends its edits (the settings screens' handler, the online lobby's room settings).</summary>
    public interface ISettingsRowEditor
    {
        /// <summary>A switch flipped.</summary>
        void SetToggle(SettingsToggleItem item, bool value);

        /// <summary>A dropdown's option chosen.</summary>
        void SetChoice(SettingsChoiceItem item, int index);

        /// <summary>A slider moved, also while dragging.</summary>
        void SetNumber(SettingsNumberItem item, float value);

        /// <summary>A number field's edit ended with a legal value.</summary>
        void SetInteger(SettingsIntegerItem item, int value);

        /// <summary>A text field edited.</summary>
        void SetText(SettingsTextItem item, string value);

        /// <summary>A slider drag or text edit ended.</summary>
        void CommitEdit();
    }

    /// <summary>
    /// Builds one setting's row (<see cref="UILabeledRow"/>: its name at the left, the control of
    /// the item's <see cref="SettingsItemKind"/> at the right), as the settings screens show it.
    /// Shared by <see cref="UISettingsMenu"/> and every other screen that edits
    /// <see cref="SettingsMenuItem"/>s (the online lobby's room settings), so they look the same.
    /// </summary>
    public static class SettingsRowBuilder
    {
        /// <summary>
        /// A setting's row: the name, and the control of the item's kind. Its edits go to
        /// <paramref name="editor"/>; each refresher added to <paramref name="refreshers"/> updates
        /// the row (name, values, availability) from a settings instance.
        /// </summary>
        /// <param name="factory">Creates the row and its control.</param>
        /// <param name="item">The setting.</param>
        /// <param name="editor">Receives the control's edits.</param>
        /// <param name="refreshers">Gets the row's refreshers.</param>
        /// <param name="isEnabled">Optional availability of computer-option rows.</param>
        /// <returns>The row (not yet added to a parent).</returns>
        /// <exception cref="ArgumentOutOfRangeException">The item is of an unknown kind.</exception>
        public static UILabeledRow Create(IUiFactory factory, SettingsMenuItem item, ISettingsRowEditor editor, List<Action<PlayerSettings>> refreshers, Func<PlayerSettings, bool> isEnabled = null)
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(editor);
            ArgumentNullException.ThrowIfNull(refreshers);

            var row = factory.CreateElement<UILabeledRow, UILabeledRowHandler, UILabeledRowRenderer>();
            row.Font = UILayoutStyles.SettingsMenu.ItemFont;
            row.TextBrush = UILayoutStyles.SettingsMenu.ItemTextBrush;
            row.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Item);
            refreshers.Add(s =>
            {
                row.Text = item.RowText(s);
                row.TextBrush = (isEnabled?.Invoke(s) ?? true)
                    ? UILayoutStyles.SettingsMenu.ItemTextBrush : UILayoutStyles.SettingsMenu.DisabledItemTextBrush;
            });

            switch (item)
            {
                case SettingsToggleItem toggle:
                    var toggleSwitch = factory.CreateElement<UIToggleSwitch, UIToggleSwitchHandler, UIToggleSwitchRenderer>();
                    toggleSwitch.Style = UILayoutStyles.SettingsMenu.ToggleStyle;
                    toggleSwitch.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Toggle);
                    toggleSwitch.Handler.ValueChanged = value => editor.SetToggle(toggle, value);
                    row.AddChild(toggleSwitch);
                    refreshers.Add(s =>
                    {
                        toggleSwitch.IsOn = toggle.Get(s);
                        toggleSwitch.IsEnabled = toggle.IsAvailable(s);
                    });
                    break;

                case SettingsChoiceItem choice:
                    var dropdown = factory.CreateElement<UIDropdown, UIDropdownHandler, UIDropdownRenderer>();
                    dropdown.Style = UILayoutStyles.SettingsMenu.DropdownStyle;
                    dropdown.Font = UILayoutStyles.SettingsMenu.ValueFont;
                    dropdown.SetOptions(choice.Options);
                    dropdown.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Dropdown(DropdownWidth(choice)));
                    dropdown.Handler.SelectionChanged = index => editor.SetChoice(choice, index);
                    row.AddChild(dropdown);
                    refreshers.Add(s =>
                    {
                        dropdown.SelectedIndex = choice.GetIndex(s);
                        dropdown.IsEnabled = isEnabled?.Invoke(s) ?? true;
                    });
                    break;

                case SettingsNumberItem number:
                    var slider = factory.CreateElement<UISlider, UISliderHandler, UISliderRenderer>();
                    slider.Style = UILayoutStyles.SettingsMenu.SliderStyle;
                    slider.Font = UILayoutStyles.SettingsMenu.ValueFont;
                    slider.SetRange(number.Min, number.Max, number.Step);
                    slider.ValueFormatter = number.FormatValue;
                    slider.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Slider);
                    slider.Handler.ValueChanged = value => editor.SetNumber(number, value);
                    slider.Handler.ValueCommitted = _ => editor.CommitEdit();
                    row.AddChild(slider);
                    refreshers.Add(s => slider.Value = number.Get(s));
                    break;

                case SettingsIntegerItem integer:
                    var numberField = factory.CreateElement<UINumberField, UINumberFieldHandler, UINumberFieldRenderer>();
                    numberField.Style = UILayoutStyles.SettingsMenu.TextFieldStyle;
                    numberField.Font = UILayoutStyles.SettingsMenu.ValueFont;
                    numberField.UnitBrush = UILayoutStyles.SettingsMenu.ItemTextBrush;
                    numberField.Unit = integer.Unit;
                    numberField.Min = integer.Min;
                    numberField.Max = integer.Max;
                    numberField.LayoutRules.Apply(UILayoutSheet.SettingsMenu.NumberField);
                    numberField.Handler.ValueCommitted = value => editor.SetInteger(integer, value);
                    row.AddChild(numberField);
                    // Not while typing: the field itself is the source then.
                    refreshers.Add(s =>
                    {
                        if (!numberField.IsFocused)
                            numberField.Value = integer.Get(s);
                    });
                    break;

                case SettingsTextItem text:
                    var field = factory.CreateElement<UITextField, UITextFieldHandler, UITextFieldRenderer>();
                    field.Style = UILayoutStyles.SettingsMenu.TextFieldStyle;
                    field.Font = UILayoutStyles.SettingsMenu.ValueFont;
                    field.MaxLength = text.MaxLength;
                    field.Placeholder = text.Placeholder;
                    field.LayoutRules.Apply(UILayoutSheet.SettingsMenu.TextField);
                    field.Handler.TextChanged = value => editor.SetText(text, value);
                    field.Handler.TextCommitted = _ => editor.CommitEdit();
                    row.AddChild(field);
                    // Not while typing: the field itself is the source then.
                    refreshers.Add(s =>
                    {
                        if (!field.IsFocused)
                            field.Text = text.Get(s);
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item.Kind, "Unknown settings item kind");
            }

            return row;
        }

        /// <summary>
        /// Width of a choice's dropdown: its longest choice's text with the text padding on both
        /// sides, plus the ▼ arrow and its inset; at least a number's width and at most
        /// <see cref="UILayoutConstants.SettingsMenu.ValueMaxWidth"/>.
        /// </summary>
        private static float DropdownWidth(SettingsChoiceItem choice)
        {
            using var g = GraphicsBackend.Factory.CreateMeasurementContext();
            float longest = choice.Options.Max(option => g.MeasureString(option, UILayoutStyles.SettingsMenu.ValueFont).Width);
            float width = longest + UILayoutConstants.SettingsMenu.ValueTextPaddingX * 2f
                + UILayoutConstants.SettingsMenu.DropdownArrowWidth + UILayoutConstants.SettingsMenu.DropdownArrowInset;
            return Math.Clamp(width, UILayoutConstants.SettingsMenu.ValueWidth, UILayoutConstants.SettingsMenu.ValueMaxWidth);
        }
    }
}
