using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Initials
{
    public class Game1 : Game
    {
        // Zarządzanie graficznymi ustawieniami gry.
        private GraphicsDeviceManager _graphics;

        // Obiekt do rysowania na ekranie.
        private SpriteBatch _spriteBatch;

        // Czcionka do wyświetlania tekstu.
        private SpriteFont _font;

        // Lista list punktów kontrolnych dla krzywych Béziera.
        private List<List<Vector2>> controlPointsList;

        // Lista obiektów do narysowania.
        private List<List<Vector2>> curvePointsList;

        // Lista kolorów dla punktów kontrolnych w poszczególnych obiektach.
        private List<Color> controlPointsColors;

        // Stała definiująca promień punktu kontrolnego.
        private const int POINT_RADIUS = 4;

        // Precyzja rysowania krzywych Béziera reprezentowana ilością punktów pośrednich.
        private const int CURVE_PRECISION = 100;

        // Poprzedni stan klawiatury, używany do obsługi przycisków.
        private KeyboardState previousKeyboardState;

        // Poprzedni stan myszy, używany do obsługi przeciągania kropek.
        private MouseState previousMouseState;

        // Struktura przechowująca informacje o przeciąganym punkcie.
        private DraggedPoint? draggedPoint;

        // Wartość toggla widoczności.
        private bool showControlPoints = false;

        // Wartość toggla koordów.
        private bool showPointValues = false;

        // Definicja pixela
        public static Texture2D Pixel { get; private set; }

        // Kod odpalany w trakcie uruchomienia programu
        public Game1()
        {
            // Inicjalizacja ustawień graficznych gry.
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Inicjalizacja list punktów kontrolnych i kolorów.
            controlPointsList = new List<List<Vector2>>();
            curvePointsList = new List<List<Vector2>>();
            controlPointsColors = new List<Color>();
        }

        //Wstępna inicjalizacja
        protected override void Initialize()
        {
            // Inicjalizacja punktów kontrolnych dla różnych obiektów.
            InitializeControlPoints();

            base.Initialize();
        }

         // Metoda inicjalizująca punkty kontrolne krzywych.
        private void InitializeControlPoints()
        {
            List<Vector2> P1 = new List<Vector2>();
            P1.Add(new Vector2(200, 100));
            P1.Add(new Vector2(250, 450));
            P1.Add(new Vector2(130, 350));
            controlPointsList.Add(P1);
            controlPointsColors.Add(Color.Red);

            List<Vector2> P2 = new List<Vector2>();
            P2.Add(new Vector2(160, 175));
            P2.Add(new Vector2(130, 110));
            P2.Add(new Vector2(120, 120));
            P2.Add(new Vector2(200, 10));
            P2.Add(new Vector2(400, 200));
            P2.Add(new Vector2(212, 200));
            controlPointsList.Add(P2);
            controlPointsColors.Add(Color.Green);

            List<Vector2> N1 = new List<Vector2>();
            N1.Add(new Vector2(424, 356));
            N1.Add(new Vector2(570, 476));
            N1.Add(new Vector2(440, 115));
            controlPointsList.Add(N1);
            controlPointsColors.Add(Color.Blue);

            List<Vector2> N2 = new List<Vector2>();
            N2.Add(new Vector2(440, 115));
            N2.Add(new Vector2(562, 213));
            N2.Add(new Vector2(579, 386));
            controlPointsList.Add(N2);
            controlPointsColors.Add(Color.LimeGreen);

            List<Vector2> N3 = new List<Vector2>();
            N3.Add(new Vector2(579, 386));
            N3.Add(new Vector2(666, 40));
            N3.Add(new Vector2(288, 26));
            N3.Add(new Vector2(568, 70));
            N3.Add(new Vector2(669, 162));
            N3.Add(new Vector2(557, 124));
            controlPointsList.Add(N3);
            controlPointsColors.Add(Color.Cyan);

            UpdateCurves();
        }

        //Ładowanie zasobów takich jak czcionka oraz definiowanie czym jest pixel
        protected override void LoadContent()
        {
            // Inicjalizacja obiektu do rysowania na ekranie.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Utworzenie pojedynczego piksela, używanego do rysowania punktów.
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            // Ładowanie czcionki do wyświetlania tekstu.
            _font = Content.Load<SpriteFont>("arial");
        }

        //Metoda wykonywana w każdej klatce gry
        protected override void Update(GameTime gameTime)
        {
            // Pobranie pozycji myszy i stanu klawiatury
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            // Escape wyłącza program
            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // Toggle widoczności punktów przycisk: Space
            if (currentKeyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
                showControlPoints = !showControlPoints;

            // Toggle widoczności koordynatów przycisk: Tab
            if (currentKeyboardState.IsKeyDown(Keys.Tab) && !previousKeyboardState.IsKeyDown(Keys.Tab))
                showPointValues = !showPointValues;

            // Obsługa przeciągania punktów kontrolnych za pomocą myszy.
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

                if (draggedPoint.HasValue)
                {
                    controlPointsList[draggedPoint.Value.ControlPointsListIndex][draggedPoint.Value.PointIndex] = mousePosition;
                    UpdateCurves();
                }
                else
                {
                    foreach (List<Vector2> controlPoints in controlPointsList)
                    {
                        for (int i = 0; i < controlPoints.Count; i++)
                        {
                            Vector2 point = controlPoints[i];
                            if (Vector2.Distance(point, mousePosition) < POINT_RADIUS)
                            {
                                draggedPoint = new DraggedPoint { ControlPointsListIndex = controlPointsList.IndexOf(controlPoints), PointIndex = i };
                                break;
                            }
                        }
                        if (draggedPoint.HasValue)
                            break;
                    }
                }
            }
            else
            {
                draggedPoint = null;
            }

            // Zapisanie aktualnego stanu klawiatury i myszy.
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            base.Update(gameTime);
        }

        // Metoda aktualizująca punkty na krzywych Béziera w zależności od położenia punktów kontrolnych.
        private void UpdateCurves()
        {
            // Wyczyszczenie listy punktów na krzywych Béziera.
            curvePointsList.Clear();

            // Aktualizacja krzywych Béziera dla każdego zestawu punktów kontrolnych.
            foreach (List<Vector2> controlPoints in controlPointsList)
            {
                List<Vector2> curvePoints = new List<Vector2>();
                if (controlPoints.Count >= 2)
                {
                    for (float t = 0; t <= 1; t += 1f / CURVE_PRECISION)
                    {
                        Vector2 point = CalculateBezierPoint(t, controlPoints);
                        curvePoints.Add(point);
                    }
                }
                curvePointsList.Add(curvePoints);
            }
        }

        /// Metoda obliczająca punkt na krzywej Béziera dla danego parametru t.
        private Vector2 CalculateBezierPoint(float t, List<Vector2> points)
        {
            int n = points.Count - 1;
            Vector2[] temp = new Vector2[points.Count];

            for (int i = 0; i <= n; i++)
            {
                temp[i] = points[i];
            }

            for (int k = 1; k <= n; k++)
            {
                for (int i = 0; i <= n - k; i++)
                {
                    temp[i] = (1 - t) * temp[i] + t * temp[i + 1];
                }
            }

            return temp[0];
        }

        // Metoda wywoływana podczas rysowania klatki gry.
        // Rysuje punkty kontrolne, krzywe Béziera oraz wartości punktów nad punktami kontrolnymi (jeśli są widoczne).
        protected override void Draw(GameTime gameTime)
        {
            // Czarne tło
            GraphicsDevice.Clear(Color.Black);

            // Rozpoczęcie rysowania na ekranie.
            _spriteBatch.Begin();

            // Rysowanie punktów kontrolnych (jeśli są widoczne).
            if (showControlPoints)
            {
                for (int i = 0; i < controlPointsList.Count; i++)
                {
                    List<Vector2> controlPoints = controlPointsList[i];
                    List<Vector2> curvePoints = curvePointsList[i];
                    Color pointColor = controlPointsColors[i];

                    foreach (Vector2 point in controlPoints)
                    {
                        Rectangle rect = new Rectangle((int)point.X - POINT_RADIUS, (int)point.Y - POINT_RADIUS, POINT_RADIUS * 2, POINT_RADIUS * 2);
                        _spriteBatch.Draw(Pixel, rect, pointColor);

                        // Wyświetlanie wartości punktu nad punktem (jeśli są widoczne).
                        if (showPointValues)
                        {
                            Vector2 textPosition = new Vector2(point.X, point.Y - POINT_RADIUS - 20); // Przesunięcie tekstu nad punkt
                            string text = $"({point.X}, {point.Y})"; // Tekst z wartościami x i y
                            _spriteBatch.DrawString(_font, text, textPosition, pointColor); // Rysowanie tekstu
                        }
                    }
                }
            }

            // Rysowanie krzywych Béziera.
            for (int i = 0; i < curvePointsList.Count; i++)
            {
                List<Vector2> curvePoints = curvePointsList[i];

                if (curvePoints.Count >= 2)
                {
                    for (int j = 0; j < curvePoints.Count - 1; j++)
                    {
                        _spriteBatch.DrawLine(curvePoints[j], curvePoints[j + 1], Color.White);
                    }
                }
            }

            // Zakończenie rysowania.
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Informacje o przeciąganym punkcie.
        struct DraggedPoint
        {
            public int ControlPointsListIndex;
            public int PointIndex;
        }
    }

    //Klasa zawierająca rozszerzenia dla obiektu SpriteBatch.
    //Zapewnia dodatkowe funkcje rysowania, takie jak rysowanie linii.
    public static class SpriteBatchExtensions
    {
        // Metoda rysująca linię na ekranie za pomocą obiektu SpriteBatch.
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int width = 1)
        {
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(start, end);

            spriteBatch.Draw(Game1.Pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }
    }
}
