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

        public List<rpt_MaterialReceiptVoucher_Result> RptMaterialReceiptVoucher(string mrvNo)
        {
            List<rpt_MaterialReceiptVoucher_Result> lst = null;
            try
            {
                lst = _context.rpt_MaterialReceiptVoucher(mrvNo).ToList();
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

        public List<rpt_BalanceSheet_Result> RptBalanceSheet(DateTime startDate, DateTime endDate)
        {
            List<rpt_BalanceSheet_Result> lst = null;
            try
            {
                lst = _context.rpt_BalanceSheet(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public List<rpt_ARStatement2_Result> RptArStatement(DateTime startDate, DateTime endDate)
        {
            List<rpt_ARStatement2_Result> lst = null;
            try
            {
                lst = _context.rpt_ARStatement2(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public List<rpt_ARStatementOutstanding_Result> RptArStatementOutstanding(DateTime startDate, DateTime endDate)
        {
            List<rpt_ARStatementOutstanding_Result> lst = null;
            try
            {
                lst = _context.rpt_ARStatementOutstanding(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_ARSummary_Result> RptArSummary(DateTime startDate, DateTime endDate)
        {
            List<rpt_ARSummary_Result> lst = null;
            try
            {
                lst = _context.rpt_ARSummary(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_BankStatement_Result> RptBankStatement(DateTime startDate, DateTime endDate)
        {
            List<rpt_BankStatement_Result> lst = null;
            try
            {
                lst = _context.rpt_BankStatement(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_CancelledJobs_Result> RptCancelledJobs(DateTime startDate, DateTime endDate)
        {
            List<rpt_CancelledJobs_Result> lst = null;
            try
            {
                lst = _context.rpt_CancelledJobs(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_CashMemoReverse_Result> RptCashMemoReverse(DateTime startDate, DateTime endDate)
        {
            List<rpt_CashMemoReverse_Result> lst = null;
            try
            {
                lst = _context.rpt_CashMemoReverse(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_CashMemoWiseReport_Result> RptCashMemoWiseReport(DateTime startDate, DateTime endDate)
        {
            List<rpt_CashMemoWiseReport_Result> lst = null;
            try
            {
                lst = _context.rpt_CashMemoWiseReport(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_DeliveredMrv_Result> RptDeliveredMrvDetails(DateTime startDate, DateTime endDate)
        {
            List<rpt_DeliveredMrv_Result> lst = null;
            try
            {
                lst = _context.rpt_DeliveredMrv(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_InvWiseReprot_Result> RptInvoiceWiseReport(DateTime startDate, DateTime endDate)
        {
            List<rpt_InvWiseReprot_Result> lst = null;
            try
            {
                lst = _context.rpt_InvWiseReprot(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_GLStatement_Result> RptGlStatement(string glCode, DateTime startDate, DateTime endDate)
        {
            List<rpt_GLStatement_Result> lst = null;
            try
            {
                lst = _context.rpt_GLStatement(glCode, startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_EmpWiseJobDetails_Result> RptEmpWiseJobDetails(DateTime startDate, DateTime endDate)
        {
            List<rpt_EmpWiseJobDetails_Result> lst = null;
            try
            {
                lst = _context.rpt_EmpWiseJobDetails(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_DeliveryNoteDetails_Result> RptDeliveryNoteDetails(string dlNo)
        {
            List<rpt_DeliveryNoteDetails_Result> lst = null;
            try
            {
                lst = _context.rpt_DeliveryNoteDetails(dlNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_DeliveryDetails_Result> RptDeliveryDetails(DateTime startDate, DateTime endDate)
        {
            List<rpt_DeliveryDetails_Result> lst = null;
            try
            {
                lst = _context.rpt_DeliveryDetails(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public void SpGetStockLedgerData(DateTime startDate, DateTime endDate)
        {
            try
            {
                _context.sp_GetStockLedgerData(startDate, endDate);
            }
            catch (Exception)
            {
                //exception handling pending
            }
        }

        public IList<rpt_StockLedger_Result> RptStockLedger(DateTime startDate, DateTime endDate)
        {
            List<rpt_StockLedger_Result> lst = null;
            try
            {
                lst = _context.rpt_StockLedger(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_StockJournal_Result> RptStockJournal(string voucherNo)
        {
            List<rpt_StockJournal_Result> lst = null;
            try
            {
                lst = _context.rpt_StockJournal(voucherNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_SalesInvoice_Result> RptSalesInvoice(string invNo, string invType)
        {
            List<rpt_SalesInvoice_Result> lst = null;
            try
            {
                lst = _context.rpt_SalesInvoice(invNo, invType).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_ReversedInvoice_Result> RptReversedInvoice(DateTime startDate, DateTime endDate)
        {
            List<rpt_ReversedInvoice_Result> lst = null;
            try
            {
                lst = _context.rpt_ReversedInvoice(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_PurchaseRegister_Result> RptPurchaseRegister(DateTime startDate, DateTime endDate)
        {
            List<rpt_PurchaseRegister_Result> lst = null;
            try
            {
                lst = _context.rpt_PurchaseRegister(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_PandLStatement_Result> RptProfLossStatement(DateTime startDate, DateTime endDate)
        {
            List<rpt_PandLStatement_Result> lst = null;
            try
            {
                lst = _context.rpt_PandLStatement(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_PendingMrvDetails_Result> RptPendingMrvDetails(DateTime startDate, DateTime endDate)
        {
            List<rpt_PendingMrvDetails_Result> lst = null;
            try
            {
                lst = _context.rpt_PendingMrvDetails(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_MrvStatement_Result> RptMrvStatement(DateTime startDate, DateTime endDate)
        {
            List<rpt_MrvStatement_Result> lst = null;
            try
            {
                lst = _context.rpt_MrvStatement(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_JournalVoucher_Result> RptJournalVoucher()
        {
            List<rpt_JournalVoucher_Result> lst = null;
            try
            {
                lst = _context.rpt_JournalVoucher().ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_JobWiseStatement_Result> RptJobWiseStatement(DateTime startDate, DateTime endDate)
        {
            List<rpt_JobWiseStatement_Result> lst = null;
            try
            {
                lst = _context.rpt_JobWiseStatement(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_JobCardDetails_Result> RptJobCardDetails(string jobNo)
        {
            List<rpt_JobCardDetails_Result> lst = null;
            try
            {
                lst = _context.rpt_JobCardDetails(jobNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_EmpSales_Result> RptEmpSales(string empCode, DateTime startDate, DateTime endDate)
        {
            List<rpt_EmpSales_Result> lst = null;
            try
            {
                lst = _context.rpt_EmpSales(empCode, startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_StockReceipt_Result> RptStockReceipt(string voucherNo)
        {
            List<rpt_StockReceipt_Result> lst = null;
            try
            {
                lst = _context.rpt_StockReceipt(voucherNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_StockReport_Result> RptStockReport()
        {
            List<rpt_StockReport_Result> lst = null;
            try
            {
                lst = _context.rpt_StockReport().ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_ServiceItems_Result> RptServiceItems()
        {
            List<rpt_ServiceItems_Result> lst = null;
            try
            {
                lst = _context.rpt_ServiceItems().ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public IList<rpt_TrialBalance_Result> RptTrialBalance(DateTime startDate, DateTime endDate)
        {
            List<rpt_TrialBalance_Result> lst = null;
            try
            {
                lst = _context.rpt_TrialBalance(startDate, endDate).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }

        public void SpGetBankStatementData(string bankCode, DateTime startDate, DateTime endDate)
        {
            try
            {
                _context.sp_GetBankStatementData(bankCode, startDate, endDate,null,null,null,null,null);
            }
            catch (Exception)
            {
                //exception handling pending
            }
        }

        public void SpGetGlStatementData(string glCode, DateTime startDate, DateTime endDate)
        {
            try
            {
                _context.sp_GetGLStatementData(glCode, startDate, endDate,null,null,null,null,null);
            }
            catch (Exception)
            {
                //exception handling pending
            }
        }

        public void SpGetTrialBalanceData(DateTime startDate, DateTime endDate)
        {
            try
            {
                _context.sp_GetTrialBalanceData(startDate, endDate);
            }
            catch (Exception)
            {
                //exception handling pending
            }
        }

        public void Sp_JvDataList(string jvNo)
        {
            try
            {
                _context.sp_JvDataList(jvNo);
            }
            catch (Exception)
            {
                //exception handling pending
            }
        }
        public List<rpt_MRV_Job_Details_Result> Rpt_MrvJobDetails(string mrvNo)
        {
            List<rpt_MRV_Job_Details_Result> lst = null;
            try
            {
                lst = _context.rpt_MRV_Job_Details(mrvNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public void Sp_GetVoucherDetails(string vType, string vCode)
        {
            try
            {
                _context.sp_GetVoucherDetails(vType, vCode);
            }
            catch (Exception)
            {
                //exception handling pending
            }
        }
        public string sp_GetLastDeliveryNumber()
        {
            string lastDlnNumber = null;
            try
            {
                lastDlnNumber = !string.IsNullOrEmpty(_context.sp_GetLastDeliveryNumber().FirstOrDefault()) ? _context.sp_GetLastDeliveryNumber().FirstOrDefault() : "";
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lastDlnNumber;
        }
        public void Sp_GetARStatementData(string acCode, DateTime startDate, DateTime endDate)
        {
            try
            {
                _context.sp_GetARStatementData(acCode, startDate, endDate,null,null,null,null,null);
            }
            catch (Exception)
            {
                //exception handling pending
            }
        }
        public void Sp_GetARStatementOutStandingData(string acCode, DateTime startDate, DateTime endDate)
        {
            try
            {
                _context.sp_GetARStatementOutStandingData(acCode, startDate, endDate,null,null,null,null,null);
            }
            catch (Exception)
            {
                //exception handling pending
            }
        }
        public IList<sp_GetCashMrvDetails_Result> sp_GetCashMrvDetails(string custCode = null, string custName = null, string telephone = null, string mrvNo = null, string jobNo = null)
        {
            List<sp_GetCashMrvDetails_Result> lst = null;
            try
            {
                lst = _context.sp_GetCashMrvDetails(custCode, custName, telephone, mrvNo, jobNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public bool Sp_DeleteSalesByJobId(string jobId)
        {
            try
            {
                _context.sp_DeleteSalesbyJobId(jobId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<sp_GetARMrvDetails_Result> sp_GetARMrvDetails(string custCode = null, string custName = null, string telephone = null, string mrvNo = null, string jobNo = null)
        {
            List<sp_GetARMrvDetails_Result> lst = null;
            try
            {
                lst = _context.sp_GetARMrvDetails(custCode, custName, telephone, mrvNo, jobNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public bool sp_GetSalesDetailsByJobNo(string jobId)
        {
            try
            {
                _context.sp_GetSalesDetailsByJobNo(jobId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<sp_GetSalesByJobNo_Result> Sp_GetSalesByJobNo(string jobId)
        {
            List<sp_GetSalesByJobNo_Result> lst = null;
            try
            {
                lst = _context.sp_GetSalesByJobNo(jobId).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public IList<sp_GetCashMemoMrvList_Result> sp_GetCashMemoMrvList(int pageIndex, int pageSize, string mrvCode = null, string custName = null, string jobNo = null)
        {
            List<sp_GetCashMemoMrvList_Result> lst = null;
            try
            {
                lst = _context.sp_GetCashMemoMrvList(pageIndex, pageSize, mrvCode, custName, jobNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public IList<sp_GetInvoicePreparationMrvList_Result> sp_GetInvoicePreparationMrvList(int pageIndex, int pageSize, string mrvCode = null, string custName = null, string jobNo = null)
        {
            List<sp_GetInvoicePreparationMrvList_Result> lst = null;
            try
            {
                lst = _context.sp_GetInvoicePreparationMrvList(pageIndex, pageSize, mrvCode, custName, jobNo).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public IList<sp_GetAllocationDetailsDocNo_Result> sp_GetAllocationDetailsDocList(int pageIndex, int pageSize, string partyCode, string docId = null)
        {
            List<sp_GetAllocationDetailsDocNo_Result> lst = null;
            try
            {
                lst = _context.sp_GetAllocationDetailsDocNo(pageIndex, pageSize, partyCode, docId).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public IList<sp_GetPartyAllocationDocumentsCredit_Result> sp_GetPartyAllocationCreditDocuments(string partyCode)
        {
            List<sp_GetPartyAllocationDocumentsCredit_Result> lst = null;
            try
            {
                lst = _context.sp_GetPartyAllocationDocumentsCredit(partyCode).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public IList<sp_GetPartyAllocationDocuments_Result> sp_GetPartyAllocationDocuments(string partyCode)
        {
            List<sp_GetPartyAllocationDocuments_Result> lst = null;
            try
            {
                lst = _context.sp_GetPartyAllocationDocuments(partyCode).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
        public IList<sp_FindMrvDetails_Result> sp_FindMrvDetails(string searchParam, int type)
        {
            List<sp_FindMrvDetails_Result> lst = null;
            try
            {
                lst = _context.sp_FindMrvDetails(searchParam, type).ToList();
            }
            catch (Exception)
            {
                //exception handling pending
            }
            return lst;
        }
    }
}
