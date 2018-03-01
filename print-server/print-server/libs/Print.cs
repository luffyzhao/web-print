using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using O2S.Components.PDFRender4NET;
using O2S.Components.PDFRender4NET.Printing;

namespace Libs
{
    class Print
    {
        private String printName;
        private int rawKind = 9;

        private String filename;
        private String type;
        public Print(String printName, int rawKind)
        {
            this.printName = printName;
            this.rawKind = rawKind;
        }

        public Print()
        {
            PrintDocument print = new PrintDocument();

            this.printName = print.PrinterSettings.PrinterName;
        }

        public void set(String filename, String type)
        {
            this.filename = filename;
            this.type = type;
        }

        public void run()
        {
            switch (this.type)
            {
                case "pdf":
                    this.pdf(this.filename);
                    break;
                case "html":
                default:
                    this.html(this.filename);
                    break;
            }
        } 

        private bool html(String filename)
        {
            // WebKitBrowser browser = new WebKitBrowser();
            // browser.Url = new Uri(filename);
            // browser.ShowPrintDialog();
            // browser.Navigate(filename);
            return true;
        }

        private bool pdf(String filename)
        {
            PDFFile file = PDFFile.Open(filename);
            try
            {
                file.Print(this.PrinterSettings());
            }
            catch
            {
                return false;
            }
            finally
            {

                file.Dispose();
            }

            return true;
        }

        private PDFPrintSettings PrinterSettings()
        {
            PrinterSettings settings = new PrinterSettings();
            //PrintDocument pd = new PrintDocument();
            settings.PrinterName = this.printName;
            settings.PrintToFile = false;

            //设置纸张大小（可以不设置，取默认设置）3.90 in,  8.65 in
            PaperSize ps = new PaperSize();
            //如果是自定义纸张，就要大于118，（A4值为9，详细纸张类型与值的对照请看http://msdn.microsoft.com/zh-tw/library/system.drawing.printing.papersize.rawkind(v=vs.85).aspx）
            ps.RawKind = this.rawKind;

            PDFPrintSettings pdfPrintSettings = new PDFPrintSettings(settings);
            pdfPrintSettings.PaperSize = ps;
            pdfPrintSettings.PageScaling = PageScaling.FitToPrinterMarginsProportional;
            pdfPrintSettings.PrinterSettings.Copies = 1;

            return pdfPrintSettings;
        }
    }
}
