/* ----- ----- ----- ----- */
// IUiFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.UI.Elements;
using Chinese_Chess_v3.UI.Screens.Menu;

namespace Chinese_Chess_v3.UI.Core
{
    public interface IUiFactory
    {
        UIScrollContainer CreateScrollContainer();
        MainMenu CreateMainMenu();

        T Create<T>() where T : UIElement;
    
        //SubMenu CreateSubMenu();
        //Tooltip CreateTooltip(string text);
    }
}