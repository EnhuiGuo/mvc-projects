using System.Data.Linq.Mapping;

namespace SouthlandMetals.Dynamics.Domain.Models
{
    [Table(Name = "dbo.POP10310")]
    public class POP10310_ReceiptLine_Work
    {
        [Column]
        public string POPRCTNM { get; set; }
        [Column]
        public string RCPTLNNM { get; set; }
        [Column]
        public string PONUMBER { get; set; }
        [Column]
        public string ITEMNMBR { get; set; }
        [Column]
        public string ITEMDESC { get; set; }
        [Column]
        public string VNDITNUM { get; set; }
        [Column]
        public string VNDITDSC { get; set; }
        [Column]
        public string UMQTYINB { get; set; }
        [Column]
        public string ACTLSHIP { get; set; }
        [Column]
        public string COMMNTID { get; set; }
        [Column]
        public string INVINDX { get; set; }
        [Column]
        public string UOFM { get; set; }
        [Column]
        public string UNITCOST { get; set; }
        [Column]
        public string EXTDCOST { get; set; }
        [Column]
        public string LOCNCODE { get; set; }
        [Column]
        public string RcptLineNoteIDArray_1 { get; set; }
        [Column]
        public string RcptLineNoteIDArray_2 { get; set; }
        [Column]
        public string RcptLineNoteIDArray_3 { get; set; }
        [Column]
        public string RcptLineNoteIDArray_4 { get; set; }
        [Column]
        public string RcptLineNoteIDArray_5 { get; set; }
        [Column]
        public string RcptLineNoteIDArray_6 { get; set; }
        [Column]
        public string RcptLineNoteIDArray_7 { get; set; }
        [Column]
        public string RcptLineNoteIDArray_8 { get; set; }
        [Column]
        public string NONINVEN { get; set; }
        [Column]
        public string DECPLCUR { get; set; }
        [Column]
        public string DECPLQTY { get; set; }
        [Column]
        public string ITMTRKOP { get; set; }
        [Column]
        public string VCTNMTHD { get; set; }
        [Column]
        public string TRXSORCE { get; set; }
        [Column]
        public string JOBNUMBR { get; set; }
        [Column]
        public string COSTCODE { get; set; }
        [Column]
        public string COSTTYPE { get; set; }
        [Column]
        public string CURNCYID { get; set; }
        [Column]
        public string CURRNIDX { get; set; }
        [Column]
        public string RATETPID { get; set; }
        [Column]
        public string XCHGRATE { get; set; }
        [Column]
        public string RATECALC { get; set; }
        [Column]
        public string DENXRATE { get; set; }
        [Column]
        public string ORUNTCST { get; set; }
        [Column]
        public string OREXTCST { get; set; }
        [Column]
        public string ODECPLCU { get; set; }
        [Column]
        public string BOLPRONUMBER { get; set; }
        [Column]
        public string Capital_Item { get; set; }
        [Column]
        public string Product_Indicator { get; set; }
        [Column]
        public string Purchase_IV_Item_Taxable { get; set; }
        [Column]
        public string Purchase_Item_Tax_Schedu { get; set; }
        [Column]
        public string Purchase_Site_Tax_Schedu { get; set; }
        [Column]
        public string BSIVCTTL { get; set; }
        [Column]
        public string TAXAMNT { get; set; }
        [Column]
        public string ORTAXAMT { get; set; }
        [Column]
        public string BCKTXAMT { get; set; }
        [Column]
        public string OBTAXAMT { get; set; }
        [Column]
        public string Revalue_Inventory { get; set; }
        [Column]
        public string Tolerance_Percentage { get; set; }
        [Column]
        public string PURPVIDX { get; set; }
        [Column]
        public string Remaining_AP_Amount { get; set; }
        [Column]
        public string SHIPMTHD { get; set; }
        [Column]
        public string Landed_Cost_Group_ID { get; set; }
        [Column]
        public string Landed_Cost_Warnings { get; set; }
        [Column]
        public string BackoutTradeDiscTax { get; set; }
        [Column]
        public string OrigBackoutTradeDiscTax { get; set; }
        [Column]
        public string Landed_Cost { get; set; }
        [Column]
        public string Invoice_Match { get; set; }
        [Column]
        public string RCPTRETNUM { get; set; }
        [Column]
        public string RCPTRETLNNUM { get; set; }
        [Column]
        public string INVRETNUM { get; set; }
        [Column]
        public string INVRETLNNUM { get; set; }
        [Column]
        public string ISLINEInt32RA { get; set; }
        [Column]
        public string ProjNum { get; set; }
        [Column]
        public string CostCatID { get; set; }
    }
}
