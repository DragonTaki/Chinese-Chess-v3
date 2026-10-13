/* ----- ----- ----- ----- */
// UIKindMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/13
// Version: v1.8
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;

using Engine.Configs;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.KindMenu
{
    /// <summary>
    /// Routes each game-kind button to the main menu presenter according to the submenu's purpose.
    /// Unwired flows keep the normal buttons and leave the current menu open.
    /// </summary>
    public class UIKindMenuHandler : UIMenuHandler<UIKindMenu, UIKindMenuHandler, UIKindMenuRenderer>, IScreen, ISettingsRowEditor
    {
        public UIKindMenuHandler() { }

        /// <summary>The main menu's decisions (the <see cref="MainMenuPresenter"/> registered in DI).</summary>
        private MainMenuPresenter Presenter => _factory.ServiceProvider.GetRequiredService<MainMenuPresenter>();

        internal AiSetupPresenter AiSetup => Presenter.AiSetup;

        private SettingsScreenModel _model;
        internal PlayerSettings Live => _model.Live;

        /// <summary>The pre-game pages of the prepared settings (规則, 計時 and, for a computer game, 電腦).</summary>
        internal IReadOnlyList<SettingsMenuPage> SetupPages => _model.Pages;

        internal void PrepareSettings(GameKind kind) =>
            Bind(new SettingsScreenModel(kind, Service<PlayerSettings>(), Service<SettingsFile>(), Service<IDialogService>()));

        internal void PrepareComputerSettings() =>
            Bind(SettingsScreenModel.ForComputerGame(Service<PlayerSettings>(), Service<SettingsFile>(), Service<IDialogService>()));

        private T Service<T>() => _factory.ServiceProvider.GetRequiredService<T>();

        private void Bind(SettingsScreenModel model)
        {
            _model?.Exit();
            _model = model;
            _model.Enter(0);
            _model.ValuesChanged += () => Element.RefreshSettings();
        }

        /// <summary>A tab of the pre-game pages selected.</summary>
        internal void SelectSetupPage(int index)
        {
            if (_model != null && _model.SelectTab(index)) Element.ShowSetupPage(index);
        }

        /// <summary>重置: this mode's settings (all its pages, and the computer options for a computer game) return to their defaults.</summary>
        internal void ResetSettings()
        {
            if (_model == null) return;
            if (Element.Purpose == KindMenuPurpose.VsComputer) AiSetup.ResetToDefaults(_model);
            else _model.ResetToDefaults();
            Element.RefreshSettings();
        }

        /// <summary>Saves the chosen opening (empty when none is assigned) with the other computer-game settings.</summary>
        internal void PersistOpening()
        {
            Live.Ai.OpeningName = AiSetup.PersistedOpening;
            _model.CommitEdit();
        }

        internal void BackToKinds()
        {
            _model?.Exit();
            Presenter.BackToKinds();
            Element.ShowKinds();
        }

        internal void StartGame()
        {
            if (Element.Purpose == KindMenuPurpose.VsComputer && _model != null) PersistOpening();
            _model?.Exit();
            if (Element.Purpose == KindMenuPurpose.VsComputer) AiSetup.Start();
            else Presenter.StartPreparedSinglePlayer();
        }

        public void SetToggle(SettingsToggleItem item, bool value) => _model.SetToggle(item, value);
        public void SetChoice(SettingsChoiceItem item, int index) => _model.SetChoice(item, index);
        public void SetNumber(SettingsNumberItem item, float value) => _model.SetNumber(item, value);
        public void SetInteger(SettingsIntegerItem item, int value) => _model.SetInteger(item, value);
        public void SetText(SettingsTextItem item, string value) => _model.SetText(item, value);
        public void CommitEdit() => _model.CommitEdit();

        public void OnEnter() => Element.ShowKinds();
        public void OnExit() => _model?.Exit();

        /// <summary>Passes the chosen kind to the presenter for this submenu's purpose.</summary>
        /// <param name="kind">The chosen game kind.</param>
        public void ChooseKind(GameKind kind)
        {
            switch (Element.Purpose)
            {
                case KindMenuPurpose.SinglePlayer:
                    Presenter.PrepareSinglePlayer(kind);
                    break;
                case KindMenuPurpose.VsComputer:
                    Presenter.StartVsComputer(kind);
                    break;
            }
        }
    }
}
