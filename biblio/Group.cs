using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace biblio {
    public class Group {

        private List<Button> thingClickies;

        private Rectangle bounds;

        private Point scroll;

        public Rectangle Bounds { get => bounds; }

        public string Name { get; private set; }

        public List<string> Things { get; private set; }

        public List<Button> ThingBodies {get => thingClickies; }

        public int ThingWidths { get; private set; }

        public Button Clicky { get; private set; }

        public bool ShowNames { get; set; }

        public int ScrollX { get => scroll.X; set {
                foreach(Button thing in thingClickies) {
                    thing.X += value - scroll.X;
                }
                scroll.X = value;
            } }

        public int ScrollY {
            get => scroll.Y; set {
                bounds.Y += value - scroll.Y;
                foreach(Button thing in thingClickies) {
                    thing.Y += value - scroll.Y;
                }
                scroll.Y = value;
            }
        }


        public Group(string name, int location, List<string> things, bool showNames) {
            scroll = new Point();
            bounds = new Rectangle(0,location,402,220);
            Name = name;
            Things = things;
            thingClickies = new List<Button>();
            ThingWidths = 0;
            for(int i = 0; i < things.Count; i++) {
                thingClickies.Add(new Button(
                    new Rectangle(bounds.X + 10 + i * 120 - scroll.X, bounds.Y + 30, 100, 160)
                    ));
                ThingWidths += 100;
            }
            Clicky = new Button(new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 5));
            ShowNames = showNames;
        }
    }
}
