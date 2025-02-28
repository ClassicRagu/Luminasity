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
            var tripleTriadCardResidents = gameData.GetExcelSheet<TripleTriadCardResident>();

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
                        ProcessMogTome(mission.Content0.Value, contentFinderCondition, items, tripleTriadCardResidents);
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Minimog Objectives: ");
                    foreach (var mission in csBonusmissions[bonus.Category2].Select((mission, i) => new {i, mission}))
                    {
                        Console.WriteLine("Week " + (mission.i + 1) + ":");
                        Console.Write("Option 1 - ");
                        ProcessMogTome(mission.mission.Content0.Value, contentFinderCondition, items, tripleTriadCardResidents);
                        Console.Write("Option 2 - ");
                        ProcessMogTome(mission.mission.Content1.Value, contentFinderCondition, items, tripleTriadCardResidents);
                        //ProcessMogTome(mission.Content0.Value, contentFinderCondition);
                    }

                    Console.WriteLine("");
                    Console.WriteLine("Ultimog Challenge: ");
                    ProcessMogTome(csBonusmissions[bonus.Category3][0].Content0.Value, contentFinderCondition, items, tripleTriadCardResidents);
                }
            }
        }

        private static void ProcessMogTome(CSBonusContent content, ExcelSheet<ContentFinderCondition> contentFinderCondition, ExcelSheet<Item> items, ExcelSheet<TripleTriadCardResident> tripleTriadCardResidents)
        {
            switch (content.ContentType.RowId)
            {
                // Dungeons, Trials, Raids
                case 1: case 2: case 3: case 4:
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
                case 6:
                    Console.Write("Ocean Fishing: ");
                    Console.Write(" " + content.Score1 + " - " + content.RewardCount0 + " |");
                    Console.Write(" " + content.Score2 + " - " + content.RewardCount1 + " |");
                    Console.Write(" " + content.Score3 + " - " + content.RewardCount2);
                    Console.WriteLine();
                    break;
                // GATEs, sadly there are no clear links here so we have to do something nasty with the code
                case 11:
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
                case 12:
                    var territory1 = content.Content0.Value.Content.GetValueOrDefault<TerritoryType>()?.PlaceName.Value.Name;
                    var territory2 = content.Content1.Value.Content.GetValueOrDefault<TerritoryType>()?.PlaceName.Value.Name;
                    Console.WriteLine("Complete " + content.Score1 + " FATEs in " + territory1 + " or " + territory2 + ".");
                    break;
                case 18:
                    Console.WriteLine("Gather materials " + content.Score1 + " times in Island Sanctuary");
                    break;
                case 17:
                    Console.WriteLine("Complete a stage of the Masked Carnivale");
                    break;
                case 10:
                    var fishItem = content.Content0.Value.Content.GetValueOrDefault<FishParameter>()?.Item.RowId;
                    if(fishItem != null){
                        var fishName = items.GetRow(fishItem ?? 0);
                        Console.Write("Catch " + fishName.Name);
                        Console.WriteLine();
                    }
                    break;
                case 7:
                    var ttNPC = content.Content0.Value.Content.GetValueOrDefault<ENpcResident>()?.Singular;
                    var ttNPCLocal = tripleTriadCardResidents.First((x) => x.AcquisitionType == 6 && x.Acquisition.RowId == content.Content0.Value.Content.GetValueOrDefault<ENpcResident>()?.RowId);
                    Console.WriteLine("Win a game of Triple Triad against " + ttNPC +  " in " + ttNPCLocal.Location.GetValueOrDefault<Level>().Value.Map.Value.PlaceName.Value.Name + ".");
                    break;
                case 8:
                    Console.WriteLine("Decipher a timeworn map and collect the treasure.");
                    break;
                case 15:
                    Console.WriteLine("Defeat " + content.Score1 + " notorious monster(s) in the Forbidden Land, Eureka");
                    break;
                case 16:
                    Console.WriteLine("Complete " + content.Score1 + " critical engagement(s) on the Bozjan southern front or in Zadnor");
                    break;
                case 9:
                    var huntName = content.Content0.Value.Content.GetValueOrDefault<MobHuntOrderType>()?.EventItem.Value.Name;
                    Console.WriteLine("Complete an " + huntName + ".");
                    break;
                default:
                    Console.WriteLine("Unknown");
                    break;
            }
        }
    }
}
