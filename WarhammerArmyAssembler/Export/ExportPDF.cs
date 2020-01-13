using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WarhammerArmyAssembler
{
    class ExportPDF
    {
        const int MARGIN_TOP = 30;
        const int MARGIN_LEFT = 45;

        const int LINE_HEIGHT = 13;
        static float CURRENT_Y = MARGIN_TOP;

        static Document document;
        static PdfContentByte cb;

        public static string SaveArmyToPDF()
        {
            string fileName = "test_filename.pdf";

            document = new Document();
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            cb = writer.DirectContent;
            cb.SetColorFill(BaseColor.BLACK);

            AddText(String.Format("{0} // warhammer fantasy battles", Army.ArmyName), fontSize: 20, lineHeight: 18, leftColumn: true);
            AddText(String.Format("{0} pts", Army.MaxPoints), fontSize: 12, lineHeight: 22, leftColumn: true);
            AddText();

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
            {
                AddText(String.Format("{0}", entry.Value.Size), leftColumn: true, newLine: false);
                AddText(String.Format("{0} ({1} pts)",  entry.Value.Name, entry.Value.GetUnitPoints()), lineHeight: 10);

                string[] specialRules = InterfaceOther.WordSplit(entry.Value.GetSpecialRulesLine(), partLength: 210);
                foreach(string specialRule in specialRules)
                    AddText(specialRule, fontSize: 6, lineHeight: 8);

                AddText(lineHeight: 16);
            }

            AddText(lineHeight: 20);

            AddText(
                String.Format(
                    "Points: {0} / Models: {0} / Cast: {0} / Dispell: {0}",
                    ArmyParams.GetArmyPoints(), ArmyParams.GetArmySize(), ArmyParams.GetArmyCast(), ArmyParams.GetArmyDispell()
                ),
                fontSize: 12, lineHeight: 18, leftColumn: true
            );

            document.Close();
            fs.Close();
            writer.Close();

            System.Diagnostics.Process.Start(fileName);

            return String.Empty;
        }

        static public void AddLine(float x1, float y1, float x2, float y2)
        {
            cb.SetLineWidth(0.4);
            cb.MoveTo(x1, document.PageSize.Height - y1);
            cb.LineTo(x2, document.PageSize.Height - y2);
            cb.Stroke();
        }

        static public void AddText(string text = "", float? x = null, float? y = null, int aligment = 0,
            float fontSize = 14, float lineHeight = 13, bool leftColumn = false, bool newLine = true)
        {
            BaseFont bf = BaseFont.CreateFont("FONT.TTF", Encoding.GetEncoding(1251).BodyName, BaseFont.NOT_EMBEDDED);
            cb.SetFontAndSize(bf, fontSize);

            float yPos = y ?? MARGIN_TOP + CURRENT_Y;
            float xPos = x ?? MARGIN_LEFT;

            cb.BeginText();
            cb.ShowTextAligned(aligment, text, xPos + (leftColumn ? 0 : 20), (document.PageSize.Height - yPos), 0);
            cb.EndText();

            if (newLine)
                CURRENT_Y += lineHeight;
        }
    }
}
