/* ----- ----- ----- ----- */
// InfoBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/10/22
// Version: v2.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Constants.UI;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.InfoBoards
{
    public class InfoBoard : UIContainer<InfoBoardHandler>, IScreen, IDisposable
    {
        public InfoBoard() {}

        protected override void OnInit((IUiFactory, InfoBoardHandler) arg)
        {
            base.OnInit(arg);

            Layout = UILayoutConstants.Sidebar.Infoboard.Layout;

            _renderer = new InfoBoardRenderer(this);
            (_renderer as InfoBoardRenderer)?.Initialize(arg.Item2);
        }
    }
}
