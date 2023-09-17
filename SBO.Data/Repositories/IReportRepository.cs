using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;

namespace WMSPortal.Data.Repositories
{
    public interface IReportRepository
    {
        IEnumerable<ImportDeclarationReport> GetImportDeclaraion(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] importers,string sku);
        IEnumerable<ImportDeclarationReport> GetImportDeclaraionIncludeId(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] importers);
        IEnumerable<ImportDeclarationReport> GetImportDeclaraionFreeZone(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] importers, bool isSkipRemark, string sku);
        IEnumerable<ExportDeclarationReport> GetExportDeclaraion(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] exporters,string sku);
        IEnumerable<ExportDeclarationReport> GetExportFreeZoneDeclaraion(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] exporters,string sku);
        IEnumerable<ExportDeclarationReport> GetExportFreeZoneDeclaraionNew(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] exporters, string sku);
        IEnumerable<GLDeclarationReport> GetStockDeclaration(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers,string sku);
        IEnumerable<GLDeclarationReport> GetStockDeclarationByLocation(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers, string sku);
        IEnumerable<GLDeclarationReport> GetStockDeclarationFreeZone(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers, string sku);
        IEnumerable<GLDeclarationReport> GetLedgerDeclaration(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers);
        InventoryMovementReport GetLedgerDeclarationFreezone(string dateType, string inventoryDate, string year, string month, object[] declarationType, object[] importers);
        InventoryMovementReport GetMovementDeclaration(string dateType, string startDate, string stopDate, string year, string month, object[] declarationType, object[] importers,string sku);
    }
}
