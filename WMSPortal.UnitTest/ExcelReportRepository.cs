using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;
using System.Drawing;

namespace WMSPortal.UnitTest
{
    public class ExcelReportRepository
    {
        public void GenerateImportDeclarationReport(IEnumerable<ImportDeclarationReport> items)
        {
            int rowIndex = 1;
            int rowDecl = 1;
            string currentReceiptKey = string.Empty;

            ExcelPackage pck = new ExcelPackage();
            var wsEnum = pck.Workbook.Worksheets.Add("ImportDeclaration14");

            //Add the headers
            //Solid black border around the board.
            wsEnum.Cells[string.Format("A{0}:K{1}", 1, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
            wsEnum.Cells[string.Format("A{0}:K{1}", 1, 1)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:K{1}", 2, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[2, 1].Value = "คทบ ๑๔";
            wsEnum.Cells[string.Format("A{0}:K{1}", 2, 2)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:K{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[3, 1].Value = "คลังสินค้าทัณฑ์บนทั่วไป บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396";
            wsEnum.Cells[string.Format("A{0}:K{1}", 3, 3)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:K{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[4, 1].Value = "รายงานของที่นำเข้าเก็บในคลังสินค้าทัณฑ์บนฯ";
            wsEnum.Cells[string.Format("A{0}:K{1}", 4, 4)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[5, 1].Value = "นำเข้าจากต่างประเทศ";
            wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:K{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[6, 1].Value = "สำหรับงวดเดือน";
            wsEnum.Cells[string.Format("A{0}:K{1}", 6, 6)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 6, 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells[string.Format("A{0}:K{1}", 7, 7)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

            rowIndex = 8;

            wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
            wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำเข้าคลัง";
            wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
            wsEnum.Cells[rowIndex, 4].Value = "ชื่อผู้นำเข้า";
            wsEnum.Cells[rowIndex, 5].Value = "วันที่นำเข้า";
            wsEnum.Cells[rowIndex, 6].Value = "วันที่นำเข้าคลัง";
            wsEnum.Cells[rowIndex, 7].Value = "ชื่อของ";
            wsEnum.Cells[rowIndex, 8].Value = "ปริมาณ";
            wsEnum.Cells[rowIndex, 9].Value = "น้ำหนัก";
            wsEnum.Cells[rowIndex, 10].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex, 11].Value = "ภาษีอากรรวม (บาท)";

            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

            rowIndex++;
            var imps = items.GroupBy(
                                  i => i.ReceiptKey,
                                  (key, group) => group.First()
                              ).ToArray();




            foreach (var imp in imps)
            {
                currentReceiptKey = imp.ReceiptKey;
                IEnumerable<ImportDeclarationReport> impds = items.Where(x => x.ReceiptKey == imp.ReceiptKey);
                foreach (ImportDeclarationReport impd in impds)
                {
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = impd.ImportDeclarationNo;
                    wsEnum.Cells[rowIndex, 3].Value = impd.ImportDeclarationItemNo;
                    wsEnum.Cells[rowIndex, 4].Value = impd.ImporterName;
                    wsEnum.Cells[rowIndex, 5].Value = impd.ImportDeclarationDate;
                    wsEnum.Cells[rowIndex, 6].Value = impd.WarehouseReceivedDate;
                    wsEnum.Cells[rowIndex, 7].Value = impd.SkuDescription;
                    wsEnum.Cells[rowIndex, 8].Value = impd.Quantity;
                    wsEnum.Cells[rowIndex, 9].Value = impd.NetWgt;
                    wsEnum.Cells[rowIndex, 10].Value = impd.TotalAmount;
                    wsEnum.Cells[rowIndex, 11].Value = impd.TotalDuty;
                    rowIndex++;
                }

                wsEnum.Cells[rowIndex, 1].Value = "รวม";
                wsEnum.Cells[string.Format("A{0}:G{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 8].Value = items.Where(x => x.ReceiptKey == currentReceiptKey).Sum(pk => pk.Quantity);
                wsEnum.Cells[rowIndex, 9].Value = items.Where(x => x.ReceiptKey == currentReceiptKey).Sum(pk => pk.NetWgt);
                wsEnum.Cells[rowIndex, 10].Value = items.Where(x => x.ReceiptKey == currentReceiptKey).Sum(pk => pk.TotalAmount);
                wsEnum.Cells[rowIndex, 11].Value = items.Where(x => x.ReceiptKey == currentReceiptKey).Sum(pk => pk.TotalDuty);
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rowIndex++;
                rowDecl++;
            }


            // Footer //
            wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
            wsEnum.Cells[string.Format("A{0}:G{1}", rowIndex, rowIndex)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsEnum.Cells[rowIndex, 8].Value = items.Sum(pk => pk.Quantity);
            wsEnum.Cells[rowIndex, 9].Value = items.Sum(pk => pk.NetWgt);
            wsEnum.Cells[rowIndex, 10].Value = items.Sum(pk => pk.TotalAmount);
            wsEnum.Cells[rowIndex, 11].Value = items.Sum(pk => pk.TotalDuty);
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

            wsEnum.Cells["A1:K" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            wsEnum.Cells["A1:K" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            // End Footer //

            // Formatting //
            wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["F2:F" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["I2:K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();

            string storefile = @"D:\02-Agility Jobs\Source Code\WMSPortal\WMSPortal.UnitTest\bin\Debug\BondReports" + string.Format(@"\ImportDeclaration14_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm"));
            var fi = new FileInfo(storefile);
            if (fi.Exists)
            {
                fi.Delete();
            }
            pck.SaveAs(fi);
           
            Console.WriteLine(string.Format("The import declaration report has been generated at '{0}'", storefile));
        }
        public void GenerateExportDeclarationReport(IEnumerable<ExportDeclarationReport> items)
        {
            int rowIndex = 1;
            int rowDecl = 1;
            string currentOrderKey = string.Empty;

            ExcelPackage pck = new ExcelPackage();
            var wsEnum = pck.Workbook.Worksheets.Add("ExportDeclaration15");

            //Add the headers
            //Solid black border around the board.
            wsEnum.Cells[string.Format("A{0}:K{1}", 1, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
            wsEnum.Cells[string.Format("A{0}:K{1}", 1, 1)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:K{1}", 2, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[2, 1].Value = "คทบ ๑๕";
            wsEnum.Cells[string.Format("A{0}:K{1}", 2, 2)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:K{1}", 3, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[3, 1].Value = "คลังสินค้าทัณฑ์บนทั่วไป บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396";
            wsEnum.Cells[string.Format("A{0}:K{1}", 3, 3)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:K{1}", 4, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[4, 1].Value = "รายงานของที่นำออกจากคลังสินค้าทัณฑ์บนฯ";
            wsEnum.Cells[string.Format("A{0}:K{1}", 4, 4)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[5, 1].Value = "นำเข้าจากต่างประเทศ";
            wsEnum.Cells[string.Format("A{0}:K{1}", 5, 5)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:K{1}", 6, 6)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[6, 1].Value = "สำหรับงวดเดือน";
            wsEnum.Cells[string.Format("A{0}:K{1}", 6, 6)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 6, 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells[string.Format("A{0}:K{1}", 7, 7)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);

            rowIndex = 8;

            wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
            wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำออกคลัง";
            wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
            wsEnum.Cells[rowIndex, 4].Value = "วันที่ส่งออก/วันที่นำออก";
            wsEnum.Cells[rowIndex, 5].Value = "ชื่อของ";
            wsEnum.Cells[rowIndex, 6].Value = "ปริมาณ";
            wsEnum.Cells[rowIndex, 7].Value = "น้ำหนัก";
            wsEnum.Cells[rowIndex, 8].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex, 9].Value = "ภาษีอากรรวม (บาท)";
            wsEnum.Cells[rowIndex, 10].Value = "ตัดจากใบขนสินค้านำเข้าคลังฯ เลขที่";
            wsEnum.Cells[rowIndex, 11].Value = "รายการที่อ้างอิง";

            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);

            rowIndex++;

            var exps = items.GroupBy(
                                   i => i.ExportDeclarationNo,
                                   (key, group) => group.First()
                               ).ToArray();



            foreach (var exp in exps)
            {
                currentOrderKey = exp.OrderKey;
                IEnumerable<ExportDeclarationReport> expds = items.Where(x => x.OrderKey == exp.OrderKey);
                foreach (ExportDeclarationReport expd in expds)
                {
                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = expd.ExportDeclarationNo;
                    wsEnum.Cells[rowIndex, 3].Value = expd.ExportDeclarationItemNo;
                    wsEnum.Cells[rowIndex, 4].Value = expd.ExportDeclarationDate;
                    wsEnum.Cells[rowIndex, 5].Value = expd.SkuDescription;
                    wsEnum.Cells[rowIndex, 6].Value = expd.Quantity;
                    wsEnum.Cells[rowIndex, 7].Value = expd.NetWgt;
                    wsEnum.Cells[rowIndex, 8].Value = expd.TotalAmount;
                    wsEnum.Cells[rowIndex, 9].Value = expd.TotalDuty;
                    wsEnum.Cells[rowIndex, 10].Value = expd.ImportDeclarationNo;
                    wsEnum.Cells[rowIndex, 11].Value = expd.ImportDeclarationItemNo;
                    rowIndex++;
                }

                wsEnum.Cells[rowIndex, 1].Value = "รวม";
                wsEnum.Cells[string.Format("A{0}:E{1}", rowIndex, rowIndex)].Merge = true;
                wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsEnum.Cells[rowIndex, 6].Value = items.Where(x => x.OrderKey == currentOrderKey).Sum(pk => pk.Quantity);
                wsEnum.Cells[rowIndex, 7].Value = items.Where(x => x.OrderKey == currentOrderKey).Sum(pk => pk.NetWgt);
                wsEnum.Cells[rowIndex, 8].Value = items.Where(x => x.OrderKey == currentOrderKey).Sum(pk => pk.TotalAmount);
                wsEnum.Cells[rowIndex, 9].Value = items.Where(x => x.OrderKey == currentOrderKey).Sum(pk => pk.TotalDuty);
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rowIndex++;
                rowDecl++;


            }
            // Footer //
            wsEnum.Cells[rowIndex, 1].Value = "รวมทั้งสิ้น";
            wsEnum.Cells[string.Format("A{0}:E{1}", rowIndex, rowIndex)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", rowIndex, rowIndex)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsEnum.Cells[rowIndex, 6].Value = items.Sum(pk => pk.Quantity);
            wsEnum.Cells[rowIndex, 7].Value = items.Sum(pk => pk.NetWgt);
            wsEnum.Cells[rowIndex, 8].Value = items.Sum(pk => pk.TotalAmount);
            wsEnum.Cells[rowIndex, 9].Value = items.Sum(pk => pk.TotalDuty);
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Font.Bold = true;
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsEnum.Cells[string.Format("A{0}:K{1}", rowIndex, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
            wsEnum.Cells[string.Format("F{0}:I{1}", rowIndex, rowIndex)].Style.Numberformat.Format = "#,##0.00";

            wsEnum.Cells["A1:K" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            wsEnum.Cells["A1:K" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            // End Footer //

            // Formatting //
            wsEnum.Cells["D2:D" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["F2:F" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["G2:I" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            wsEnum.Cells[wsEnum.Dimension.Address].AutoFitColumns();

            string storefile = @"D:\02-Agility Jobs\Source Code\WMSPortal\WMSPortal.UnitTest\bin\Debug\BondReports" + string.Format(@"\ExportDeclaration15_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm"));
            var fi = new FileInfo(storefile);
            if (fi.Exists)
            {
                fi.Delete();
            }
            pck.SaveAs(fi);
            Console.WriteLine(string.Format("The export declaration report has been generated at '{0}'", storefile));
        }
        public void GenerateGLDeclarationReport(IEnumerable<GLDeclarationReport> items)
        {
            int rowIndex = 1;
            int rowDecl = 1;
            decimal currentQty;
            decimal currentWeight;
            string endColumnName = "N";
            string currentReceipItem = string.Empty;

            ExcelPackage pck = new ExcelPackage();
            var wsEnum = pck.Workbook.Worksheets.Add("GLDeclaration16");

            //Add the headers
            //Solid black border around the board.
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 1,endColumnName, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 1,endColumnName, 1)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 2,endColumnName, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[2, 1].Value = "คทบ ๑๖";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName,2)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 3,endColumnName, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[3, 1].Value = "คลังสินค้าทัณฑ์บนทั่วไป บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 3,endColumnName, 3)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 4,endColumnName, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[4, 1].Value = "รายงานแยกประเภท";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName,4)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 5,endColumnName, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[5, 1].Value = "สำหรับงวดเดือน";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName,5)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            rowIndex = 7;

            wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
            wsEnum.Cells["A7:A8"].Merge = true;
            wsEnum.Cells["A7:A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["A7:A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนสินค้านำออกคลังฯ";
            wsEnum.Cells["B7:B8"].Merge = true;
            wsEnum.Cells["B7:B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["B7:B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
            wsEnum.Cells["C7:C8"].Merge = true;
            wsEnum.Cells["C7:C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["C7:C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 4].Value = "วันที่นำเข้า";
            wsEnum.Cells["D7:D8"].Merge = true;
            wsEnum.Cells["D7:D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["D7:D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 5].Value = "วันที่นำเข้าคลังฯ";
            wsEnum.Cells["E7:E8"].Merge = true;
            wsEnum.Cells["E7:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["E7:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 6].Value = "ชื่อของ";
            wsEnum.Cells["F7:F8"].Merge = true;
            wsEnum.Cells["F7:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["F7:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 7].Value = "ปริมาณ";
            wsEnum.Cells["G7:G8"].Merge = true;
            wsEnum.Cells["G7:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["G7:G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 8].Value = "น้ำหนัก";
            wsEnum.Cells["H7:H8"].Merge = true;
            wsEnum.Cells["H7:H8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["H7:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 9].Value = "เลขที่ใบขนสินค้านำออกจากคลังฯ";
            wsEnum.Cells["I7:I8"].Merge = true;
            wsEnum.Cells["I7:I8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["I7:I8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 10].Value = "วันที่นำออก";
            wsEnum.Cells["J7:J8"].Merge = true;
            wsEnum.Cells["J7:J8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["J7:J8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 11].Value = "ปริมาณ";
            wsEnum.Cells["K7:K8"].Merge = true;
            wsEnum.Cells["K7:K8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["K7:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 12].Value = "น้ำหนัก";
            wsEnum.Cells["L7:L8"].Merge = true;
            wsEnum.Cells["L7:L8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["L7:L8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 13].Value = "คงเหลือ";
            wsEnum.Cells["M7:N7"].Merge = true;
            wsEnum.Cells["M7:N7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["M7:N8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 13].Value = "คงเหลือ";
            wsEnum.Cells[rowIndex+1, 13].Value = "ปริมาณ";
            wsEnum.Cells[rowIndex+1, 14].Value = "น้ำหนัก";
            //wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex,endColumnName,rowIndex)].Style.Font.Bold = true;
            //wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex,endColumnName, rowIndex)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //wsEnum.Cells[string.Format("A{0}:{1}{2}", rowIndex,endColumnName, rowIndex)].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
            rowIndex = 9;

            var exps = items.GroupBy(x => new { x.ReceiptKey, x.ImportDeclarationItemNo,x.Quantity,x.NetWgt })
                    .Select(y => new GLDeclarationReport()
                    {
                        ReceiptKey = y.Key.ReceiptKey,
                        ImportDeclarationItemNo = y.Key.ImportDeclarationItemNo,
                        Quantity = y.Key.Quantity,
                        NetWgt = y.Key.NetWgt
                    }
                    );

            foreach (var exp in exps)
            {
                currentWeight = exp.Quantity * exp.NetWgt;
                currentQty = exp.Quantity;
                IEnumerable<GLDeclarationReport> expds = items.Where(x => x.ReceiptKey == exp.ReceiptKey && x.ImportDeclarationItemNo ==exp.ImportDeclarationItemNo);
              
                foreach (GLDeclarationReport expd in expds)
                {
                    decimal pickWeight =(expd.NetWgt * expd.QtyPicked);
                    currentWeight -= pickWeight;
                    currentQty -=expd.QtyPicked;

                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = expd.ImportDeclarationNo.Trim();
                    wsEnum.Cells[rowIndex, 3].Value = expd.ImportDeclarationItemNo;
                    wsEnum.Cells[rowIndex, 4].Value = expd.ImportDeclarationDate;
                    wsEnum.Cells[rowIndex, 5].Value = expd.WarehouseReceivedDate;
                    wsEnum.Cells[rowIndex, 6].Value = expd.Sku.Trim();
                    wsEnum.Cells[rowIndex, 7].Value = expd.Quantity;
                    wsEnum.Cells[rowIndex, 8].Value = expd.NetWgt * expd.Quantity;
                    wsEnum.Cells[rowIndex, 9].Value = expd.ExportDeclarationNo.Trim();
                    wsEnum.Cells[rowIndex, 10].Value = expd.ExportDeclarationDate;
                    wsEnum.Cells[rowIndex, 11].Value = expd.QtyPicked;
                    wsEnum.Cells[rowIndex, 12].Value = pickWeight;

                    wsEnum.Cells[rowIndex, 13].Value = currentQty;
                    wsEnum.Cells[rowIndex, 14].Value = currentWeight;
 
                    rowIndex++;
                }
                rowDecl++;
            }

            // Formatting //
            wsEnum.Cells["D2:D" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["E2:E" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["J2:J" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0.00";
            wsEnum.Cells["K2:K" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["L2:N" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            var modelCells = wsEnum.Cells[string.Format("A7:{0}{1}",endColumnName,rowIndex-1)];
            var border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            wsEnum.Cells[string.Format("A1:{0}{1}", endColumnName, rowIndex - 1)].AutoFitColumns();

            string storefile = @"D:\02-Agility Jobs\Source Code\WMSPortal\WMSPortal.UnitTest\bin\Debug\BondReports" + string.Format(@"\GLDeclaration16_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm"));
            var fi = new FileInfo(storefile);
            if (fi.Exists)
            {
                fi.Delete();
            }
            pck.SaveAs(fi);
            Console.WriteLine(string.Format("The export declaration report has been generated at '{0}'", storefile));
        }
        public void GenerateInventoryDeclarationReport(IEnumerable<GLDeclarationReport> items)
        {
            int rowIndex = 1;
            int rowDecl = 1;
            decimal currentQty;
            decimal currentWeight;
            string endColumnName = "K";
            string currentReceipItem = string.Empty;

            ExcelPackage pck = new ExcelPackage();
            var wsEnum = pck.Workbook.Worksheets.Add("InvDeclaration17");

            //Add the headers
            //Solid black border around the board.
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 1, endColumnName, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 1, endColumnName, 1)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[2, 1].Value = "คทบ ๑๗";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName, 2)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[3, 1].Value = "คลังสินค้าทัณฑ์บนทั่วไป บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[4, 1].Value = "รายงานของคงเหลือ";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[5, 1].Value = "สำหรับงวดเดือน";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            rowIndex = 7;

            wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
            wsEnum.Cells[rowIndex, 2].Value = "เลขที่ใบขนนำเข้าคลังฯ";
            wsEnum.Cells[rowIndex, 3].Value = "รายการที่";
            wsEnum.Cells[rowIndex, 4].Value = "วันที่ชื่อผู้นำเข้า";
            wsEnum.Cells[rowIndex, 5].Value = "วันที่นำเข้า";
            wsEnum.Cells[rowIndex, 6].Value = "วันที่นำเข้าคลังฯ";
            wsEnum.Cells[rowIndex, 7].Value = "ชื่อของ";
            wsEnum.Cells[rowIndex, 8].Value = "ปริมาณ";
            wsEnum.Cells[rowIndex, 9].Value = "น้ำหนัก";
            wsEnum.Cells[rowIndex, 10].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex, 11].Value = "ภาษีอากร (บาท)";
            wsEnum.Cells["A7:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex = 8;

            var exps = items.GroupBy(x => new { x.ReceiptKey, x.ImportDeclarationItemNo, x.Quantity, x.NetWgt })
                    .Select(y => new GLDeclarationReport()
                    {
                        ReceiptKey = y.Key.ReceiptKey,
                        ImportDeclarationItemNo = y.Key.ImportDeclarationItemNo,
                        Quantity = y.Key.Quantity,
                        NetWgt = y.Key.NetWgt
                    }
                    );

            foreach (var exp in exps)
            {
                currentWeight = exp.Quantity * exp.NetWgt;
                currentQty = exp.Quantity;
                IEnumerable<GLDeclarationReport> expds = items.Where(x => x.ReceiptKey == exp.ReceiptKey && x.ImportDeclarationItemNo == exp.ImportDeclarationItemNo);

                foreach (GLDeclarationReport expd in expds)
                {
                    decimal pickWeight = (expd.NetWgt * expd.QtyPicked);
                    currentWeight -= pickWeight;
                    currentQty -= expd.QtyPicked;

                    wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                    wsEnum.Cells[rowIndex, 2].Value = expd.ImportDeclarationNo.Trim();
                    wsEnum.Cells[rowIndex, 3].Value = expd.ImportDeclarationItemNo;
                    wsEnum.Cells[rowIndex, 4].Value = expd.ImporterName;
                    wsEnum.Cells[rowIndex, 5].Value = expd.ImportDeclarationDate;
                    wsEnum.Cells[rowIndex, 6].Value = expd.WarehouseReceivedDate;
                    wsEnum.Cells[rowIndex, 7].Value = expd.Sku;
                    wsEnum.Cells[rowIndex, 8].Value = expd.Quantity;
                    wsEnum.Cells[rowIndex, 9].Value = expd.NetWgt * expd.Quantity;
                    wsEnum.Cells[rowIndex, 10].Value = expd.UnitPrice * expd.Quantity;
                    wsEnum.Cells[rowIndex, 11].Value = expd.TotalDuty;
                

                    rowIndex++;
                }
                //rowIndex++;
                rowDecl++;
            }

            // Formatting //
           
            wsEnum.Cells["E2:F" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["F2:F" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["I2:K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            var modelCells = wsEnum.Cells[string.Format("A7:{0}{1}", endColumnName, rowIndex - 1)];
            var border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            wsEnum.Cells[string.Format("A1:{0}{1}", endColumnName, rowIndex - 1)].AutoFitColumns();

            string storefile = @"D:\02-Agility Jobs\Source Code\WMSPortal\WMSPortal.UnitTest\bin\Debug\BondReports" + string.Format(@"\InvDeclaration17_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm"));
            var fi = new FileInfo(storefile);
            if (fi.Exists)
            {
                fi.Delete();
            }
            pck.SaveAs(fi);
            Console.WriteLine(string.Format("The export declaration report has been generated at '{0}'", storefile));
        }
        public void GenerateMovementReport(InventoryMovementReport trans)
        {
            int rowIndex = 1;
            int rowDecl = 1;
            int firstRow = 0;
            decimal currentQty;
            string endColumnName = "S";
            string currentReceipItem = string.Empty;

            ExcelPackage pck = new ExcelPackage();
            var wsEnum = pck.Workbook.Worksheets.Add("MovementTrans18");

            //Add the headers
            //Solid black border around the board.
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 1, endColumnName, 1)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[1, 1].Value = "แบบแนบท้ายประมวลฯ ข้อ ๕ ๐๒ ๐๒ ๒๗";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 1, endColumnName, 1)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 1, 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName, 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[2, 1].Value = "คทบ ๑๘";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 2, endColumnName, 2)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 2, 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[3, 1].Value = "คลังสินค้าทัณฑ์บนทั่วไป บริษัท อจิลิตี้ จำกัด เลขทะเบียนผู้ใช้สิทธิ์ประโยชน์ทางภาษีอากร 0105532011396";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 3, endColumnName, 3)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 3, 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[4, 1].Value = "รายงานการเคลื่อนไหวของของที่นำเข้า";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 4, endColumnName, 4)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 4, 4)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Transparent);
            wsEnum.Cells[5, 1].Value = "สำหรับงวดเดือน";
            wsEnum.Cells[string.Format("A{0}:{1}{2}", 5, endColumnName, 5)].Merge = true;
            wsEnum.Cells[string.Format("A{0}:A{1}", 5, 5)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            rowIndex = 7;

            wsEnum.Cells[rowIndex, 1].Value = "ลำดับที่";
            wsEnum.Cells["A7:A8"].Merge = true;
            wsEnum.Cells["A7:A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["A7:A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["A7:A8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 2].Value = "ชื่อผู้นำเข้า";
            wsEnum.Cells["B7:B8"].Merge = true;
            wsEnum.Cells["B7:B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["B7:B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["B7:B8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 3].Value = "วันที่นำเข้า";
            wsEnum.Cells["C7:C8"].Merge = true;
            wsEnum.Cells["C7:C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["C7:C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["C7:C8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 4].Value = "เลขที่ใบขนสินค้านำเข้าคลังฯ";
            wsEnum.Cells["D7:D8"].Merge = true;
            wsEnum.Cells["D7:D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["D7:D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["D7:D8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 5].Value = "รายการที่";
            wsEnum.Cells["E7:E8"].Merge = true;
            wsEnum.Cells["E7:E8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["E7:E8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["E7:E8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 6].Value = "ชนิดของเป็นภาษาอังกฤษ";
            wsEnum.Cells["F7:F8"].Merge = true;
            wsEnum.Cells["F7:F8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["F7:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["F7:F8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 7].Value = "หน่วยของปริมาณ";
            wsEnum.Cells["G7:G8"].Merge = true;
            wsEnum.Cells["G7:G8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            wsEnum.Cells["G7:G8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["G7:G8"].Style.WrapText = true;

            wsEnum.Cells[rowIndex, 8].Value = "ยกมา";
            wsEnum.Cells[rowIndex + 1, 8].Value = "ปริมาณยกมา";
            wsEnum.Cells[rowIndex + 1, 9].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex + 1, 10].Value = "ภาษีอากรรวม (บาท)";
            wsEnum.Cells[rowIndex + 1, 10].Style.WrapText = true;
            wsEnum.Cells["H7:J7"].Merge = true;
            wsEnum.Cells["H7:J7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsEnum.Cells[rowIndex, 11].Value = "นำเข้า";
            wsEnum.Cells[rowIndex + 1, 11].Value = "ปริมาณนำเข้า";
            wsEnum.Cells[rowIndex + 1, 12].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex + 1, 13].Value = "ภาษีอากรรวม (บาท)";
            wsEnum.Cells[rowIndex + 1, 13].Style.WrapText = true;
            wsEnum.Cells["K7:M7"].Merge = true;
            wsEnum.Cells["K7:M7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            wsEnum.Cells[rowIndex, 14].Value = "นำออก";
            wsEnum.Cells[rowIndex + 1, 14].Value = "เลขที่ใบขนสินค้านำออกคลังฯ";
            wsEnum.Cells[rowIndex + 1, 14].Style.WrapText = true;
            wsEnum.Cells[rowIndex + 1, 15].Value = "รายการที่";
            wsEnum.Cells[rowIndex + 1, 16].Value = "ปริมาณนำออก";
            wsEnum.Cells["N7:P7"].Merge = true;
            wsEnum.Cells["N7:P7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            wsEnum.Cells[rowIndex, 17].Value = "คงเหลือ";
            wsEnum.Cells[rowIndex + 1, 17].Value = "ปริมาณคงเหลือ";
            wsEnum.Cells[rowIndex + 1, 18].Value = "มูลค่า (บาท)";
            wsEnum.Cells[rowIndex + 1, 18].Style.WrapText = true;
            wsEnum.Cells[rowIndex + 1, 19].Value = "ภาษีอากรรวม (บาท)";
            wsEnum.Cells[rowIndex + 1, 19].Style.WrapText = true;
            wsEnum.Cells["Q7:S7"].Merge = true;
            wsEnum.Cells["Q7:S7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["A7:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["H8:S8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells["H8:S8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            rowIndex = 9;

            IEnumerable<GLDeclarationReport> bfItems = trans.BroughtForwardItems;
            IEnumerable<ExportDeclarationReport> expItems = trans.ExportDeclarationItems;
            IEnumerable<GLDeclarationReport> glItems = trans.GLDeclarationItems;

            foreach (var bf in bfItems)
            {
                currentQty = bf.BroughtForward;

                wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                wsEnum.Cells[rowIndex, 2].Value = bf.ImporterName.Trim();
                wsEnum.Cells[rowIndex, 3].Value = bf.ImportDeclarationDate;
                wsEnum.Cells[rowIndex, 4].Value = bf.ImportDeclarationNo.Trim();
                wsEnum.Cells[rowIndex, 5].Value = bf.ImportDeclarationItemNo.Trim();
                wsEnum.Cells[rowIndex, 6].Value = bf.SkuDescription.Trim();
                wsEnum.Cells[rowIndex, 7].Value = bf.PackageUnit;
                wsEnum.Cells[rowIndex, 8].Value = bf.BroughtForward;
                wsEnum.Cells[rowIndex, 9].Value = bf.UnitPrice * bf.BroughtForward;
                wsEnum.Cells[rowIndex, 10].Value = bf.TotalDuty;

                IEnumerable<ExportDeclarationReport> expfItems = expItems.Where(x => x.Id == bf.Id);
                firstRow = 0;
                if (expfItems.Count() == 0)
                {
                    wsEnum.Cells[rowIndex, 17].Value = bf.BroughtForward;
                    wsEnum.Cells[rowIndex, 18].Value = bf.UnitPrice * bf.BroughtForward;
                    wsEnum.Cells[rowIndex, 19].Value = bf.TotalDuty;
                }
                else
                {
                    foreach (var exp in expfItems)
                    {
                        if (firstRow > 0) rowIndex++;
                        wsEnum.Cells[rowIndex, 14].Value = exp.ExportDeclarationNo.Trim();
                        wsEnum.Cells[rowIndex, 15].Value = exp.ExportDeclarationItemNo.Trim();
                        wsEnum.Cells[rowIndex, 16].Value = exp.Quantity;

                        currentQty -= exp.Quantity;

                        wsEnum.Cells[rowIndex, 17].Value = currentQty;
                        wsEnum.Cells[rowIndex, 18].Value = bf.UnitPrice * currentQty;
                        wsEnum.Cells[rowIndex, 19].Value = bf.TotalDuty;


                        firstRow++;
                    }
                }
                

                rowIndex++;
                rowDecl++;
            }

            currentQty = 0;

            var Ids = glItems.GroupBy(x => new { x.Id })
                  .Select(y => new GLDeclarationReport()
                  {
                      Id = y.Key.Id
                  }
                  );

            foreach (var key in Ids)
            {
                IEnumerable<GLDeclarationReport> expfItems = glItems.Where(x => x.Id == key.Id);
                firstRow = 0;
                foreach (var exp in expfItems)
                {
                    if (firstRow == 0)
                    {
                        currentQty = exp.Quantity;

                        wsEnum.Cells[rowIndex, 1].Value = rowDecl;
                        wsEnum.Cells[rowIndex, 2].Value = exp.ImporterName.Trim();
                        wsEnum.Cells[rowIndex, 3].Value = exp.ImportDeclarationDate;
                        wsEnum.Cells[rowIndex, 4].Value = exp.ImportDeclarationNo.Trim();
                        wsEnum.Cells[rowIndex, 5].Value = exp.ImportDeclarationItemNo.Trim();
                        wsEnum.Cells[rowIndex, 6].Value = exp.SkuDescription.Trim();
                        wsEnum.Cells[rowIndex, 7].Value = exp.PackageUnit;
                        wsEnum.Cells[rowIndex, 11].Value = exp.Quantity;
                        wsEnum.Cells[rowIndex, 12].Value = exp.UnitPrice * exp.Quantity;
                        wsEnum.Cells[rowIndex, 13].Value = exp.TotalDuty;

                        rowDecl++;
                    }

                    currentQty -= exp.QtyPicked;
                    wsEnum.Cells[rowIndex, 14].Value = StringExtension.NonBlankValueOf(exp.ExportDeclarationNo);
                    wsEnum.Cells[rowIndex, 15].Value = exp.UnitPrice * exp.QtyPicked;
                    wsEnum.Cells[rowIndex, 16].Value = exp.QtyPicked;
                    wsEnum.Cells[rowIndex, 17].Value = currentQty;
                    wsEnum.Cells[rowIndex, 18].Value = exp.UnitPrice * currentQty;
                    wsEnum.Cells[rowIndex, 19].Value = exp.TotalDuty;
                    rowIndex++;
                    firstRow++;
                }
            }

            // Formatting //
            wsEnum.Cells["C2:C" + rowIndex].Style.Numberformat.Format = "dd-mm-yyyy";
            wsEnum.Cells["G2:G" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["H2:H" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["I2:J" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            wsEnum.Cells["K2:K" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["L2:M" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            wsEnum.Cells["P2:Q" + rowIndex].Style.Numberformat.Format = "#,##0";
            wsEnum.Cells["R2:S" + rowIndex].Style.Numberformat.Format = "#,##0.00";

            var modelCells = wsEnum.Cells[string.Format("A7:{0}{1}", endColumnName, rowIndex - 1)];
            var border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            wsEnum.Cells[string.Format("A1:{0}{1}", endColumnName, rowIndex - 1)].AutoFitColumns();
            wsEnum.Cells["E7:E8"].Merge = true;

            rowIndex += 3;
            int lastRow = rowIndex;
            wsEnum.Cells[rowIndex, 2].Value = "สรุปการนำเข้า/ออก";
            wsEnum.Cells[string.Format("B{0}:{1}{0}", lastRow, "E")].Merge = true;
            wsEnum.Cells[string.Format("B{0}:{1}{0}", lastRow, "E")].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            rowIndex++;
            wsEnum.Cells[rowIndex, 2].Value = "รายการ";
            wsEnum.Cells[rowIndex, 3].Value = "จำนวนใบขน(ฉบับ)";
            wsEnum.Cells[rowIndex, 3].Style.WrapText = true;
            wsEnum.Cells[rowIndex, 4].Value = "มูลค่า(บาท)";
            wsEnum.Cells[rowIndex, 5].Value = "ภาษีอากรรวม(บาท)";
            wsEnum.Cells[rowIndex, 5].Style.WrapText = true;
            rowIndex++;
            wsEnum.Cells[rowIndex, 2].Value = "ยอดยกมา";
            rowIndex++;
            wsEnum.Cells[rowIndex, 2].Value = "นำเข้าจากต่างประเทศ";
            wsEnum.Cells[rowIndex, 2].Style.WrapText = true;
            rowIndex++;
            wsEnum.Cells[rowIndex, 2].Value = "รับโอน";
            rowIndex++;
            wsEnum.Cells[rowIndex, 2].Value = "ส่งออกต่างประเทศ";
            wsEnum.Cells[rowIndex, 2].Style.WrapText = true;
            rowIndex++;
            wsEnum.Cells[rowIndex, 2].Value = "โอนย้าย";
            rowIndex++;
            wsEnum.Cells[rowIndex, 2].Value = "ชำระภาษี";
            rowIndex++;
            wsEnum.Cells[rowIndex, 2].Value = "อื่นๆ";

            wsEnum.Cells[string.Format("B{0}:E{0}",lastRow+1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsEnum.Cells[string.Format("B{0}:E{0}", lastRow + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            modelCells = wsEnum.Cells[string.Format("B{0}:{1}{2}", lastRow, "E", rowIndex)];
            border = modelCells.Style.Border.Top.Style = modelCells.Style.Border.Left.Style = modelCells.Style.Border.Right.Style = modelCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


            string storefile = @"D:\02-Agility Jobs\Source Code\WMSPortal\WMSPortal.UnitTest\bin\Debug\BondReports" + string.Format(@"\MovementTrans18_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm"));
            var fi = new FileInfo(storefile);
            if (fi.Exists)
            {
                fi.Delete();
            }
            pck.SaveAs(fi);
            Console.WriteLine(string.Format("The export declaration report has been generated at '{0}'", storefile));
        }
    }
}
