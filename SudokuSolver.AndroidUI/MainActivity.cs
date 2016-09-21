using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace SudokuSolver.AndroidUI
{
    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }

    [Activity(Label = "Sudoku Solver", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ImageView _imageView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                var button = FindViewById<Button>(Resource.Id.TakeSudokuBoardPictureButton);
                _imageView = FindViewById<ImageView>(Resource.Id.PhotoPreview);
                button.Click += TakeAPicture;
            }
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "CameraAppDemo");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            var availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            var intent = new Intent(MediaStore.ActionImageCapture);

            App._file = new File(App._dir, string.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));

            StartActivityForResult(intent, 0);
        }
    }
}