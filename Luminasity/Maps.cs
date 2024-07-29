using Lumina;
using Lumina.Data.Files;
using Lumina.Excel.GeneratedSheets2;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Lumina.Text;

namespace Luminasity
{
	public class Maps
	{
		// Modified from https://github.com/TomestoneGG/Lumenstone
		public static void ExtractMaps(GameData gameData, bool fullImport)
		{
			var maps = gameData.GetExcelSheet<Map>();

			string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "maps");

			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			foreach (var map in maps)
			{
				var idProperty = typeof(Map).GetProperty("Id");
				if (idProperty == null || idProperty.PropertyType != typeof(SeString))
					continue;
				var idValue = idProperty.GetValue(map);
				if (idValue == null)
					continue;
				var idString = ((SeString)idValue).RawString;

				// Assuming idString is in the format "xxx/yyy"
				string[] parts = idString.Split('/');
				if (parts.Length != 2)
					continue; // Ensure idString is in the expected format


				var outputFilePath = Path.Combine(directoryPath, parts[0] + "/" + parts[0] + "." + parts[1] + ".png");

				if (fullImport || !File.Exists(outputFilePath))
				{
					// Directly concatenate "ui" and "maps" with the rest of the path
					string filePath = "ui/map/" + idString + "/" + parts[0] + parts[1] + "_m.tex";
					string maskPath = "ui/map/" + idString + "/" + parts[0] + parts[1] + "m_m.tex";

					if (!gameData.FileExists(filePath))
						filePath = "ui/map/" + idString + "/" + parts[0] + parts[1] + "m_m.tex";
					if (!gameData.FileExists(filePath))
						continue;

					Console.WriteLine("Extracting data for map: " + idString);

					var diffuseFile = gameData.GetFile<TexFile>(filePath);
					var maskFile = gameData.FileExists(maskPath) ? gameData.GetFile<TexFile>(maskPath) : null;

					var folderPath = Path.Combine(directoryPath, parts[0]);
					if (!Directory.Exists(folderPath))
					{
						Directory.CreateDirectory(folderPath);
					}

					if (maskFile != null && filePath != maskPath)
					{
						var image = BlendImage(diffuseFile, maskFile);
						image.SaveAsPng(outputFilePath);
					}
					else
					{
						var image = Image.LoadPixelData<Bgra32>(diffuseFile.ImageData, diffuseFile.Header.Width, diffuseFile.Header.Height);
						image.SaveAsPng(outputFilePath);
					}
				}
			}
		}

		private static Image<Bgra32> BlendImage(TexFile diffuse, TexFile mask)
		{
			// Create a new image from the raw pixel data
			var diffuseImage = Image.LoadPixelData<Bgra32>(diffuse.ImageData, diffuse.Header.Width, diffuse.Header.Height);
			var maskImage = Image.LoadPixelData<Bgra32>(mask.ImageData, mask.Header.Width, mask.Header.Height);

			// Blend Images
			diffuseImage.Mutate(x => x.DrawImage(maskImage, PixelColorBlendingMode.Multiply, PixelAlphaCompositionMode.SrcAtop, 1));

			return diffuseImage;
		}
	}
}
