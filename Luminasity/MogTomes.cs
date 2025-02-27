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
                // Dungeons
                case 2:
                    var meow = contentFinderCondition.First((x) => x.Content.GetValueOrDefault<InstanceContent>() != null && x.Content.RowId == content.Content0.Value.Content.RowId);
                    Console.Write(meow.Name);
                    Console.Write(": ");
                    Console.Write(content.RewardCount0);
                    Console.WriteLine();
                    break;
                default:
                    break;
            }
        }
    }
}
