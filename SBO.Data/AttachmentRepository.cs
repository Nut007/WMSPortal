using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Data.Repositories;

namespace WMSPortal.Data
{
    public class AttachmentRepository : IAttachmentRepository
    {
        public OperationResult SaveAttachment(Core.Attachment NewAttachment)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                //AttachmentContext db = new AttachmentContext();
                //db.Attachments.Add(NewAttachment);
                //db.SaveChanges();
                operationResult.Success = true;
                operationResult.Message = "Attachment Added Successfully.";

            }
            catch (Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = "An Error Ocured During saving the new Attachment ";
            }
            return operationResult;
        }
    }
    public class OperationResult
    {
        public bool Success;
        public string Message;
        public OperationResult()
        {
            Success = true;
            Message = string.Empty;
        }
        public OperationResult(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }
    }
}
