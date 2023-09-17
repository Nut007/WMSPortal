using AutoMapper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMSPortal.Core.Model;
using WMSPortal.Data.Repositories;
using WMSPortal.ViewModels;

namespace WMSPortal.Controllers
{
    public class MailReportController : Controller
    {
        // GET: MailReport
       
        private IReceiptRepository _receiptRepository;
        public MailReportController(IReceiptRepository receiptRepository)
        {
            _receiptRepository = receiptRepository;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TSAReport(string mawb, string handoverdate)
        {
            IEnumerable<TEMP_ID> model = _receiptRepository.GetDespatchItems(mawb, handoverdate);
            IEnumerable<MailReportViewModels> items = Mapper.Map<IEnumerable<TEMP_ID>, IEnumerable<MailReportViewModels>>(model);
            ParameterField paramField = new ParameterField();
            ParameterFields paramFields = new ParameterFields();
            ParameterDiscreteValue paramDiscreteValue = new ParameterDiscreteValue();

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "rptTSA.rpt"));
            rd.SetDataSource(items);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptTSA.pdf");
        }
        public ActionResult CSDReport(string mawb, string handoverdate)
        {
            IEnumerable<TEMP_ID> model = _receiptRepository.GetConsigmentItems(mawb, handoverdate);
            IEnumerable<MailReportViewModels> items = Mapper.Map<IEnumerable<TEMP_ID>, IEnumerable<MailReportViewModels>>(model);
            ParameterField paramField = new ParameterField();
            ParameterFields paramFields = new ParameterFields();
            ParameterDiscreteValue paramDiscreteValue = new ParameterDiscreteValue();

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "rptCSD.rpt"));
            rd.SetDataSource(items);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptCSD.pdf");
        }
        public ActionResult ManifestReport(string mawb)
        {
            IEnumerable<TEMP_ID> model = _receiptRepository.GetManifestItems(mawb);
            IEnumerable<MailReportViewModels> items = Mapper.Map<IEnumerable<TEMP_ID>, IEnumerable<MailReportViewModels>>(model);
            ParameterField paramField = new ParameterField();
            ParameterFields paramFields = new ParameterFields();
            ParameterDiscreteValue paramDiscreteValue = new ParameterDiscreteValue();

            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "rptMailManifest.rpt"));
            rd.SetDataSource(items);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptManifest.pdf");
        }
    }
}