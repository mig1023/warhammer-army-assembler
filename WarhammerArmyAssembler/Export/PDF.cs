using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Linq;

namespace WarhammerArmyAssembler.Export
{
    class PDF
    {
        const int MARGIN_TOP = 30;
        const int MARGIN_LEFT = 45;

        static float unitSizeWidth = 20;
        static Document document = null;
        static PdfContentByte cb = null;

        static float currentY;

        public static string SaveArmy(bool fullRules = false)
        {
            string fileName = Services.GetFileName("pdf");

            currentY = MARGIN_TOP;

            unitSizeWidth = CountMaxUnitSizeWidth();

            document = new Document();
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            cb = writer.DirectContent;
            cb.SetColorFill(BaseColor.BLACK);

            AddText(Services.AllArmyName(), fontSize: 20, lineHeight: 18, leftColumn: true);
            AddText(Services.AllArmyPointsAndEdition(), fontSize: 12, lineHeight: 22, leftColumn: true);
            AddText();

            List<Unit> armyByCategories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in armyByCategories)
            {
                foreach (Unit unit in unitType.Items)
                {
                    AddText($"{Services.UnitSizeIfNeed(unit)}", leftColumn: true, newLine: false);
                    AddText($"{Services.GetUnitName(unit)}{Services.UnitPointsLine(unit)}", lineHeight: 10);

                    List<string> linesForEachUnit = new List<string> { unit.GetEquipmentLine() };

                    if (fullRules)
                    {
                        linesForEachUnit.Add(unit.GetWizardLevelLine());
                        linesForEachUnit.Add(unit.GetSpecialRulesLine(withoutWizards: true));
                        linesForEachUnit.Add(unit.GetModifiedParamsLine());
                    }

                    foreach (string param in linesForEachUnit)
                    {
                        List<string> lines = Interface.Services
                            .WordSplit(param, partLength: 210)
                            .Where(x => !String.IsNullOrEmpty(x))
                            .ToList();

                        foreach (string line in lines)
                            AddText(line, fontSize: 6, lineHeight: 8);
                    }

                    AddText(lineHeight: 16);
                }
            }

            AddText(lineHeight: 8);

            double points = Army.Params.GetArmyPoints();
            double size = Army.Params.GetArmySize();
            double cast = Army.Params.GetArmyCast();
            double dispell = Army.Params.GetArmyDispell();

            AddText($"Points: {points} / Models: {size} / Cast: {cast} / Dispell: {dispell}",
                fontSize: 12, lineHeight: 18, leftColumn: true);

            document.Close();
            fs.Close();
            writer.Close();

            System.Diagnostics.Process.Start(fileName);

            return String.Empty;
        }

        static private float CountMaxUnitSizeWidth()
        {
            int maxLength = 0;

            foreach (Unit entry in Army.Data.Units.Values)
            {
                int unitSizeLen = Services.UnitSizeIfNeed(entry).Length;

                if (maxLength < unitSizeLen)
                    maxLength = unitSizeLen;
            }

            return 20 + (maxLength <= 3 ? 0 : maxLength * 4);
        }

        static private void AddText(string text = "", float? x = null,
            float? y = null, int aligment = 0, float fontSize = 14,
            float lineHeight = 13, bool leftColumn = false, bool newLine = true)
        {
            string fontBodyName = Encoding.GetEncoding(1251).BodyName;
            BaseFont bf = BaseFont.CreateFont("FONT.TTF", fontBodyName, BaseFont.NOT_EMBEDDED);
            cb.SetFontAndSize(bf, fontSize);

            float yPos = y ?? MARGIN_TOP + currentY;
            float xPos = x ?? MARGIN_LEFT;

            cb.BeginText();

            float xPosFixed = xPos + (leftColumn ? 0 : unitSizeWidth);
            float yPosFixed = document.PageSize.Height - yPos;
            cb.ShowTextAligned(aligment, text, xPosFixed, yPosFixed, 0);

            cb.EndText();

            if (newLine)
            {
                currentY += lineHeight;

                if ((document.PageSize.Height - currentY - (MARGIN_TOP * 2)) < 0)
                {
                    document.NewPage();
                    currentY = MARGIN_TOP;
                }
            }
        }
    }
}
