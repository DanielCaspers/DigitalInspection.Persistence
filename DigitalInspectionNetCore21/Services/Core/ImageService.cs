using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;
using Image = DigitalInspectionNetCore21.Models.Inspections.Image;

namespace DigitalInspectionNetCore21.Services.Core
{
	// FIXME DJC This is  done differently here, revisit
	public static class ImageService
	{
		private static readonly string UPLOAD_DIR = "~/Uploads/";

		public static Image SaveImage(IFormFile picture, string uploadSubdirectory, string fileNamePrefix, bool compress = true)
		{
			// TODO Improve error handling and prevent NPEs
			if (picture != null && picture.Length > 0)
			{
				var imageDirectoryPath = CreateFolderTree(UPLOAD_DIR, new[] { uploadSubdirectory });
				return SaveImageInternal(picture, imageDirectoryPath, fileNamePrefix, compress);
			}

			return null;
		}

		// uploadDirectoryTree represents a route tree, like the Angular router. 
		public static Image SaveImage(IFormFile picture, string[] uploadDirectoryTree, string fileNamePrefix, bool compress = true)
		{
			// TODO Improve error handling and prevent NPEs
			if (picture != null && picture.Length > 0)
			{
				var imageDirectoryPath = CreateFolderTree(UPLOAD_DIR, uploadDirectoryTree);
				return SaveImageInternal(picture, imageDirectoryPath, fileNamePrefix, compress);
			}

			return null;
		}

		public static void DeleteImage(Image picture)
		{
			if (picture != null && File.Exists(picture.ImageUrl))
			{
				try
				{
					File.Delete(picture.ImageUrl);
				}
				catch (IOException e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}

		private static string CreateFolderTree(string baseDir, string[] dirTree)
		{
			// Starts with already created directory, and incrementally builds to solution
			// FIXME DJC Needs to inject IHostingEnvironment, which comes from controller
			// https://www.mikesdotnetting.com/article/302/server-mappath-equivalent-in-asp-net-core
			string folderPath = ""; // HttpContext.WebRootPath(UPLOAD_DIR);
			foreach (string folderToCreate in dirTree)
			{
				folderPath = Path.Combine(folderPath, folderToCreate);
				DirectoryInfo di = Directory.CreateDirectory(folderPath); // No folders are created if they already exist
																		  // SetPathAccessControl(folderPath);
			}

			return folderPath;
		}

		private static Image SaveImageInternal(IFormFile picture, string imageDirectoryPath, string fileNamePrefix, bool compress)
		{
			var imageFileName = fileNamePrefix + "_" + picture.FileName;
			var imagePath = Path.Combine(imageDirectoryPath, imageFileName);

			// TODO: Allow users of function to pass in image quality, or decide whether or not to save at some setting. Maybe custom enum model for image quality?
			if (compress)
			{
				throw new NotImplementedException();
				//image = ReduceImageSize(image, 240, 320);
			}

			using (Stream stream = picture.OpenReadStream())
			{
				System.Drawing.Image image = System.Drawing.Image.FromStream(stream, true, true);
				image.Save(imagePath);
			}

			return new Image
			{
				Title = imageFileName,
				ImageUrl = imagePath
			};
		}

		//// http://stackoverflow.com/a/21394605/2831961
		//private static Bitmap ReduceImageSize(System.Drawing.Image image, int height, int width)
		//{
		//	Bitmap newImg = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
		//	Graphics newGraphic = Graphics.FromImage(newImg);

		//	newGraphic.InterpolationMode = InterpolationMode.Bicubic;
		//	newGraphic.DrawImage(image, 0, 0, width, height);
		//	newGraphic.Dispose();

		//	return newImg;
		//}


	}
}