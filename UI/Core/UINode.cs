using System.Drawing;

namespace Chinese_Chess_v3.UI.Core
{
    /// <summary>
    /// A generic node element that does not render itself and always passes hit tests.
    /// Used as a structural container for other UI elements.
    /// </summary>
    public class UINode : UIElement
    {
        public UINode(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
            : base(zIndex, isPersistent, type)
        {
            /* no-op */
        }

        /// <summary>
        /// Always returns true to participate in hit testing.
        /// </summary>
        public override bool HitTest(PointF point) => true;
    }
}
