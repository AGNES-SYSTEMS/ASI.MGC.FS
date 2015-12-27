using System;
using System.Collections.Generic;
using System.Linq;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Domain.Repositories
{
    public class ReportRepository : Repository<BANKTRANSACTION>
    {
        readonly DataAccess.ASI_MGC_FSEntities _context;
        public ReportRepository(DataAccess.ASI_MGC_FSEntities context)
            : base(context)
        {
            dbSet = context.Set<BANKTRANSACTION>();
            _context = context;
        }

        public List<rpt_CashPayment_Result> RptCashPayment(string voucherType, string voucherCode)
        {
            List<rpt_CashPayment_Result> lst = null;
            try
            {
                lst = _context.rpt_CashPayment(voucherType, voucherCode).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_BankPayment_Result> RptBankPayment(string voucherType, string voucherCode)
        {
            List<rpt_BankPayment_Result> lst = null;
            try
            {
                lst = _context.rpt_BankPayment(voucherType, voucherCode).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_BankReceipt_Result> RptBankReceipt(string voucherType, string voucherCode)
        {
            List<rpt_BankReceipt_Result> lst = null;
            try
            {
                lst = _context.rpt_BankReceipt(voucherType, voucherCode).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_CashReceipt_Result> RptCashReceipt(string voucherType, string voucherCode)
        {
            List<rpt_CashReceipt_Result> lst = null;
            try
            {
                lst = _context.rpt_CashReceipt(voucherType, voucherCode).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_Invoice_Result> RptInvoice(string invNo, string invType)
        {
            List<rpt_Invoice_Result> lst = null;
            try
            {
                lst = _context.rpt_Invoice(invNo, invType).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_DeliveryNote_Result> RptDeliveryNote(string dlNo, string dlnType)
        {
            List<rpt_DeliveryNote_Result> lst = null;
            try
            {
                lst = _context.rpt_DeliveryNote(dlNo, dlnType).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_Quotation_Result> RptQuotation(string quotNo)
        {
            List<rpt_Quotation_Result> lst = null;
            try
            {
                lst = _context.rpt_Quotation(quotNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_JobCardFormat_Result> RptJobCardFormat(string jobNo)
        {
            List<rpt_JobCardFormat_Result> lst = null;
            try
            {
                lst = _context.rpt_JobCardFormat(jobNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_MaterialReceiptVcoucher_Result> RptMaterialReceiptVoucher(string mrvNo)
        {
            List<rpt_MaterialReceiptVcoucher_Result> lst = null;
            try
            {
                lst = _context.rpt_MaterialReceiptVcoucher(mrvNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_CashMemo_Result> RptCashMemo(string invNo, string invType)
        {
            List<rpt_CashMemo_Result> lst = null;
            try
            {
                lst = _context.rpt_CashMemo(invNo, invType).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
    }
}
