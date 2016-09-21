using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace SudokuSolver
{
    public class SudokuPhotoSolver
    {
        public SudokuBoard SolveSudokuPhoto(Bitmap sudokuPhoto, Bitmap thresholdedImage)
        {
            //TODO: Threshold will probably be needed for noisy images
            //var thresholdFilter = new Threshold(100);
            //thresholdFilter.ApplyInPlace(image);
            var invertFilter = new Invert();
            var invertedImage = invertFilter.Apply(thresholdedImage);

            invertedImage.Save(@"D:\k\trash\invertedInput.jpg");


            var blobCounter = new BlobCounter
                                  {
                                      BackgroundThreshold = Color.FromArgb(255, 70, 70, 70)
                                  };

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

            blobCounter.ProcessImage(thresholdedImage);

            var cellBlobCandidates = blobCounter.GetObjectsInformation();

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
                                            boardBlob.Rectangle.Y + (int) ((chi + 0.5)*expectedCellBlobHeight))
                                })).ToArray();

            var sudokuBoard = new SudokuBoard();

            var dir = Directory.CreateDirectory(Guid.NewGuid().ToString());


            var digitImages = new List<Bitmap>();
            var parsedDigitIndexToCellBlobMap = new Dictionary<int, Blob>();
            var lastParsedDigitIndex = 0;

            int idx = 0;
            foreach (var cellBlob in cellBlobs)
            {
                // TODO: There may be more than one blob candidate
                var digitBlob = invertedImageBlobs.SingleOrDefault(b => cellBlob.Rectangle.Contains(b.Rectangle));

                if (digitBlob == null)
                {
                    continue;
                }

                //todo
                var digitImage = sudokuPhoto.Clone(digitBlob.Rectangle, sudokuPhoto.PixelFormat);
                var expectedCellData = expectedCellsData.Single(d => cellBlob.Rectangle.Contains(d.ExpectedCellCenter));

                digitImage.Save(@"D:\k\Trash\Board\blob"+ (expectedCellData.CellHorizontalIndex +1) + (expectedCellData.CellVerticalIndex +1) + ".jpg");


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

            return solvedBoard;
        }

        private bool IsMatch(int size, int expectedSize, double tolerance)
        {
            var sizeDifference = Math.Abs(size - expectedSize);
            var relativeSizeDifference = (double) sizeDifference/expectedSize;

            return relativeSizeDifference <= tolerance;
        }
    }
}