/* ----- ----- ----- ----- */
// UIOpeningMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Openings;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.OpeningMenu
{
    /// <summary>
    /// The opening submenu's logic (shared part:
    /// <see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}"/>): loads the
    /// openings with <see cref="OpeningLoader.LoadAll"/> and starts one with
    /// <see cref="GameManager.StartOpening"/> (position, then the opening line played onto
    /// the board; the player continues from there).
    /// </summary>
    public class UIOpeningMenuHandler : UICategoryListMenuHandler<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer, OpeningLine>
    {
        /// <summary>The built-in openings (<see cref="SystemSettings.BuiltInOpeningFolder"/>).</summary>
        public static string BuiltInFolder => SystemSettings.BuiltInOpeningFolder;

        /// <summary>
        /// The player's own openings: <see cref="PlayerSettings.ResolvedOpeningUserFolder"/>
        /// (<c>[opening] user_folder</c> in settings.ini; empty there means
        /// <see cref="SystemSettings.DefaultUserOpeningFolder"/>, <c>Openings</c> in the
        /// game's per-user data folder). Created by <see cref="OpeningLoader.LoadAll"/> when missing.
        /// </summary>
        public override string UserFolder =>
            (_factory?.ServiceProvider.GetService<PlayerSettings>() ?? PlayerSettings.Defaults).ResolvedOpeningUserFolder;

        public UIOpeningMenuHandler() { }

        protected override string LogLabel => "Opening";

        protected override string EmptyMessageText =>
            $"找不到開局。\n可以把開局檔（.pgn）放到：\n{UserFolder}";

        protected override IEnumerable<OpeningLine> LoadItems(List<string> warnings) =>
            OpeningLoader.LoadAll(BuiltInFolder, UserFolder, warnings);

        protected override void StartOnBoard(GameManager gameManager, OpeningLine opening) =>
            gameManager.StartOpening(opening);
    }
}
