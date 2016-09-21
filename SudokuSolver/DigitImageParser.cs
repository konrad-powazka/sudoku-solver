﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Tesseract;

namespace SudokuSolver
{
    public class DigitParser : IDisposable
    {
        private readonly TesseractEngine _engine;

        public DigitParser()
        {
            var tesseractLanguageDataPath = Path.Combine(Directory.GetCurrentDirectory(),
                "tessdata");

            _engine = new TesseractEngine(tesseractLanguageDataPath, "eng", EngineMode.Default);
            _engine.SetVariable("tessedit_char_whitelist", "4123565789");
        }

        public void Dispose()
        {
            _engine.Dispose();
        }

        public IReadOnlyCollection<int> ParseDigits(IEnumerable<Bitmap> digitImages)
        {
            digitImages = digitImages.ToArray();
            var mergedDigitsSpacing = (int) (digitImages.Max(i => i.Width)*0.2);

            var mergedDigitsImageWidth = digitImages.Sum(i => i.Width) +
                                         digitImages.Skip(1).Count()*mergedDigitsSpacing;

            var mergedDigitsImageHeight = digitImages.Max(i => i.Height);

            using (var mergedDigitsImage = new Bitmap(mergedDigitsImageWidth, mergedDigitsImageHeight))
            {
                using (var mergedDigitsImageGraphics = Graphics.FromImage(mergedDigitsImage))
                {
                    mergedDigitsImageGraphics.FillRectangle(Brushes.White, 0, 0, mergedDigitsImage.Width,
                        mergedDigitsImage.Height);

                    var currentDigitImageLeftOffset = 0;

                    foreach (var digitImage in digitImages)
                    {
                        mergedDigitsImageGraphics.DrawImage(digitImage, new Point(currentDigitImageLeftOffset, 0));
                        currentDigitImageLeftOffset += digitImage.Width + mergedDigitsSpacing;
                    }
                }

                using (var mergedDigitsImagePix = PixConverter.ToPix(mergedDigitsImage))
                using (var page = _engine.Process(mergedDigitsImagePix, PageSegMode.SingleWord))
                {
                    var text = page.GetText();
                    return text.Trim().Select(c => int.Parse(c.ToString())).ToList();
                }
            }
        }
    }
}