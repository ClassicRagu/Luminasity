using Luminasity;

class Program
{
	static void Main(string[] args)
	{
		bool endApp = false;

		if(args.Length < 1)
		{
			Console.WriteLine("Please include a file path");
			return;
		}

		string xivPath = args[0];

		var lumina = new Lumina.GameData(xivPath, new() { DefaultExcelLanguage = Lumina.Data.Language.English });
		bool fullMode = false;

		while (!endApp)
		{
			Console.Write("> ");
			string input = Console.ReadLine();

			switch(input.ToLowerInvariant())
			{
				case "end":
					endApp = true;
					break;
				case "maps":
					Console.WriteLine("Please be aware that map extraction could take a while due to image processing");
					Maps.ExtractMaps(lumina, fullMode);
					break;
				case "icons":
					Console.WriteLine("Please be aware that icon extraction could take a while due to number of images");
					Icons.ExtractIcons(1, 250000, lumina, fullMode);
					break;
				case "full":
					fullMode = !fullMode;
					Console.WriteLine(fullMode ? "Full Export Mode Enabled" : "Full Export Mode Disabled");
					break;
				default:
					break;
			}
		}
	}
}
