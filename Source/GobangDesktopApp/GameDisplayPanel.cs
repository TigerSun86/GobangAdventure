using GobangGameLib.GameBoard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GobangDesktopApp
{
    /// <summary>
    /// Create this class for DoubleBuffered property.
    /// </summary>
    public class GameDisplayPanel : Panel
    {
        private static readonly Dictionary<PieceType, Brush> PieceToDisplay = new Dictionary<PieceType, Brush>()
        {
            { PieceType.Empty, null },
            { PieceType.P1,Brushes.Black },
            { PieceType.P2,Brushes.White},
        };

        GameThread GameThread;
        int boardLeftX;
        int boardRightX;
        int boardTopY;
        int boardBottomY;
        int cellSize;
        int pieceSize;
        public Point[,] piecePositionsOnScreen = new Point[11, 11];
        Timer graphicsTimer;

        public GameDisplayPanel()
        {
            this.DoubleBuffered = true;

            graphicsTimer = new Timer();
            graphicsTimer.Interval = 1000 / 120;
            graphicsTimer.Tick += GraphicsTimer_Tick;

            // Initialize & Start GameLoop
            GameThread = new GameThread();
            GameThread.Start();

            // Start Graphics Timer
            graphicsTimer.Start();
        }

        public void GameDisplayPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Control control = (Control)sender;
            int rowCount = 11;

            int w = control.ClientSize.Width;
            int h = control.ClientSize.Height;
            int centerX = w / 2;
            int centerY = h / 2;
            int boardSize = Math.Min(w, h);
            boardSize = (int)((boardSize * 0.8) / (rowCount - 1)) * (rowCount - 1);
            boardLeftX = centerX - (int)(boardSize / 2);
            boardRightX = centerX + (int)(boardSize / 2);
            boardTopY = centerY - (int)(boardSize / 2);
            boardBottomY = centerY + (int)(boardSize / 2);

            cellSize = (int)((1 / (double)(rowCount - 1)) * boardSize);
            pieceSize = (int)(cellSize * 0.8);

            for (int r = 0; r < rowCount; r++)
            {
                int startX = boardLeftX;
                int startY = cellSize * r + boardTopY;
                int endX = boardRightX;
                int endY = startY;
                g.DrawLine(Pens.Black, startX, startY, endX, endY);
                g.DrawString(r.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), (int)(startX - pieceSize), startY, new StringFormat());

                for (int c = 0; c < piecePositionsOnScreen.GetLength(1); c++)
                {
                    piecePositionsOnScreen[r, c].Y = startY;
                }
            }

            for (int c = 0; c < rowCount; c++)
            {
                int startX = cellSize * c + boardLeftX;
                int startY = boardTopY;
                int endX = startX;
                int endY = boardBottomY;
                g.DrawLine(Pens.Black, startX, startY, endX, endY);
                g.DrawString(c.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), startX, startY - pieceSize, new StringFormat());

                for (int r = 0; r < piecePositionsOnScreen.GetLength(1); r++)
                {
                    piecePositionsOnScreen[r, c].X = startX;
                }
            }

            for (var i = 0; i < this.GameThread.context.RowSize; i++)
            {
                for (var j = 0; j < this.GameThread.context.ColSize; j++)
                {
                    Point p = this.piecePositionsOnScreen[i, j];

                    this.DrawPiece(g, PieceToDisplay[this.GameThread.board.Get(new Position(i, j))], p, pieceSize);
                }
            }
        }

        public void GameDisplayPanel_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            control.Invalidate();
        }

        public void GameDisplayPanel_MouseUp(object sender, MouseEventArgs e)
        {

            int r = (int)Math.Round(((double)e.Y - this.boardTopY) / this.cellSize);
            r = r < 0 ? 0 : r;

            int c = (int)Math.Round(((double)e.X - this.boardLeftX) / this.cellSize);
            c = c < 0 ? 0 : c;
            Point? p = this.GetPiecePointFromPosition(e.X, e.Y);
            if (p != null)
            {
                this.GameThread.MakeHumanMove(r, c);
            }
        }


        private void GraphicsTimer_Tick(object sender, EventArgs e)
        {
            // Refresh this panel's graphics
            this.Invalidate();
        }

        private Point? GetPiecePointFromPosition(int x, int y)
        {

            int r = (int)Math.Round(((double)y - this.boardTopY) / this.cellSize);
            r = r < 0 ? 0 : r;

            int c = (int)Math.Round(((double)x - this.boardLeftX) / this.cellSize);
            c = c < 0 ? 0 : c;
            Point p = this.piecePositionsOnScreen[r, c];

            double distance = GetDistance(p.X, p.Y, x, y);
            if (distance <= this.pieceSize / 2)
            {
                return p;
            }

            return null;
        }


        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }


        private void DrawPiece(Graphics g, Brush color, Point p, int pieceSize)
        {
            if (color != null)
            {
                g.FillEllipse(color, p.X - pieceSize / 2, p.Y - pieceSize / 2, pieceSize, pieceSize);
            }
        }

    }
}
