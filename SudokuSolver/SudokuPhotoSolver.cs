using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace SudokuSolver
{
    public class SudokuPhotoSolver
    {
        public Bitmap SolveSudokuPhoto(Bitmap sudokuPhoto)
        {
            var image = sudokuPhoto;
            //TODO: Threshold will probably be needed for noisy images
            //var thresholdFilter = new Threshold(100);
            //thresholdFilter.ApplyInPlace(image);
            var invertFilter = new Invert();
            var invertedImage = invertFilter.Apply(image);
            var blobCounter = new BlobCounter {BackgroundThreshold = Color.FromArgb(255, 70, 70, 70)};
            blobCounter.ProcessImage(invertedImage);
            var invertedImageBlobs = blobCounter.GetObjectsInformation();
            var boardBlobCandidates = invertedImageBlobs;
            var biggestBoardBlobCandidate = boardBlobCandidates.OrderByDescending(b => b.Area).First();
            var boardBlob = biggestBoardBlobCandidate;

            var expectedCellBlobHeight = boardBlob.Rectangle.Height/
                                         SudokuBoard.NumberOfBoardCellsInSingleDirection;

            var expectedCellBlobWidth = boardBlob.Rectangle.Width/
                                        SudokuBoard.NumberOfBoardCellsInSingleDirection;

            var blobCellSizeTolerance = 0.25;

            blobCounter.ProcessImage(image);

            var cellBlobCandidates =
                blobCounter.GetObjectsInformation();

            var cellBlobs = cellBlobCandidates.Where(
                b =>
                    boardBlob.Rectangle.Contains(b.Rectangle) &&
                    IsMatch(b.Rectangle.Width, expectedCellBlobWidth, blobCellSizeTolerance) &&
                    IsMatch(b.Rectangle.Height, expectedCellBlobHeight, blobCellSizeTolerance)).ToArray();

            if (cellBlobs.Length != SudokuBoard.NumberOfBoardCells)
            {
                throw new InvalidOperationException();
            }

            var expectedCellsData =
                Enumerable.Range(0, SudokuBoard.NumberOfBoardCellsInSingleDirection)
                    .SelectMany(
                        cvi =>
                            Enumerable.Range(0, SudokuBoard.NumberOfBoardCellsInSingleDirection)
                                .Select(chi => new
                                {
                                    CellHorizontalIndex = chi,
                                    CellVerticalIndex = cvi,
                                    ExpectedCellCenter =
                                        new Point((int) (boardBlob.Rectangle.X + (cvi + 0.5)*expectedCellBlobWidth),
                                            boardBlob.Rectangle.Y + (int) ((chi + 0.5)*expectedCellBlobHeight)),
                                })).ToArray();

            var sudokuBoard = new SudokuBoard();
            var digitImages = new List<Bitmap>();
            var parsedDigitIndexToCellBlobMap = new Dictionary<int, Blob>();
            var lastParsedDigitIndex = 0;

            foreach (var cellBlob in cellBlobs)
            {
                // TODO: There may be more than one blob candidate
                var digitBlob = invertedImageBlobs.SingleOrDefault(b => cellBlob.Rectangle.Contains(b.Rectangle));
                
                if (digitBlob == null)
                {
                    continue;
                }

                var digitImage = image.Clone(digitBlob.Rectangle, image.PixelFormat);
                digitImages.Add(digitImage);
                parsedDigitIndexToCellBlobMap[lastParsedDigitIndex++] = cellBlob;
            }

            IReadOnlyCollection<int> parsedDigits;

            using (var digitParser = new DigitParser())
            {
                parsedDigits = digitParser.ParseDigits(digitImages);
            }

            foreach (var digitImage in digitImages)
            {
                digitImage.Dispose();
            }

            for (var parsedDigitIndex = 0; parsedDigitIndex < parsedDigits.Count; parsedDigitIndex++)
            {
                var digitCellBlob = parsedDigitIndexToCellBlobMap[parsedDigitIndex];

                var expectedCellData =
                    expectedCellsData.Single(d => digitCellBlob.Rectangle.Contains(d.ExpectedCellCenter));

                var parsedDigit = parsedDigits.ElementAt(parsedDigitIndex);

                if (!SudokuBoard.ValidNumbers.Contains(parsedDigit))
                {
                    throw new InvalidOperationException();
                }

                sudokuBoard[expectedCellData.CellHorizontalIndex, expectedCellData.CellVerticalIndex] =
                    parsedDigit;
            }

            var solvedBoard = sudokuBoard.Solve();

            if (solvedBoard == null)
            {
                return null;
            }

            Debug.Assert(solvedBoard.IsComplete() && solvedBoard.IsValid());

            var cellsToPrint = Enumerable.Range(0, SudokuBoard.NumberOfBoardCellsInSingleDirection)
                    .SelectMany(
                        vi =>
                            Enumerable.Range(0, SudokuBoard.NumberOfBoardCellsInSingleDirection)
                                .Select(hi => new
                                {
                                    HorizontalIndex = hi,
                                    VerticalIndex = vi
                                })).Where(c => sudokuBoard[c.HorizontalIndex, c.VerticalIndex] == null).Select(
                                    i =>
                                    {
                                        var expectedCellData =
                                            expectedCellsData.Single(
                                                d =>
                                                    d.CellHorizontalIndex == i.HorizontalIndex &&
                                                    d.CellVerticalIndex == i.VerticalIndex);

                                        var cellBlob =
                                            cellBlobs.Single(
                                                b => b.Rectangle.Contains(expectedCellData.ExpectedCellCenter));

                                        var cellCenter = new Point(cellBlob.Rectangle.X + cellBlob.Rectangle.Width/2,
                                            cellBlob.Rectangle.Y + cellBlob.Rectangle.Height/2);
                                        return new Cell(i.HorizontalIndex, i.VerticalIndex, cellBlob.Rectangle);
                                    }).ToList();

            var solutionPhoto = PrintSolutionToSourceImage(image, cellsToPrint, solvedBoard);
            return solutionPhoto;
        }

        private bool IsMatch(int size, int expectedSize, double tolerance)
        {
            var sizeDifference = Math.Abs(size - expectedSize);
            var relativeSizeDifference = (double) sizeDifference/expectedSize;

            return relativeSizeDifference <= tolerance;
        }

        private Bitmap PrintSolutionToSourceImage(Bitmap sourceImage,
            IReadOnlyCollection<Cell> cellsToPrint, SudokuBoard solvedBoard)
        {
            var solutionImage = sourceImage.Clone(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                PixelFormat.Format24bppRgb);

            using (var solutionImageGraphics = Graphics.FromImage(solutionImage))
            {
                solutionImageGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                solutionImageGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                solutionImageGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                foreach (var cell in cellsToPrint)
                {
                    var borderRatio = 0.1;
                    var horizontalBorderWidth = (int)(cell.Rectangle.Width*borderRatio);
                    var verticalBorderWidth = (int)(cell.Rectangle.Height*borderRatio);

                    var rectangle = new Rectangle(
                        cell.Rectangle.X + horizontalBorderWidth,
                        cell.Rectangle.Y + verticalBorderWidth, 
                        cell.Rectangle.Width - 2*horizontalBorderWidth,
                        cell.Rectangle.Height - 2*verticalBorderWidth);

                    var cellValue = solvedBoard[cell.HorizontalIndex, cell.VerticalIndex];

                    solutionImageGraphics.DrawString(cellValue.ToString(),
                        new Font("Tahoma", rectangle.Height, GraphicsUnit.Pixel),
                        Brushes.DarkRed,
                        rectangle);
                }
            }

            return solutionImage;
        }

        private class Cell
        {
            public Cell(int horizontalIndex, int verticalIndex, Rectangle rectangle)
            {
                HorizontalIndex = horizontalIndex;
                VerticalIndex = verticalIndex;
                Rectangle = rectangle;
            }

            public int HorizontalIndex { get; }
            public int VerticalIndex { get; }
            public Rectangle Rectangle { get; }
        }
    }
}