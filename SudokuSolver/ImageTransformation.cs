using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;

using Image = AForge.Imaging.Image;

namespace SudokuSolver
{
    //public struct Point
    //{
    //    public int X;
    //    public int Y;

    //    public Point(int x, int y)
    //    {
    //        X = x;
    //        Y = y;
    //    }
    //}

    public class ImageTransformation
    {
        private Bitmap image;

        public ImageTransformation(Bitmap image)
        {
            this.image = image;
        }

        public ImageTransformation(string fileName)
        {
            this.image = Image.FromFile(fileName);


            //TODO: Threshold will probably be needed for noisy images
            //var thresholdFilter = new Threshold(100);
            //thresholdFilter.ApplyInPlace(image);
            //var invertFilter = new Invert();
            //var invertedImage = invertFilter.Apply(image);


        }

        //public void TranformImage(string outputFileName)
        //{
        //    /*
        //    BaseUsingCopyPartialFilter[] detectors = new BaseUsingCopyPartialFilter[]
        //                                                 {
        //                                                     new HomogenityEdgeDetector(), 
        //                                                     new CannyEdgeDetector(),
        //                                                     new DifferenceEdgeDetector(),  
        //                                                     new SobelEdgeDetector()
        //                                                 };

        //    for (int i = 0; i < detectors.Length; i++)
        //    {
        //        BaseUsingCopyPartialFilter detector = detectors[i];*/
        //        //using (Bitmap newBmp = new Bitmap(image))
        //        List<IntPoint> corners;
        //        using (Bitmap edgesBitmap = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format8bppIndexed))
        //        {
        //            DifferenceEdgeDetector detector = new DifferenceEdgeDetector();
        //            detector.ApplyInPlace(edgesBitmap);

        //            QuadrilateralFinder qf = new QuadrilateralFinder();
        //            corners = qf.ProcessImage(edgesBitmap);

        //            //    AdaptiveSmoothing filter = new AdaptiveSmoothing();
        //            //filter.ApplyInPlace(targetBmp);

        //            //Threshold filter = new Threshold(100);
        //            //filter.ApplyInPlace(targetBmp);

        //            edgesBitmap.Save(@"D:\k\trash\CornersTest.jpg");
        //            //return;
        //        }

        //        //Invert invertFilter = new Invert();
        //        //Bitmap invertedImage = invertFilter.Apply(image);

        //        //Bitmap edgesBitmap = new Bitmap(image);
        //        //HomogenityEdgeDetector detector = new HomogenityEdgeDetector();
        //        //detector.ApplyInPlace(edgesBitmap);

        //        int boundingSquareSideLength = GetBoundingSquareSideLength(corners);
        //        QuadrilateralTransformation transformation = new QuadrilateralTransformation(corners, boundingSquareSideLength, boundingSquareSideLength);
        //    Bitmap bmp2 = transformation.Apply(image);

        //    Grayscale grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
        //    bmp2 = grayscale.Apply(bmp2);
        //    //bmp2.Save(outputFileName);

        //    //using (Bitmap bmp2 = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format16bppGrayScale))
        //    {
        //        Threshold filter = new Threshold(220);
        //        filter.ApplyInPlace(bmp2);
        //        bmp2.Save(outputFileName);
        //    }


        //    //Bitmap newImage = transformation.Apply(image);
        //    //newImage.Save(outputFileName);

        //    //using (Graphics g = Graphics.FromImage(image))
        //    //{
        //    //    int len = 2;
        //    //    foreach (IntPoint corner in corners)
        //    //    {

        //    //        image.SetPixel(corner.X, corner.Y, Color.White);
        //    //        //g.
        //    //        //g.FillEllipse(Brushes.Red, corner.X -len, corner.Y + len, corner.X + len, corner.Y - len);
        //    //    }

        //    //    //image.Save(@"D:\k\trash\CornersTest"+i+".jpg");
        //    //}
        //    // }


        //    //QuadrilateralTransformation transformation = new QuadrilateralTransformation(corners);
        //    //QuadrilateralTransformation transformation = new QuadrilateralTransformation(corners, invertedImage.Width, invertedImage.Height);

        //    //Bitmap newImage = transformation.Apply(image);

        //    //newImage.Save(outputFileName);
        //}

        public static Bitmap ConvertToGreyscale(Bitmap bitmap)
        {
            Grayscale grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
            return grayscale.Apply(bitmap);

            //return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format8bppIndexed);
        }

        public static Bitmap PerformThresholding(Bitmap bitmap, int value = 100)
        {
            using (Bitmap grayscaleBitmap = ImageTransformation.ConvertToGreyscale(bitmap))
            {
                //grayscaleBitmap.Save(@"D:\k\trash\grayscaleBitmap.jpg");
                if (grayscaleBitmap.PixelFormat == PixelFormat.Format16bppGrayScale)
                    value *= 255;

                Threshold filter = new Threshold(value);
                return filter.Apply(grayscaleBitmap);
            }
        }

        public static Bitmap TranformImage(Bitmap image)
        {
            List<IntPoint> corners;
            //using (Bitmap workingBitmap = (Bitmap)image.Clone())
            using (Bitmap grayscaleBitmap = ConvertToGreyscale(image))
            {
                DifferenceEdgeDetector detector = new DifferenceEdgeDetector();
                detector.ApplyInPlace(grayscaleBitmap);

                //grayscaleBitmap.Save(@"D:\k\trash\workingBitmap.jpg");

                //todo: threshold value
                new Threshold(20).ApplyInPlace(grayscaleBitmap);

                //grayscaleBitmap.Save(@"D:\k\trash\workingBitmapAfterThreshold.jpg");

                QuadrilateralFinder quadrilateralFinder = new QuadrilateralFinder();
                corners = quadrilateralFinder.ProcessImage(grayscaleBitmap);

                //todo del
                //using (Bitmap clone = workingBitmap.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppArgb))
                //{
                //    foreach (IntPoint corner in corners)
                //    {

                //        clone.SetPixel(corner.X, corner.Y, Color.Red);
                //        //g.
                //        //g.FillEllipse(Brushes.Red, corner.X -len, corner.Y + len, corner.X + len, corner.Y - len);
                //    }
                //    clone.Save(@"D:\k\trash\edgeDifference.jpg");
                //}
            }

            int boundingSquareSideLength = GetBoundingSquareSideLength(corners);
            QuadrilateralTransformation quadrilateralTransformation
                = new QuadrilateralTransformation(corners, boundingSquareSideLength, boundingSquareSideLength);

            return quadrilateralTransformation.Apply(image);
        }

        private static int GetBoundingSquareSideLength(List<IntPoint> points)
        {
            int minX = points.Min(o => o.X), maxX = points.Max(o => o.X);
            int minY = points.Min(o => o.Y), maxY = points.Max(o => o.Y);
            return Math.Max(maxX - minX, maxY - minY);
        }
    }
}