using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShapeUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Threading;

namespace biblio
{
    public class Game1 : Game {
        enum Section { Browse, Search, Scan, MyLibrary, Account }
        enum BrowseTab { Books, Collections, Libraries }
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Color _bg;
        private Color _banner;

        private List<Button> _buttonList;
        private Button[] _mainButtons;
        private Section _currentSection;
        private Section _previousSection;
        private BrowseTab _currentTab;
        private BrowseTab _previousTab;
        private int _subSection;
        private SpriteFont _fontArial;
        private SpriteFont _fontArialBold;
        private Texture2D _others;
        private Texture2D _logo;
        private Texture2D _bookPlaceHolder;
        private Texture2D _bookCovers;
        private Texture2D _icons;
        private Texture2D _arrow;
        public static Texture2D _pixel;
        private Dictionary<string, Book> _books;
        private Dictionary<string, List<string>> _authors;
        private Dictionary<string, List<string>> _genres;
        private Dictionary<string, List<string>> _collections;
        private List<string> _bookNames;
        private List<string> _authorNames;
        private List<string> _genreNames;
        private List<string> _collectionNames;
        private List<string> _featured;
        private List<string> _reccomended;
        private List<Group> _bookGroups;
        private List<Group> _otherGroups;
        private List<string> _libraryNames;
        private List<Button> _libraries;
        private Dictionary<string, Point> _otherImages;
        private Group _selectedGroup;
        private string _selectedThing;
        private int _selectedLibrary;
        private static MouseState _previousMouseState;
        private static MouseState _currentMouseState;

        private Random _rng;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _previousMouseState = Mouse.GetState();
            _currentSection = Section.Browse;
            _currentTab = BrowseTab.Books;
            _previousSection = Section.Browse;
            _previousTab = BrowseTab.Books;
            _graphics.PreferredBackBufferHeight = 778;
            _graphics.PreferredBackBufferWidth = 402;
            _graphics.ApplyChanges();
            _buttonList = new List<Button>();
            _subSection = 0;
            _banner = new Color(75, 75, 75);
            _bg = new Color(35, 35, 35);
            _mainButtons = new Button[5];
            _collections = new Dictionary<string, List<string>>();
            _authors = new Dictionary<string, List<string>>();
            _genres = new Dictionary<string, List<string>>();
            _books = new Dictionary<string, Book>();
            _featured = new List<string>();
            _reccomended = new List<string>();
            _bookNames = new List<string>();
            _authorNames = new List<string>();
            _collectionNames = new List<string>();
            _genreNames = new List<string>();
            _bookGroups = new List<Group>();
            _otherGroups = new List<Group>();
            _libraries = new List<Button>();
            _libraryNames = new List<string>();
            _rng = new Random();
            _selectedGroup = null!;
            _selectedThing = null!;
            _selectedLibrary = -1;
            _otherImages = new Dictionary<string, Point>();
            for(int i = 0; i < 5; i++) {
                _mainButtons[i] = new Button(new Rectangle(new Point(402 / 5 * i, 698), new Point(80)));
            }
            SetupSection();
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = Content.Load<Texture2D>("PIXEL");
            _fontArial = Content.Load<SpriteFont>("Arial");
            _fontArialBold = Content.Load<SpriteFont>("ArialBold");
            _logo = Content.Load<Texture2D>("Title");
            _bookPlaceHolder = Content.Load<Texture2D>("BookPlaceHolder");
            _icons = Content.Load<Texture2D>("Icons");
            _bookCovers = Content.Load<Texture2D>("BookCovers");
            _arrow = Content.Load<Texture2D>("Arrow");
            StreamReader reader = new StreamReader(File.OpenRead("..\\..\\..\\Content\\books.txt"));
            while(!reader.EndOfStream) {
                string[] bookData = reader.ReadLine().Split('|');
                Book newBook;
                if(bookData[4] == "-") {
                    if(bookData[5] == "-") newBook = 
                            new Book(bookData[0], bookData[1], bookData[2] == "1", bookData[3]);
                    else newBook = 
                            new Book(bookData[0], bookData[1], bookData[2] == "1", bookData[3], bookData[5]);
                }
                else {
                    string[] coverParams = bookData[4].Split(',');
                    Point cover = new Point(int.Parse(coverParams[0]), int.Parse(coverParams[1]));
                    if(bookData[5] == "-") newBook = 
                            new Book(bookData[0], bookData[1], bookData[2] == "1", bookData[3], cover);
                    else newBook = 
                            new Book(bookData[0], bookData[1], bookData[2] == "1", bookData[3], cover, bookData[5]);
                }
                _books.Add(newBook.Title, newBook);
                _bookNames.Add(newBook.Title);
                if(!_genres.ContainsKey(newBook.Genre)) {
                    _genres.Add(newBook.Genre, new List<string>());
                    _genreNames.Add(newBook.Genre);
                }
                _genres[newBook.Genre].Add(newBook.Title);
                if(!_authors.ContainsKey(newBook.Author)) {
                    _authors.Add(newBook.Author, new List<string>());
                    _authorNames.Add(newBook.Author);
                }
                _authors[newBook.Author].Add(newBook.Title);
                if(newBook.Series!=null) {
                    if(!_collections.ContainsKey(bookData[5])) {
                        _collections.Add(bookData[5], new List<string>());
                        _collectionNames.Add(bookData[5]);
                    }
                    _collections[newBook.Series].Add(newBook.Title);
                }
            }
            for(int i = 0; i < 10; i++) {
                _featured.Add(_bookNames[_rng.Next(_bookNames.Count)]);
                _reccomended.Add(_bookNames[_rng.Next(_bookNames.Count)]);
            }
            bool[] picked = new bool[_genreNames.Count];
            _bookGroups.Add(new Group("Featured", 100, _featured, false));
            _bookGroups.Add(new Group("Reccomended for You", 340, _reccomended, false));
            int offset = 580;
            while(_bookGroups.Count < 5) {
                int pickedGenre = _rng.Next(_genreNames.Count);
                if(picked[pickedGenre]) continue;
                picked[pickedGenre] = true;
                _bookGroups.Add(new Group(_genreNames[pickedGenre], offset, _genres[_genreNames[pickedGenre]], false));
                offset += 240;
            }
            _otherGroups.Add(new Group("Franchises", 100, _collectionNames, true));
            _otherGroups.Add(new Group("Genres", 340, _genreNames, true));
            _otherGroups.Add(new Group("Authors", 580, _authorNames, false));
            _others = Content.Load<Texture2D>("Others");
            for(int i = 0;i<_otherGroups.Count;i++) {
                Debug.WriteLine("\n"+_otherGroups[i].Name + ":");
                for(int j = 0; j<_otherGroups[i].Things.Count;j++) {
                    Debug.WriteLine(_otherGroups[i].Things[j]);
                    _otherImages.Add(_otherGroups[i].Things[j], new Point(i * 125, j * 200));
                }
            }
            for(int i = 0; i < 3; i++) {
                _libraries.Add(new Button(new Rectangle(0, 100 + i * 165, 402, 125)));
                _libraryNames.Add("Library" + (i + 1));
            }
            reader.Close();
            // TODO:
            // use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _currentMouseState = Mouse.GetState();
            bool inGroup = false;
            int scroll = _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;

            if(_currentMouseState.Position.Y > 80 && _currentMouseState.Position.Y < 698)
                if(_subSection == 0) switch(_currentSection) {
                    case Section.Browse:
                        if(_currentTab!=BrowseTab.Libraries) {
                            List<Group> groupList = _bookGroups;
                            if(_currentTab == BrowseTab.Collections) groupList = _otherGroups; 
                            foreach(Group group in groupList) {
                                if(group.Bounds.Contains(_currentMouseState.Position)) {
                                    inGroup = true;
                                    if(scroll != 0&&Keyboard.GetState().IsKeyUp(Keys.LeftShift)) 
                                        group.ScrollX = Math.Clamp(group.ScrollX+ scroll / 4,
                                             -75 - group.ThingWidths, 0);
                                    
                                    if(group.Clicky.Check()) {
                                        _subSection = 1;
                                        _selectedGroup = group;
                                    }
                                    else for(int i = 0; i < group.Things.Count; i++) {
                                            if(group.ThingBodies[i].Check()) {
                                                _subSection = 2;
                                                _selectedThing = group.Things[i];
                                                break;
                                            }
                                        }
                                }

                            }
                            if((!inGroup || Keyboard.GetState().IsKeyDown(Keys.LeftShift)) && scroll!=0) {
                                foreach(Group group in groupList) {
                                    group.ScrollY = Math.Clamp(group.ScrollY+scroll,
                                             groupList.Count*-200+100, 0);;
                                }
                            }
                        }
                        else {
                            for(int i = 0;i<_libraries.Count;i++) {
                                //if(scroll!=0) _libraries[i].Y += scroll; 
                                if(_libraries[i].Check()){
                                    _subSection = 3;
                                    _selectedLibrary = i;
                                }
                            }
                        }
                        break;
                    case Section.Search:
                        
                        break;
                    case Section.Scan:
                        
                        break;
                    case Section.MyLibrary:
                        
                        break;
                    case Section.Account:
                        
                        break;
            }
            if(_currentSection == Section.Browse && _subSection == 0) for(int i = 0; i < 3; i++) {
                    if(_buttonList[i].Check()) 
                        _currentTab = (BrowseTab) i;
                }
            for(int i = 0; i < 5; i++) {
                if(_mainButtons[i].Check()) _currentSection = (Section) i;
            }

            if(_currentSection != _previousSection) {
                SetupSection();
                _previousSection = _currentSection;
            }
            else if (_currentTab != _previousTab) {

                _previousTab = _currentTab;
            }
                _previousMouseState = _currentMouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_bg);

            ShapeBatch.Begin(_graphics.GraphicsDevice);


            ShapeBatch.End();

            _spriteBatch.Begin();
            switch(_currentSection) {
                case Section.Browse:
                    switch(_currentTab) {
                        case BrowseTab.Books:
                            if(_subSection==0)
                            foreach(Group group in _bookGroups) {
                                DrawGroup(group);
                            }
                            
                            break;
                        case BrowseTab.Collections:
                            if(_subSection == 0)
                                foreach(Group group in _otherGroups) {
                                    DrawGroup(group);
                                }

                            break;
                        case BrowseTab.Libraries:
                            if(_subSection == 0)
                                for(int i = 0; i < _libraries.Count; i++) {
                                    _spriteBatch.Draw(_bookPlaceHolder, _libraries[i].Bounds, Color.DarkBlue);
                                    _spriteBatch.DrawString(_fontArialBold, _libraryNames[i],
                                        new Vector2(_libraries[i].X, _libraries[i].Y)+new Vector2(15), 
                                        Color.White);
                                }

                            break;
                    }
                    break;
            }
            _spriteBatch.End();

            DrawOverhead();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Checks to see if the mouse was clicked once.
        /// </summary>
        /// <returns>Returns true if the mouse was clicked once and not held; false otherwise.</returns>
        public static bool SingleClick() {
            return _currentMouseState.LeftButton == ButtonState.Pressed 
                && _previousMouseState.LeftButton == ButtonState.Released;
        }

        public void DrawOverhead() {
            ShapeBatch.Begin(_graphics.GraphicsDevice);

            ShapeBatch.Box(0, 0, 402, 80, _banner);
            ShapeBatch.Box(0, 698, 402, 80, _banner);

            if(_currentSection == Section.Browse) {
                Rectangle rect = new Rectangle(0, 40, 134, 40);
                switch(_currentTab) {
                    case BrowseTab.Collections:
                        rect.X = 134;
                        break;
                    case BrowseTab.Libraries:
                        rect.X = 268;
                        break;
                }
                ShapeBatch.Box(rect,_banner,_banner, Color.DarkSeaGreen, Color.DarkSeaGreen);
            }

            

            ShapeBatch.End();

            _spriteBatch.Begin();

            byte[] selected = new byte[5];
            string centerText = "";
            switch(_currentSection) {
                case Section.Browse:
                    selected[0] = 1;
                    _spriteBatch.DrawString(_fontArialBold, "Books", new Vector2(45,50), Color.White);
                    _spriteBatch.DrawString(_fontArialBold, "Libraries", new Vector2(300, 50), Color.White);
                    centerText = "Collections";
                    break;
                case Section.Search:
                    selected[1] = 1;
                    centerText = "Search";
                    break;
                case Section.Scan:
                    selected[2] = 1;
                    centerText = "Scan";
                    break;
                case Section.MyLibrary:
                    selected[3] = 1;
                    centerText = "My Library";
                    break;
                case Section.Account:
                    selected[4] = 1;
                    centerText = "My Account";
                    break;
            }
            _spriteBatch.DrawString(_fontArialBold, centerText,
                new Vector2(201 - _fontArialBold.MeasureString(centerText).X / 2, 50), Color.White);

            for(int i = 0; i < 5; i++) {
                _spriteBatch.Draw(_icons, _mainButtons[i].Bounds,
                    new Rectangle(new Point(i * 100, selected[i] * 100), new Point(100)), Color.White);
            }

            _spriteBatch.Draw(_logo, 
                new Rectangle(new Point(201 - _logo.Width / 4, 0), new Point(_logo.Width/2,_logo.Height/2)), 
                Color.White);
            _spriteBatch.End();
        }

        public void DrawGroup(Group group) {
            _spriteBatch.Draw(_pixel, group.Bounds, _banner);
            _spriteBatch.DrawString(_fontArial, group.Name, new Vector2(10, group.Bounds.Top), Color.White);
            for(int i = 0; i < group.Things.Count; i++) {
                DrawThing(group.Things[i], group.ThingBodies[i], group.ShowNames);
            }
            _spriteBatch.Draw(_arrow, new Vector2(390, group.Bounds.Top), Color.White);
        }

        public void DrawThing(string name, Button body, bool showName) {
            if(showName) _spriteBatch.DrawString(_fontArial, name,
                new Vector2(body.X, body.Y + 170), Color.White);
            _spriteBatch.Draw(_bookPlaceHolder, body.Bounds, Color.White);
            return;
            if(_books.ContainsKey(name)) {

            }
            else if(_authors.ContainsKey(name)) {

            }
            else if(_genres.ContainsKey(name)) {

            }
            else if(_collections.ContainsKey(name)) {

            }
        }

        public void SetupSection() {
            _buttonList.Clear();
            _subSection = 0;
            switch(_currentSection) {
                case Section.Browse:
                    //Setting up overhead tabs.
                    for(int i = 0; i < 3; i++) {
                        _buttonList.Add(new Button(new Rectangle(134 * i, 40, 134, 40)));
                    }

                    break;
                case Section.Search:

                    break;
                case Section.Scan:

                    break;
                case Section.MyLibrary:

                    break;
                case Section.Account:

                    break;

            }
        }
    }
}
