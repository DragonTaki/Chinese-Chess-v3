/* ----- ----- ----- ----- */
// UISettingsMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v3.1
/* ----- ----- ----- ----- */

using System.Linq;

using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;

using Engine.Configs;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>
    /// Binds a settings screen (遊戲設定 / 單機規則設定 alike) to its
    /// <see cref="SettingsScreenModel"/>, which holds the logic (live edits and their ranges,
    /// when <c>settings.ini</c> is written, 恢復初始, applying the settings): the controls'
    /// changes go to the model's commands, the tab bar and rows show the model's tabs, and the
    /// rows are refreshed when the model's values change.
    /// <see cref="IScreen"/>: the main menu calls <see cref="OnEnter"/>/<see cref="OnExit"/>
    /// when it opens/closes the screen.
    /// </summary>
    public class UISettingsMenuHandler : UIMenuHandler<UISettingsMenu, UISettingsMenuHandler, UISettingsMenuRenderer>, IScreen
    {
        /// <summary>The screen's logic, created when the screen is first opened (for <see cref="UISettingsMenu.Screen"/>).</summary>
        private SettingsScreenModel _model;

        public UISettingsMenuHandler() { }

        /// <summary>Screen opened: set up the tabs and show the initial one.</summary>
        public void OnEnter()
        {
            if (_model == null || _model.Screen != Element.Screen)
            {
                if (_model != null)
                    _model.ValuesChanged -= OnValuesChanged;
                var services = _factory.ServiceProvider;
                _model = new SettingsScreenModel(
                    Element.Screen,
                    services.GetRequiredService<PlayerSettings>(),
                    services.GetRequiredService<SettingsFile>(),
                    services.GetRequiredService<IDialogService>());
                _model.ValuesChanged += OnValuesChanged;
            }

            _model.Enter(Element.InitialTab);
            Element.SetTabs(_model.Pages.Select(p => p.Title), _model.CurrentTab);
            ShowCurrentPage();
        }

        /// <summary>A tab selected: show its rows.</summary>
        public void SelectTab(int index)
        {
            if (_model != null && _model.SelectTab(index))
                ShowCurrentPage();
        }

        /// <summary>Screen closed: an edit still pending is written (<see cref="SettingsScreenModel.Exit"/>).</summary>
        public void OnExit() => _model?.Exit();

        /// <summary>A switch flipped (<see cref="SettingsScreenModel.SetToggle"/>).</summary>
        public void SetToggle(SettingsToggleItem item, bool value) => _model?.SetToggle(item, value);

        /// <summary>A dropdown's option chosen (<see cref="SettingsScreenModel.SetChoice"/>).</summary>
        public void SetChoice(SettingsChoiceItem item, int index) => _model?.SetChoice(item, index);

        /// <summary>A slider moved, also while dragging (<see cref="SettingsScreenModel.SetNumber"/>).</summary>
        public void SetNumber(SettingsNumberItem item, float value) => _model?.SetNumber(item, value);

        /// <summary>A number field's edit ended with a legal value (<see cref="SettingsScreenModel.SetInteger"/>).</summary>
        public void SetInteger(SettingsIntegerItem item, int value) => _model?.SetInteger(item, value);

        /// <summary>A text field edited (<see cref="SettingsScreenModel.SetText"/>).</summary>
        public void SetText(SettingsTextItem item, string value) => _model?.SetText(item, value);

        /// <summary>A slider drag or text edit ended (<see cref="SettingsScreenModel.CommitEdit"/>).</summary>
        public void CommitEdit() => _model?.CommitEdit();

        /// <summary>恢復初始 clicked (<see cref="SettingsScreenModel.RequestReset"/>).</summary>
        public void ResetRequested() => _model?.RequestReset();

        /// <summary>Shows the model's current tab, if any.</summary>
        private void ShowCurrentPage()
        {
            var page = _model.CurrentPage;
            if (page != null)
                Element.ShowPage(page, _model.Live);
        }

        /// <summary>The model applied an edit or a reset: refresh the controls.</summary>
        private void OnValuesChanged() => Element.RefreshValues(_model.Live);
    }
}
