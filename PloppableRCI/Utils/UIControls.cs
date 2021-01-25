using UnityEngine;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Static utilities class for creating UI controls.
    /// </summary>
    public static class UIControls
    {
        /// <summary>
        /// Adds a simple pushbutton.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="posX">Relative X postion</param>
        /// <param name="posY">Relative Y position</param>
        /// <param name="text">Button text</param>
        /// <param name="width">Button width (default 200)</param>
        /// <param name="height">Button height (default 30)</param>
        /// <param name="scale">Text scale (default 0.9)</param>
        /// <returns></returns>
        public static UIButton AddButton(UIComponent parent, float posX, float posY, string text, float width = 200f, float height = 30f, float scale = 0.9f)
        {
            UIButton button = parent.AddUIComponent<UIButton>();

            // Size and position.
            button.size = new Vector2(width, height);
            button.relativePosition = new Vector2(posX, posY);

            // Appearance.
            button.textScale = scale;
            button.normalBgSprite = "ButtonMenu";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.disabledTextColor = new Color32(128, 128, 128, 255);
            button.canFocus = false;

            // Text.
            button.text = text;

            return button;
        }


        /// <summary>
        /// Adds a large textfield with an attached label to the left.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="posX">Relative X postion</param>
        /// <param name="posY">Relative Y position</param>
        /// <param name="text">Label text</param>
        /// <param name="width">Textfield width (default 200)</param>
        /// <returns>New large textfield with attached label</returns>
        public static UITextField BigLabelledTextField(UIComponent parent, float posX, float posY, string text, float width = 200f) => LabelledTextField(parent, posX, posY, text, width, 30f, 1.2f, 6);


        /// <summary>
        /// Adds a textfield with an attached label to the left.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="posX">Relative X postion</param>
        /// <param name="posY">Relative Y position</param>
        /// <param name="text">Label text</param>
        /// <param name="width">Textfield width (default 200)</param>
        /// <param name="height">Textfield height (default 22)</param>
        /// <param name="scale">Text scale (default 1.0)</param>
        /// <param name="vertPad">Vertical text padding within textfield box (default 4)</param>
        /// <returns>New textfield with attached label</returns>
        public static UITextField LabelledTextField(UIComponent parent, float posX, float posY, string text, float width = 200f, float height = 22f, float scale = 1.0f, int vertPad = 4)
        {
            UITextField textField = AddTextField(parent, posX, posY, width, height, scale, vertPad);

            // Label.
            UILabel label = textField.AddUIComponent<UILabel>();
            label.textScale = scale;
            label.text = text;
            label.autoSize = true;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.wordWrap = true;

            // Set position.
            label.relativePosition = new Vector2(-(label.width + 5f), (height - label.height) / 2);

            return textField;
        }


        /// <summary>
        /// Adds an input text field at the specified coordinates.
        /// </summary>
        /// <param name="textField">Textfield object</param>
        /// <param name="posX">Relative X postion</param>
        /// <param name="posY">Relative Y position</param>
        /// <param name="parent">component to add to</param>
        /// <param name="height">Textfield height (default 22)</param>
        /// <param name="scale">Text scale (default 1.0)</param>
        /// <param name="vertPad">Vertical text padding within textfield box (default 4)</param>
        /// <param name="tooltip">Tooltip, if any</param>
        /// <returns>New textfield *without* attached label</returns>
        public static UITextField AddTextField(UIComponent parent, float posX, float posY, float width = 200f, float height = 22f, float scale = 1f, int vertPad = 4, string tooltip = null)
        {
            UITextField textField = parent.AddUIComponent<UITextField>();

            // Size and position.
            textField.size = new Vector2(width, height);
            textField.relativePosition = new Vector2(posX, posY);

            // Text settings.
            textField.textScale = scale;
            textField.padding = new RectOffset(6, 6, vertPad, vertPad);
            textField.horizontalAlignment = UIHorizontalAlignment.Center;

            // Behaviour.
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;

            // Appearance.
            textField.color = new Color32(255, 255, 255, 255);
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.disabledTextColor = new Color32(0, 0, 0, 128);
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.disabledBgSprite = "TextFieldPanel";

            // Add tooltip.
            if (tooltip != null)
            {
                textField.tooltip = tooltip;
            }

            return textField;
        }


        /// <summary>
        /// Creates a vertical scrollbar
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="scrollPanel">Panel to scroll</param>
        /// <returns></returns>
        public static UIScrollbar AddScrollbar(UIComponent parent, UIScrollablePanel scrollPanel)
        {
            // Basic setup.
            UIScrollbar newScrollbar = parent.AddUIComponent<UIScrollbar>();
            newScrollbar.orientation = UIOrientation.Vertical;
            newScrollbar.pivot = UIPivotPoint.TopLeft;
            newScrollbar.minValue = 0;
            newScrollbar.value = 0;
            newScrollbar.incrementAmount = 50f;
            newScrollbar.autoHide = true;
            newScrollbar.width = 10f;

            // Tracking sprite.
            UISlicedSprite trackSprite = newScrollbar.AddUIComponent<UISlicedSprite>();
            trackSprite.relativePosition = Vector2.zero;
            trackSprite.autoSize = true;
            trackSprite.anchor = UIAnchorStyle.All;
            trackSprite.size = trackSprite.parent.size;
            trackSprite.fillDirection = UIFillDirection.Vertical;
            trackSprite.spriteName = "ScrollbarTrack";
            newScrollbar.trackObject = trackSprite;

            // Thumb sprite.
            UISlicedSprite thumbSprite = trackSprite.AddUIComponent<UISlicedSprite>();
            thumbSprite.relativePosition = Vector2.zero;
            thumbSprite.fillDirection = UIFillDirection.Vertical;
            thumbSprite.autoSize = true;
            thumbSprite.width = thumbSprite.parent.width;
            thumbSprite.spriteName = "ScrollbarThumb";
            newScrollbar.thumbObject = thumbSprite;

            // Event handler - scroll panel.
            newScrollbar.eventValueChanged += (component, value) => scrollPanel.scrollPosition = new Vector2(0, value);

            // Event handler - mouse wheel (scrollbar and panel).
            parent.eventMouseWheel += (component, mouseEvent) => newScrollbar.value -= mouseEvent.wheelDelta * newScrollbar.incrementAmount;
            scrollPanel.eventMouseWheel += (component, mouseEvent) => newScrollbar.value -= mouseEvent.wheelDelta * newScrollbar.incrementAmount;

            // Event handler to handle resize of scroll panel.
            scrollPanel.eventSizeChanged += (component, newSize) =>
            {
                newScrollbar.relativePosition += new Vector3(scrollPanel.width, 0);
                newScrollbar.height = scrollPanel.height;
            };

            // Attach to scroll panel.
            scrollPanel.verticalScrollbar = newScrollbar;

            return newScrollbar;
        }
    }
}