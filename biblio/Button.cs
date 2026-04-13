using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace biblio {
    public class Button {
        private Rectangle bounds;
        
        public Rectangle Bounds { get => bounds; }

        public int X { get => bounds.X; set => bounds.X = value; }
        public int Y { get => bounds.Y; set => bounds.Y = value; }

        public Button(Rectangle bounds) {
            this.bounds = bounds;
        }

        public bool Check() {
            MouseState mouse = Mouse.GetState();
            return bounds.Contains(mouse.Position) && Game1.SingleClick();
        }
    }
}
