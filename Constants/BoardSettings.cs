namespace Chinese_Chess_v3.Constants
{
    public static class BoardSettings
    {
        /* ----- In Palace: General, Advisor -----*/
        // Both side palace area 3 <= X <= 5
        public static readonly (int MinX, int MaxX) PalaceXRange = (3, 5);
        // Red palace area (3, 7) to (5, 9)
        public static readonly (int MinY, int MaxY) RedPalaceYRange = (7, 9);

        // Black palace area (3, 0) to (5, 2)
        public static readonly (int MinY, int MaxY) BlackPalaceYRange = (0, 2);

        /* ----- Own Side -----*/
        public static readonly int RedYSideLimit = 5;    // Red side (Y >= 5)
        public static readonly int BlackYSideLimit = 5;  // Black side (Y <= 4)

    }
}