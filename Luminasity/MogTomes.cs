using Lumina;
using Lumina.Data.Files;
using Lumina.Excel.Sheets;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Lumina.Excel;

namespace Luminasity
{
    public class MogTomes
    {
        public static void ExtractMogTomes(GameData gameData, bool fullImport, bool parallel)
        {
            var csBonusmissions = gameData.GetSubrowExcelSheet<CSBonusMission>();
            var csBonusSeason = gameData.GetExcelSheet<CSBonusSeason>();
            var contentFinderCondition = gameData.GetExcelSheet<ContentFinderCondition>();
            var items = gameData.GetExcelSheet<Item>();

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "mogtomes");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            foreach (CSBonusSeason bonus in csBonusSeason)
            {
                if (bonus.Text0.Value.Text == "Moogle Treasure Trove")
                {
                    Console.WriteLine("");
                    Console.WriteLine(bonus.Text1.Value.Text);
                    Console.WriteLine("");
                    Console.WriteLine("Standard/Weekly Objectives: ");
                    foreach (CSBonusMission mission in csBonusmissions[bonus.Category0])
                    {
                        ProcessMogTome(mission.Content0.Value, contentFinderCondition, items);
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Minimog Objectives: ");
                    foreach (var mission in csBonusmissions[bonus.Category2].Select((mission, i) => new {i, mission}))
                    {
                        Console.WriteLine("Week " + (mission.i + 1) + ":");
                        Console.Write("Option 1 - ");
                        ProcessMogTome(mission.mission.Content0.Value, contentFinderCondition, items);
                        Console.Write("Option 2 - ");
                        ProcessMogTome(mission.mission.Content1.Value, contentFinderCondition, items);
                        //ProcessMogTome(mission.Content0.Value, contentFinderCondition);
                    }
                }
            }
        }

        private static void ProcessMogTome(CSBonusContent content, ExcelSheet<ContentFinderCondition> contentFinderCondition, ExcelSheet<Item> items)
        {
            switch (content.ContentType.Value.ContentType.Value.RowId)
            {
                // Dungeons, Trials, Raids
                case 2: case 4: case 5: case 6:
                    var meow = contentFinderCondition.First((x) => x.Content.GetValueOrDefault<InstanceContent>() != null && x.Content.RowId == content.Content0.Value.Content.RowId);
                    Console.Write(meow.Name);
                    Console.Write(": ");
                    if(content.Score1 > -1) {
                        Console.Write(" " + content.Score1 + " - " + content.RewardCount0 + " |");
                        Console.Write(" " + content.Score2 + " - " + content.RewardCount1 + " |");
                        Console.Write(" " + content.Score3 + " - " + content.RewardCount2);
                    } else {
                        Console.Write(content.RewardCount0);
                    }
                    Console.WriteLine();
                    break;
                // Ocean Fishing
                case 31:
                    Console.Write("Ocean Fishing: ");
                    Console.Write(" " + content.Score1 + " - " + content.RewardCount0 + " |");
                    Console.Write(" " + content.Score2 + " - " + content.RewardCount1 + " |");
                    Console.Write(" " + content.Score3 + " - " + content.RewardCount2);
                    Console.WriteLine();
                    break;
                // GATEs, sadly there are no clear links here so we have to do something nasty with the code
                case 35:
                    switch(content.Content0.Value.Content.RowId) {
                        case 1:
                            Console.Write("Cliffhanger: ");
                            Console.Write(" " + content.Score1 + " - " + content.RewardCount0 + " |");
                            Console.Write(" " + content.Score2 + " - " + content.RewardCount1);
                            Console.WriteLine();
                            break;
                        case 8:
                            Console.Write("Slice is Right: ");
                            Console.Write(" " + content.Score1 + " - " + content.RewardCount0 + " |");
                            Console.Write(" " + content.Score2 + " - " + content.RewardCount1 + " |");
                            Console.Write(" " + content.Score3 + " - " + content.RewardCount2 + " |");
                            Console.Write(" " + content.Score4 + " - " + content.RewardCount3 + " |");
                            Console.Write(" " + content.Score5 + " - " + content.RewardCount4);
                            Console.WriteLine();
                            break;
                        case 5:
                            Console.Write("Any Way the Wind Blows: ");
                            Console.Write(" " + content.Score1 + " - " + content.RewardCount0 + " |");
                            Console.Write(" " + content.Score2 + " - " + content.RewardCount1 + " |");
                            Console.Write(" " + content.Score3 + " - " + content.RewardCount2);
                            Console.WriteLine();
                            break;
                        case 6: case 7:
                            Console.Write("Leap of Faith or Airforce One?: ");
                            Console.Write(" " + content.Score1 + " - " + content.RewardCount0 + " |");
                            Console.Write(" " + content.Score2 + " - " + content.RewardCount1 + " |");
                            Console.Write(" " + content.Score3 + " - " + content.RewardCount2);
                            Console.WriteLine();
                            break;
                        default:
                            break;
                    }
                    break;
                case 8:
                    Console.WriteLine("FATEs");
                    break;
                case 36:
                    Console.WriteLine("Island Sanctuary");
                    break;
                case 27:
                    Console.WriteLine("Masked Carnivale");
                    break;
                case 34:
                    var fishItem = content.Content0.Value.Content.GetValueOrDefault<FishParameter>()?.Item.RowId;
                    if(fishItem != null){
                        var fishName = items.GetRow(fishItem ?? 0);
                        Console.Write("Catch Fish: " + fishName.Name);
                        Console.WriteLine();
                    }
                    break;
                case 32:
                    Console.WriteLine("Triple Triad");
                    break;
                case 9:
                    Console.WriteLine("Treasure Hunt");
                    break;
                case 26:
                    Console.WriteLine("Eureka");
                    break;
                case 29:
                    Console.WriteLine("Save the Queen");
                    break;
                case 33:
                    Console.WriteLine("The Hunt");
                    break;
                default:
                    break;
            }
        }
    }
}
