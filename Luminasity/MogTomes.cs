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
                    foreach (CSBonusMission mission in csBonusmissions[bonus.Category0])
                    {
                        ProcessMogTome(mission.Content0.Value, contentFinderCondition);
                        /*if(mission.Content0.Value.ContentType.Value.ContentType.Value.RowId == 31){

                        } else {
                            Console.WriteLine(mission.Content0.Value.Content0.Value.UnlockQuest0.Value.Name);
                        }*/

                    }

                    //Console.Write(
                }
                //ProcessMogTome(map, gameData, directoryPath, fullImport);
            }
        }

        private static void ProcessMogTome(CSBonusContent content, ExcelSheet<ContentFinderCondition> contentFinderCondition)
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
                default:
                    break;
            }
        }
    }
}
