using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;

namespace WMSPortal.Data.Repositories
{
    public interface IHelperRepository
    {
        IEnumerable<Storer> GetStorers();
        IEnumerable<Codelkup> GetDeclarationInboundType();
        IEnumerable<Codelkup> GetDeclarationOutboundType();
        IEnumerable<Codelkup> GetDeclarationType();
        IEnumerable<Codelkup> GetMawbList();
        string GetSQLInCondition(object[] conditions);
        string GetDocumentNo(DocumentType docType);
        IEnumerable<StoreProcedure> GetStoreProcedureReport();
        IEnumerable<StoreProcedure> GetStoreProcedureColumns(string procedureName);
        DataTable GetReportResult(List<StoreProcedure> parameters);
    }
}
