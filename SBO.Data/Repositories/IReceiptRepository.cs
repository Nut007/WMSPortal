using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;

namespace WMSPortal.Data.Repositories
{
    public interface IReceiptRepository
    {
        IEnumerable<ReceiptDetail> GetInboundShipment(string column,string value1,string value2,int connectionId,string userId);
        IEnumerable<ReceiptDetail> GetReceiptDetail(string receiptKey);
        Receipt GetReceipt(string receiptKey);
        TEMP_ID AddBaggage(TEMP_ID baggage);
        int DeleteBaggage(string baggageNo);
        TEMP_ID ImportBaggage(TEMP_ID baggage);
        OperationResult AddBaggages(List<TEMP_ID> baggages);
        IEnumerable<TEMP_ID> GetBaggageList(string column, string value1, string value2, int connectionId, string userId,string status);
        IEnumerable<TEMP_ID> GetMawbBaggages(string mawb, string containerno);
        IEnumerable<TEMP_ID> GetMawbItems(string mawb);
        IEnumerable<TEMP_ID> GetCN38Items(string cn38);
        IEnumerable<TEMP_ID> GetMissingCN35(string mawb);
        IEnumerable<TEMP_ID> GetDespatchItems(string mawb, string handoverdate);
        IEnumerable<TEMP_ID> GetConsigmentItems(string mawb, string handoverdate);
        IEnumerable<TEMP_ID> GetManifestItems(string mawb);
        bool DeleteBaggageItems(List<string> bagId, string type);
        bool UnManifestItems(string[] bagId);
        bool AddBaggageItems(string[] bagId,string mawb,string containerNo);
        bool DeleteCN38Items(string mawb, List<string> cn38);
        OperationResult AddCN38Items(string mawb, List<string> bagId);
        int UpdateScanningStatus(TEMP_ID bagId);

    }
}
