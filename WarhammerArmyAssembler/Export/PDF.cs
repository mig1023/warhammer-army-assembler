using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WarhammerArmyAssembler.Export
{
    class PDF
    {
        const int MARGIN_TOP = 30;
        const int MARGIN_LEFT = 45;
        
        static Document document;
        static PdfContentByte cb;

        static float currentY;

        public static string SaveArmy(bool fullRules = false)
        {
            string fileName = Export.Other.GetFileName("pdf");

            currentY = MARGIN_TOP;

            document = new Document();
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            cb = writer.DirectContent;
            cb.SetColorFill(BaseColor.BLACK);

            AddText(Export.Other.AllArmyName(), fontSize: 20, lineHeight: 18, leftColumn: true);
            AddText(Export.Other.AllArmyPointsAndEdition(), fontSize: 12, lineHeight: 22, leftColumn: true);
            AddText();

            List<Unit> armyByCategories = Army.Params.GetArmyUnitsByCategories();

            foreach (Unit unitType in armyByCategories)
                foreach (Unit unit in unitType.Items)
                {
                    AddText(String.Format("{0}", Export.Other.UnitSizeIfNeed(unit)), leftColumn: true, newLine: false);
                    AddText(String.Format("{0}{1}", unit.Name, Export.Other.UnitPointsLine(unit)), lineHeight: 10);

                    List<string> linesForEachUnit = new List<string> { unit.GetEquipmentLine() };

                    if (fullRules)
                    {
                        linesForEachUnit.Add(unit.GetSpecialRulesLine());
                        linesForEachUnit.Add(unit.GetModifiedParamsLine());
                    }

                    foreach (string param in linesForEachUnit)
                        foreach (string line in Interface.Other.WordSplit(param, partLength: 210))
                            if (!String.IsNullOrEmpty(line))
                                AddText(line, fontSize: 6, lineHeight: 8);

                    AddText(lineHeight: 16);
                }

            AddText(lineHeight: 8);

            AddText(
                String.Format(
                    "Points: {0} / Models: {1} / Cast: {2} / Dispell: {3}",
                    Army.Params.GetArmyPoints(), Army.Params.GetArmySize(), Army.Params.GetArmyCast(), Army.Params.GetArmyDispell()
                ),
                fontSize: 12, lineHeight: 18, leftColumn: true
            );

            document.Close();
            fs.Close();
            writer.Close();

            System.Diagnostics.Process.Start(fileName);

            return String.Empty;
        }

        static public void AddText(string text = "", float? x = null, float? y = null, int aligment = 0,
            float fontSize = 14, float lineHeight = 13, bool leftColumn = false, bool newLine = true)
        {
            BaseFont bf = BaseFont.CreateFont("FONT.TTF", Encoding.GetEncoding(1251).BodyName, BaseFont.NOT_EMBEDDED);
            cb.SetFontAndSize(bf, fontSize);

            float yPos = y ?? MARGIN_TOP + currentY;
            float xPos = x ?? MARGIN_LEFT;

            cb.BeginText();
            cb.ShowTextAligned(aligment, text, xPos + (leftColumn ? 0 : 20), (document.PageSize.Height - yPos), 0);
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
