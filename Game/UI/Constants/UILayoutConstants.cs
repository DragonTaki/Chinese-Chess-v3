/* ----- ----- ----- ----- */
// UILayoutConstants.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Mathematics;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    public static partial class UILayoutConstants
    {
        /// <summary>
        /// The content's own authored size — MainMenu + Board + Sidebar side
        /// by side, at MainMenu's height. Every layout constant in this
        /// class (and everything drawn through <c>UIRootNode.Draw</c>) is
        /// positioned in this coordinate space; it never changes at
        /// runtime. Pushed into <c>Engine.Globals.GlobalViewport.DesignSize</c>
        /// once at startup, which scales this space to fit whatever the
        /// actual window size is.
        /// </summary>
        public static Vector2F DesignSize =>
            new Vector2F(MainMenu.Size.X + Board.Size.X + Sidebar.Size.X, MainMenu.Size.Y);

        /// <summary>
        /// The OS window's size when the app first launches. Deliberately
        /// not the same as <see cref="DesignSize"/> — this is a common
        /// desktop resolution (1080p), picked so the window opens at a
        /// normal size on a normal screen; content is then scaled up to
        /// fill it via <c>GlobalViewport</c>, same as any later resize.
        /// </summary>
        public static readonly Vector2F DefaultWindowSize = new Vector2F(1920, 1080);

        /// <summary>
        /// The smallest the OS window is allowed to shrink to — below this,
        /// scaled-down content and text stop being legible. Half of
        /// <see cref="DefaultWindowSize"/>, same aspect ratio.
        /// </summary>
        public static readonly Vector2F MinimumWindowSize = new Vector2F(960, 540);
    }
}
