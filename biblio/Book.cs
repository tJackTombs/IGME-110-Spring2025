using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace biblio {
    public class Book {
        public static readonly Point size = new Point(50, 80);

        public string Title { get; private set; }
        public string Author { get; private set; }
        public bool Classic { get; private set; }
        public string Genre { get; private set; }
        public Point Cover { get; private set; }
        public string Series { get; private set; }

        public Book(string title, string author, bool classic, string genre) {
            Title = title;
            Author = author;
            Classic = classic;
            Genre = genre;
            Cover = new Point(-1);
            Series = null!;
        }

        public Book(string title, string author, bool classic, string genre, Point cover) {
            Title = title;
            Author = author;
            Classic = classic;
            Genre = genre;
            Cover = cover;
            Series = null!;
        }

        public Book(string title, string author, bool classic, string genre, string series) {
            Title = title;
            Author = author;
            Classic = classic;
            Genre = genre;
            Cover = new Point(-1);
            Series = series;
        }

        public Book(string title, string author, bool classic, string genre, Point cover, string series) {
            Title = title;
            Author = author;
            Classic = classic;
            Genre = genre;
            Cover = cover;
            Series = series;
        }
    }
}
