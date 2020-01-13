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
        const int MARGIN_TOP = 15;
        const int MARGIN_LEFT = 30;
        const int LINE_HEIGHT = 13;

        static int[] COLUMN_WIDTH = { 0, 20, 250, 50, 60, 75 };
        static int CURRENT_LINE = 0;

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

            BaseFont bf = BaseFont.CreateFont("FONT.TTF", Encoding.GetEncoding(1251).BodyName, BaseFont.NOT_EMBEDDED);

            cb.SetColorFill(BaseColor.BLACK);
            cb.SetFontAndSize(bf, 10);


            AddText(String.Format("Armylist {0} {1}", Army.ArmyName, Army.MaxPoints));
            AddText("=========================================");

            AddText();

            foreach (KeyValuePair<int, Unit> entry in Army.Units)
                AddText(String.Format("{0} {1}", entry.Value.Name, entry.Value.PointsView));

            document.Close();
            fs.Close();
            writer.Close();

            System.Diagnostics.Process.Start(fileName);

            return String.Empty;
        }

        static public float CurrentY()
        {
            return MARGIN_TOP + CURRENT_LINE * LINE_HEIGHT;
        }

        static public void AddLine(float x1, float y1, float x2, float y2)
        {
            cb.SetLineWidth(0.4);
            cb.MoveTo(x1, document.PageSize.Height - y1);
            cb.LineTo(x2, document.PageSize.Height - y2);
            cb.Stroke();
        }

        static public void AddText(string text = "", float x = 0, float y = 0, int aligment = 0,
            bool noNewLine = false, bool withBox = false, int column = -1, bool header = false)
        {
            if (!noNewLine && column < 0) CURRENT_LINE += 1;

            float yPos = (y == 0 ? CurrentY() : y);
            float xPos = (x == 0 ? MARGIN_LEFT : x);

            if (column == -1 && header) xPos -= 4;

            if (column >= 0)
            {
                xPos += (header ? 15 : 10);

                for (int a = 0; a <= column; a++)
                    xPos += COLUMN_WIDTH[a];

                if (header && column >= 0 && (column + 1 < COLUMN_WIDTH.Length))
                    xPos += (COLUMN_WIDTH[column + 1] / 2) - 10;
                else if (header && column + 1 == COLUMN_WIDTH.Length)
                    xPos = (document.PageSize.Width - (COLUMN_WIDTH[column] / 2) - MARGIN_LEFT);
            }

            if (withBox) xPos += 10;

            cb.BeginText();
            cb.ShowTextAligned(aligment, text, xPos, (document.PageSize.Height - yPos), 0);
            cb.EndText();
        }
    }
}
