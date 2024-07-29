using Lumina;
using Lumina.Data.Files;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Luminasity
{
	// All code from https://github.com/TomestoneGG/Lumenstone
	public class Icons
	{
		private const string IconFileFormat = "ui/icon/{0:D3}000/{1}{2:D6}.tex";
		private const string IconHDFileFormat = "ui/icon/{0:D3}000/{1}{2:D6}_hr1.tex";
		public static void ExtractIcons(int first, int last, GameData gameData, bool fullImport)
		{
			string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "icons");

			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			for (int i = first; i <= last; ++i)
			{
				var iconFilePath = Path.Combine(directoryPath, $"{i / 1000:D3}000", $"{i:D6}.png");
				if (fullImport || !File.Exists(iconFilePath))
				{
					var icon = GetIcon(gameData, "en/", i, false);
					if (icon != null && icon != default(TexFile))
					{
						Console.WriteLine($"-> {i:D6}");
						var folderPath = Path.Combine(directoryPath, $"{i / 1000:D3}000");
						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(folderPath);
						}

						var image = Image.LoadPixelData<Bgra32>(icon.ImageData, icon.Header.Width, icon.Header.Height);

						image.Save(iconFilePath);
					}
				}

				var iconHDFilePath = Path.Combine(directoryPath, $"{i / 1000:D3}000", $"{i:D6}_hr1.png");
				if (fullImport || !File.Exists(iconHDFilePath))
				{
					var iconHD = GetIcon(gameData, "en/", i, true);
					if (iconHD != null && iconHD != default(TexFile))
					{
						Console.WriteLine($"-> HD {i:D6}");
						var folderPath = Path.Combine(directoryPath, $"{i / 1000:D3}000");
						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(folderPath);
						}

						var image = Image.LoadPixelData<Bgra32>(iconHD.ImageData, iconHD.Header.Width, iconHD.Header.Height);

						image.Save(iconHDFilePath);
					}
				}
			}
		}

		private static TexFile? GetIcon(GameData gameData, string type, int iconId, bool hd)
		{
			type ??= string.Empty;
			if (type.Length > 0 && !type.EndsWith("/"))
				type += "/";

			var filePath = string.Format(hd ? IconHDFileFormat : IconFileFormat, iconId / 1000, type, iconId);
			try
			{
				var file = gameData.GetFile<TexFile>(filePath);

				if (file != default(TexFile) || type.Length <= 0) return file;

				// Couldn't get specific type, try for generic version.
				filePath = string.Format(hd ? IconHDFileFormat : IconFileFormat, iconId / 1000, string.Empty, iconId);
				file = gameData.GetFile<TexFile>(filePath);
				return file;
			}
			catch (FileNotFoundException)
			{
				return null;
			}
		}
	}
}
