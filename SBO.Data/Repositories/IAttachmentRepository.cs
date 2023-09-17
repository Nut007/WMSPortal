using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core;

namespace WMSPortal.Data.Repositories
{
    public interface IAttachmentRepository
    {
        OperationResult SaveAttachment(Attachment NewAttachment);
    }
}
