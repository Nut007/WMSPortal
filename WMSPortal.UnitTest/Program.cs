using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WMSPortal.Core.Model;
using WMSPortal.Data;
using WMSPortal.Data.Repositories;

namespace WMSPortal.UnitTest
{

    class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [STAThread]
        static void Main(string[] args)
        {
            TestReportRepository rep= new TestReportRepository();
            ExcelReportRepository excel = new ExcelReportRepository();
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            // hide the console window                   
            //setConsoleWindowVisibility(false, Console.Title);
            try
            {

                // Change to your number of menuitems.
                const int maxMenuItems = 5;
                int selector = 0;
                bool good = false;
                while (selector != maxMenuItems)
                {
                    Console.Clear();
                    DrawTitle();
                    DrawMenu(maxMenuItems);
                    good = int.TryParse(Console.ReadLine(), out selector);
                    if (good)
                    {
                        switch (selector)
                        {
                            case 1:
                                Console.WriteLine("Processing ...");
                                IEnumerable<ImportDeclarationReport> items = rep.GetImportDeclaraion("20160720", "20160725", "TMT");
                                excel.GenerateImportDeclarationReport(items);
                                break;
                            case 2:
                                Console.WriteLine("Processing ...");
                                IEnumerable<ExportDeclarationReport> exps = rep.GetExportDeclaraion("20160701", "20160721", "TMT");
                                excel.GenerateExportDeclarationReport(exps);
                                break;
                            case 3:
                                Console.WriteLine("Processing ...");
                                IEnumerable<GLDeclarationReport> gl = rep.GetGLDeclaration("20160710", "20160715", "TMT");
                                excel.GenerateGLDeclarationReport(gl);
                                break;
                            case 4:
                                Console.WriteLine("Processing ...");
                                IEnumerable<GLDeclarationReport> inv = rep.GetInventoryDeclaration("TMT");
                                excel.GenerateInventoryDeclarationReport(inv);
                                break;
                            case 5:
                                Console.WriteLine("Processing ...");
                                InventoryMovementReport trans = rep.GetMovementTransection("20160721", "20160729", "TMT");
                                excel.GenerateMovementReport(trans);
                                break;
                            case 6:
                                Environment.Exit(0);
                                break;
                            // possibly more cases here
                            default:
                                if (selector != maxMenuItems)
                                {
                                    ErrorMessage();
                                }
                                break;
                        }
                    }
                    else
                    {
                        ErrorMessage();
                    }
                    Console.ReadKey();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
            finally
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        private static void ErrorMessage()
        {
            Console.WriteLine("Typing error, press key to continue.");
        }
        private static void DrawStarLine()
        {
            Console.WriteLine("************************");
        }
        private static void DrawTitle()
        {
            DrawStarLine();
            Console.WriteLine("+++   BOUND REPORT  +++");
            DrawStarLine();
        }
        private static void DrawMenu(int maxitems)
        {
            DrawStarLine();
            Console.WriteLine(" 1. Import Declaration");
            Console.WriteLine(" 2. Export Declaration");
            Console.WriteLine(" 3. GL Declaration Trans");
            Console.WriteLine(" 4. Inventory Declaration Trans");
            Console.WriteLine(" 5. Inventory Trans");
            Console.WriteLine(" 6. Exit program");
            DrawStarLine();
            Console.WriteLine("Make your choice: type 1, 2, ... or {0} for exit", maxitems);
            DrawStarLine();
        }


    }
}
