/* ----- ----- ----- ----- */
// UIOpeningMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core.Openings;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.OpeningMenu
{
    /// <summary>
    /// The opening submenu's binding (shared part:
    /// <see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}"/>): lists the
    /// openings of the <see cref="OpeningCatalog"/> (先手 / 後手 sections) and starts one with
    /// <see cref="GameSession.StartOpening"/> (position, then the opening line played onto
    /// the board; the player continues from there).
    /// </summary>
    public class UIOpeningMenuHandler : UICategoryListMenuHandler<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer, OpeningLine>
    {
        /// <summary>The built-in openings (<see cref="OpeningCatalog.BuiltInFolder"/>).</summary>
        public static string BuiltInFolder => OpeningCatalog.BuiltInFolder;

        /// <summary>The player's own openings (<see cref="OpeningCatalog.UserFolder"/>).</summary>
        public override string UserFolder => Catalog.UserFolder;

        public UIOpeningMenuHandler() { }

        /// <summary>The opening catalog registered in DI.</summary>
        private OpeningCatalog Catalog => _factory.ServiceProvider.GetRequiredService<OpeningCatalog>();

        protected override CategoryListModel<OpeningLine> Model => Catalog.List;

        protected override string LogLabel => "Opening";

        protected override string EmptyMessageText => MenuTexts.NoOpenings(UserFolder);

        protected override void StartInSession(GameSession session, OpeningLine opening) =>
            session.StartOpening(opening, LogLabel);
    }
}
