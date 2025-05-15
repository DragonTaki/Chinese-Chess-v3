/* ----- ----- ----- ----- */
// MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Chinese_Chess_v3.Data;
using Chinese_Chess_v3.Interface.UI.Constants;
using Chinese_Chess_v3.Interface.UI.Core;
using Chinese_Chess_v3.Interface.UI.Elements;
using Chinese_Chess_v3.Renderers;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Interface.UI.Layout
{
    public class MainMenu : UIElement
    {
        private readonly MainMenuRenderer renderer;
        private UIScrollContainer scroll;
        private List<UIButton> buttons = new();

        public MainMenu()
        {
            LocalPosition = UILayoutConstants.MainMenu.Position;
            Size = UILayoutConstants.MainMenu.Size;

            renderer = new MainMenuRenderer(this);

            scroll = new UIScrollContainer();
            scroll.Size = UILayoutConstants.MainMenu.ScrollContainer.Size;
            scroll.InitializeScrollPhysics();
            scroll.Physics.Position = UILayoutConstants.MainMenu.ScrollContainer.Position;
            scroll.BaseScrollY = -UILayoutConstants.MainMenu.Margin;
            scroll.OverscrollLimit = UILayoutConstants.MainMenu.Margin;

            AddChild(scroll);

            var labels = MainMenuButtonList.CreateDefaultButtonLabels();
            for (int i = 0; i < labels.Count; i++)
            {
                var button = new UIButton(labels[i]);
                button.LocalPosition = UILayoutConstants.MainMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin) * i);
                button.Size = UILayoutConstants.MainMenu.Button.Size;

                scroll.AddChild(button);
                buttons.Add(button);
            }
            scroll.ContentHeight = buttons.Count * (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin);
        }

        public void UpdateScrollLayout()
        {
            foreach (var button in buttons)
            {
                button.LocalPosition.Current = button.LocalPosition.Base + new Vector2F(0.0f, scroll.GetContentOffsetY());
            }
        }

        public List<UIButton> GetVisibleButtons()
        {
            RectangleF view = scroll.GetClippingRect();
            return buttons.Where(button =>
            {
                //if (button == null) return false;

                float y = button.LocalPosition.Current.Y;
                float h = button.Size.Y;
                return y + h > view.Top && y < view.Bottom;
            }).ToList();
        }

        public override void Draw(Graphics g)
        {
            scroll.Update();
            UpdateScrollLayout();
            renderer.Draw(g);
        }
        public List<UIButton> Buttons => GetVisibleButtons();
        public RectangleF GetClipRect() => scroll.GetClippingRect();
    }
}