/* ----- ----- ----- ----- */
// UIInfoBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2026/10/01
// Version: v2.2
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards
{
    public class UIInfoBoard : UIContainer<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>, IResettable
    {
        /// <summary>Player2's name on the board; null (the default) for the name of the colour it plays (see <see cref="GetPlayerName"/>).</summary>
        public string Player2Name { get; set; } = null;

        /// <summary>Player1's name on the board; null (the default) for the name of the colour it plays (see <see cref="GetPlayerName"/>).</summary>
        public string Player1Name { get; set; } = null;

        /// <summary>The game shown (clocks, turn, check, colours); set by <see cref="UIInfoBoardHandler.SetGameManager"/>.</summary>
        public GameManager GameManager { get; internal set; }

        /// <summary>
        /// The side this machine plays in a network game: shown on the left, the opponent on the
        /// right. Null (the default) for a local game, laid out by <see cref="LeftSide"/>.
        /// </summary>
        public PlayerSide? LocalSide { get; set; } = null;

        /// <summary>
        /// The side shown in the left half: <see cref="LocalSide"/> in a network game; otherwise
        /// Player1 (紅方) on the Full board — fixed red left, black right, whoever moves first —
        /// and on a half board the side that moved first (<see cref="GameManager.FirstTurn"/>).
        /// Each half is coloured by the colour its player actually plays (<see cref="GameManager.ColorOf"/>).
        /// </summary>
        public PlayerSide LeftSide =>
            LocalSide ?? (GameManager.Board.Type == BoardType.Full ? PlayerSide.Player1 : GameManager.FirstTurn);

        /// <summary>The side shown in the right half: the other one of <see cref="LeftSide"/>.</summary>
        public PlayerSide RightSide => LeftSide == PlayerSide.Player1 ? PlayerSide.Player2 : PlayerSide.Player1;

        public UIInfoBoard() { }

        /// <summary>
        /// The name shown for <paramref name="side"/>: <see cref="Player1Name"/> /
        /// <see cref="Player2Name"/> when set, otherwise the colour it plays
        /// (<see cref="GameManager.ColorOf"/>) — 紅方玩家 / 黑方玩家, or 先手玩家 / 後手玩家 while a
        /// dark-chess game has not decided the colours yet.
        /// </summary>
        /// <param name="side">Player1 or Player2.</param>
        /// <returns>The name to draw.</returns>
        public string GetPlayerName(PlayerSide side)
        {
            string name = side == PlayerSide.Player2 ? Player2Name : Player1Name;
            if (name != null)
                return name;

            return GameManager?.ColorOf(side) switch
            {
                PieceColor.Red => "紅方玩家",
                PieceColor.Black => "黑方玩家",
                _ => side == GameManager?.FirstTurn ? "先手玩家" : "後手玩家",
            };
        }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.InfoBoard.Layout;

            // Flex item of the sidebar column (see UILayoutSheet.GameScreen.InfoBoard).
            LayoutRules.Apply(UILayoutSheet.GameScreen.InfoBoard);
        }
    }
}
