using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZATCA
{
    class Validators
    {

     

        private string Validate_AmountCurrency(BaseData.Amount amount)
        {
            string validation_error = "";
            //if amount values is there, then there should be currencyID attribute,
            //if amount.value="", dont prompt to enter CurrencID
            if (amount != null && !amount.value.Equals(""))
            {
                if (amount.currencyID == null || amount.currencyID.Equals(""))
                {
                    validation_error = (ERR.custerr_Amount_must_have_currencyID.ToString());
                }
            }
            return validation_error;
        }

        public string Do_Validations(BaseData.Root Root, LOGTYPE tYP)

        {

            string validation_error = "";
            try
            {
                //if AllowanceCharge.amount present, then AllowanceCharge must contain ChargeIndicator  
                for (int i = 0; i < Root.Invoice.EInvoice.AllowanceCharge.Count; i++)
                {
                    BaseData.AllowanceCharge ac = Root.Invoice.EInvoice.AllowanceCharge[i];

                    if (!"".Equals(ac.Amount.value))
                    {
                        if ("".Equals(ac?.ChargeIndicator ?? ""))
                        {
                            validation_error = ERR.custerr_Doc_Level_AllowanceCharge_charge_indicator_missing.ToString();
                        }

                    }
                    else validation_error=Validate_AmountCurrency(ac?.Amount);

                }

                //All amount fields must have currencyID

                

                if ((Root?.Invoice?.EInvoice?.ProfileID ?? "").Equals(""))

                    throw new InvalidCastException("80001-" + "ProfileID is mandatory");



                if ((Root?.Invoice?.EInvoice?.ID.arb ?? "").Equals("") )

                    throw new InvalidCastException("80002-" + "Invoice Reference Number is mandatory");

                if ((Root?.Invoice?.EInvoice?.InvoiceTypeCode.name ?? "").Equals(""))

                    throw new InvalidCastException("80003-" + "InvoiceTypeCode name is mandatory");



                if ((Root?.Invoice?.EInvoice?.InvoiceTypeCode.value ?? "").Equals(""))

                    throw new InvalidCastException("80004-" + "InvoiceTypeCode value is mandatory");



                if ((Root?.Invoice?.EInvoice?.DocumentCurrencyCode ?? "").Equals("")) 
                    throw new InvalidCastException("80005-" + "DocumentCurrencyCode value is mandatory");

                if ((Root?.Invoice?.EInvoice?.TaxCurrencyCode ?? "").Equals(""))

                    throw new InvalidCastException("80006-" + "TaxCurrencyCode value is mandatory");
                //if ((Root?.Invoice?.EInvoice?.Note.arb ?? "").Equals(""))

                //    throw new InvalidCastException("80007-" + "Note value is mandatory");

                //if ((Root?.Invoice?.EInvoice?.OrderReference.ID.arb ?? "").Equals("")) 
                //    throw new InvalidCastException("80008-" + "OrderReference value is mandatory"); //Roopa

                if ((Root?.Invoice?.InvoiceType != InvoiceType.STD_INVOICE && Root?.Invoice?.InvoiceType != InvoiceType.SIMP_INVOICE) && (Root?.Invoice?.EInvoice?.BillingReference.Count ?? 0).Equals(0))
                    throw new InvalidCastException("80009-" + "BillingReference is mandatory");
                else if ((Root?.Invoice?.InvoiceType != InvoiceType.STD_INVOICE && Root?.Invoice?.InvoiceType != InvoiceType.SIMP_INVOICE) && (bool)(Root?.Invoice?.EInvoice?.BillingReference.Any(item => item.InvoiceDocumentReference.ID.arb == ""  )))
                    throw new InvalidCastException("80010-" + "InvoiceDocumentReference ID is mandatory");

                //if ((Root?.Invoice?.EInvoice?.ContractDocumentReference.Count ?? 0).Equals(0))
                //    throw new InvalidCastException("800011-" + "ContractDocumentReference is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.ContractDocumentReference.Any(item => item.ID.arb == "")))
                //    throw new InvalidCastException("80012-" + "ContractDocumentReference ID is mandatory");

                //if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PartyIdentification?.ID.schemeID ?? "").Equals(""))

                //    throw new InvalidCastException("80013-" + "AccountingSupplierParty-Party-PartyIdentification schemeID is mandatory");
                //if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PartyIdentification?.ID.value ?? "").Equals(""))

                //    throw new InvalidCastException("80014-" + "AccountingSupplierParty-Party-PartyIdentification value is mandatory");
                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.StreetName.arb ?? "").Equals("")  )

                        throw new InvalidCastException("80015-" + "AccountingSupplierParty-Party-PostalAddress StreetName is mandatory");
                //if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.AdditionalStreetName.arb ?? "").Equals(""))

                //    throw new InvalidCastException("80016-" + "AccountingSupplierParty-Party-PostalAddress AdditionalStreetName is mandatory");
                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.BuildingNumber.arb ?? "").Equals("") )

                    throw new InvalidCastException("80017-" + "AccountingSupplierParty-Party-PostalAddress BuildingNumber is mandatory");
                //if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.PlotIdentification.arb ?? "").Equals(""))

                //    throw new InvalidCastException("80018-" + "AccountingSupplierParty-Party-PostalAddress PlotIdentification is mandatory");
                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.CityName.arb ?? "").Equals("") )
                    throw new InvalidCastException("80019-" + "AccountingSupplierParty-Party-PostalAddress CityName is mandatory");

                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.PostalZone ?? "").Equals(""))

                    throw new InvalidCastException("80020-" + "AccountingSupplierParty-Party-PostalAddress PostalZone is mandatory");
                //if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.CountrySubentity.arb ?? "").Equals(""))

                //    throw new InvalidCastException("80021-" + "AccountingSupplierParty-Party-PostalAddress CountrySubentity is mandatory");
                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.CitySubdivisionName.arb ?? "").Equals("") )

                    throw new InvalidCastException("80022-" + "AccountingSupplierParty-Party-PostalAddress CitySubdivisionName is mandatory");
                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PostalAddress?.Country.IdentificationCode ?? "").Equals(""))

                    throw new InvalidCastException("80023-" + "AccountingSupplierParty-Party-PostalAddress Country-IdentificationCode is mandatory");
                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PartyTaxScheme?.CompanyID ?? "").Equals(""))

                    throw new InvalidCastException("80024-" + "AccountingSupplierParty-Party-PartyTaxScheme CompanyID is mandatory");
                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PartyTaxScheme?.TaxScheme?.ID.arb ?? "").Equals("")  )

                    throw new InvalidCastException("80025-" + "AccountingSupplierParty-Party-PartyTaxScheme-TaxScheme ID is mandatory");
                if ((Root?.Invoice?.EInvoice?.AccountingSupplierParty?.Party?.PartyLegalEntity?.RegistrationName.arb ?? "").Equals(""))

                    throw new InvalidCastException("80026-" + "AccountingSupplierParty-Party-PartyLegalEntity RegistrationName is mandatory");

                //if ((Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyIdentification?.ID.schemeID ?? "").Equals(""))

                //    throw new InvalidCastException("80027-" + "AccountingCustomerParty-Party-PartyIdentification schemeID is mandatory");
                //if ((Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyIdentification?.ID.value ?? "").Equals(""))

                //    throw new InvalidCastException("80028-" + "AccountingCustomerParty-Party-PartyIdentification value is mandatory");

                if ((Root?.Invoice.documentType == GatewayService.DOCTYPE.B2B) && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.Country.IdentificationCode ?? "").Equals(""))

                    throw new InvalidCastException("80037-" + "AccountingCustomerParty-Party-PostalAddress Country-IdentificationCode is mandatory");

                //ROOPA :Address details are  mandatory only if Country code is  "SA"
                if ((Root?.Invoice.documentType == GatewayService.DOCTYPE.B2B) && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.Country.IdentificationCode ?? "").Equals("SA"))  
                {
                    if ((Root?.Invoice.documentType == GatewayService.DOCTYPE.B2B) && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.StreetName.arb ?? "").Equals(""))  //ROOPA

                        throw new InvalidCastException("80029-" + "AccountingCustomerParty-Party-PostalAddress StreetName is mandatory");
                    //if ((Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.AdditionalStreetName.arb ?? "").Equals(""))

                    //    throw new InvalidCastException("80030-" + "AccountingCustomerParty-Party-PostalAddress AdditionalStreetName is mandatory");
                    if ((Root?.Invoice.documentType == GatewayService.DOCTYPE.B2B) && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.BuildingNumber.arb ?? "").Equals("")) //ROOPA

                        throw new InvalidCastException("80031-" + "AccountingCustomerParty-Party-PostalAddress BuildingNumber is mandatory");
                    //if ((Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.PlotIdentification.arb ?? "").Equals(""))

                    //    throw new InvalidCastException("80032-" + "AccountingCustomerParty-Party-PostalAddress PlotIdentification is mandatory");
                    if ((Root?.Invoice.documentType == GatewayService.DOCTYPE.B2B) && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.CityName.arb ?? "").Equals(""))

                        throw new InvalidCastException("80033-" + "AccountingCustomerParty-Party-PostalAddress CityName is mandatory");

                    if ((Root?.Invoice.documentType == GatewayService.DOCTYPE.B2B) && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.PostalZone ?? "").Equals(""))

                        throw new InvalidCastException("80034-" + "AccountingCustomerParty-Party-PostalAddress PostalZone is mandatory");
                    //if ((Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.CountrySubentity.arb ?? "").Equals(""))

                    //    throw new InvalidCastException("80035-" + "AccountingCustomerParty-Party-PostalAddress CountrySubentity is mandatory");
                    if ((Root?.Invoice.documentType == GatewayService.DOCTYPE.B2B) && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.CitySubdivisionName.arb ?? "").Equals(""))

                        throw new InvalidCastException("80036-" + "AccountingCustomerParty-Party-PostalAddress CitySubdivisionName is mandatory");
                }
                   
               
                 if (Root.Invoice.documentType==GatewayService.DOCTYPE.B2B &&
                    ((Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyTaxScheme?.CompanyID ?? "").Equals("") 
                       && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyIdentification?.ID?.value ?? "").Equals("") ))   //ROOPA FOR the error   : The other Buyer ID (BT-46) must present in the tax invoice and associated debit notes and credit notes (KSA-2, position 1 and 2 = 01), where the buyer VAT registration number or buyer group VAT registration number (BT-48) is not provided.

                    throw new InvalidCastException("80038-" + "AccountingCustomerParty-Party-PartyTaxScheme CompanyID  or AccountingCustomerParty-Party-PartyIdentification-ID-value is mandatory");
                //if ((Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyTaxScheme?.TaxScheme?.ID.arb ?? "").Equals(""))

                //    throw new InvalidCastException("80039-" + "AccountingCustomerParty-Party-PartyTaxScheme-TaxScheme ID is mandatory");
             
                //if ((Root?.Invoice.documentType == GatewayService.DOCTYPE.B2B ||
                //     (Root?.Invoice.documentType == GatewayService.DOCTYPE.B2C && (bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(ttt => ttt.TaxCategory?.TaxExemptionReasonCode == "VATEX-SA-EDU" || ttt.TaxCategory?.TaxExemptionReasonCode == "VATEX-SA-HEA")))))
                //    && (Root?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyLegalEntity?.RegistrationName.arb ?? "").Equals(""))   //ROOPA

                //    throw new InvalidCastException("80040-" + "AccountingCustomerParty-Party-PartyLegalEntity RegistrationName is mandatory");

                //if ((Root?.Invoice?.EInvoice?.Delivery.Count ?? 0).Equals(0))
                //    throw new InvalidCastException("80041-" + "Delivery is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.Delivery.Any(item => item.ActualDeliveryDate == "")))
                //    throw new InvalidCastException("80042-" + "Delivery ActualDeliveryDate is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.Delivery.Any(item => item.LatestDeliveryDate == "")))
                //    throw new InvalidCastException("80043-" + "Delivery LatestDeliveryDate is mandatory");

                if ((Root?.Invoice?.InvoiceType != InvoiceType.STD_INVOICE && Root?.Invoice?.InvoiceType != InvoiceType.SIMP_INVOICE) && (Root?.Invoice?.EInvoice?.PaymentMeans.Count ?? 0).Equals(0))
                    throw new InvalidCastException("80044-" + "PaymentMeans is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.PaymentMeans.Any(item => item.PaymentMeansCode == "")))
                //    throw new InvalidCastException("80045-" + "PaymentMeans PaymentMeansCode is mandatory");

                else if ((Root?.Invoice?.InvoiceType != InvoiceType.STD_INVOICE && 
                    Root?.Invoice?.InvoiceType != InvoiceType.SIMP_INVOICE)
                    && (bool)(Root?.Invoice?.EInvoice?.PaymentMeans.Any(item => item.InstructionNote.arb == "" ))  //Roopa
                    )
                    throw new InvalidCastException("80046-" + "PaymentMeans InstructionNote is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.PaymentMeans.Any(item => item.PayeeFinancialAccount?.PaymentNote.arb == "")))
                //    throw new InvalidCastException("80047-" + "PaymentMeans PayeeFinancialAccount is mandatory");

                //if ((Root?.Invoice?.EInvoice?.AllowanceCharge.Count ?? 0).Equals(0))
                //    throw new InvalidCastException("80048-" + "AllowanceCharge is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.ChargeIndicator == "")))
                //    throw new InvalidCastException("80049-" + "AllowanceCharge ChargeIndicator is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.MultiplierFactorNumeric == "")))
                //    throw new InvalidCastException("80050-" + "AllowanceCharge MultiplierFactorNumeric is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.Amount.value == "")))
                //    throw new InvalidCastException("80051-" + "AllowanceCharge Amount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.Amount.currencyID == "")))
                //    throw new InvalidCastException("80052-" + "AllowanceCharge Amount currencyID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.BaseAmount.value == "")))
                //    throw new InvalidCastException("80053-" + "AllowanceCharge BaseAmount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.BaseAmount.currencyID == "")))
                //    throw new InvalidCastException("80054-" + "AllowanceCharge BaseAmount currencyID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.TaxCategory.ID.arb == "")))
                //    throw new InvalidCastException("80055-" + "AllowanceCharge TaxCategory ID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.TaxCategory.Percent == "")))
                //    throw new InvalidCastException("80056-" + "AllowanceCharge TaxCategory Percent is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.AllowanceCharge.Any(item => item.TaxCategory.TaxScheme.ID.arb == "")))
                //    throw new InvalidCastException("80057-" + "AllowanceCharge BaseAmount TaxScheme ID is mandatory");

                if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.LineExtensionAmount.value ?? "").Equals(""))

                    throw new InvalidCastException("80058-" + "LegalMonetaryTotal-LineExtensionAmount value is mandatory");
                if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.LineExtensionAmount.currencyID ?? "").Equals(""))

                    throw new InvalidCastException("80059-" + "LegalMonetaryTotal-LineExtensionAmount currencyID is mandatory");
                //if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.AllowanceTotalAmount.value ?? "").Equals(""))

                //    throw new InvalidCastException("80060-" + "LegalMonetaryTotal-AllowanceTotalAmount value is mandatory");
                //if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.AllowanceTotalAmount.currencyID ?? "").Equals(""))

                //    throw new InvalidCastException("80061-" + "LegalMonetaryTotal-AllowanceTotalAmount currencyID is mandatory");
                if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.TaxExclusiveAmount.value ?? "").Equals(""))

                    throw new InvalidCastException("80062-" + "LegalMonetaryTotal-TaxExclusiveAmount value is mandatory");
                if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.TaxExclusiveAmount.currencyID ?? "").Equals(""))

                    throw new InvalidCastException("80063-" + "LegalMonetaryTotal-TaxExclusiveAmount currencyID is mandatory");
                if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.TaxInclusiveAmount.value ?? "").Equals(""))

                    throw new InvalidCastException("80064-" + "LegalMonetaryTotal-TaxInclusiveAmount value is mandatory");
                if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.TaxInclusiveAmount.currencyID ?? "").Equals(""))

                    throw new InvalidCastException("80065-" + "LegalMonetaryTotal-TaxInclusiveAmount currencyID is mandatory");
                //if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.PrepaidAmount.value ?? "").Equals(""))

                //    throw new InvalidCastException("80066-" + "LegalMonetaryTotal-PrepaidAmount value is mandatory");
                //if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.PrepaidAmount.currencyID ?? "").Equals(""))

                //    throw new InvalidCastException("80067-" + "LegalMonetaryTotal-PrepaidAmount currencyID is mandatory");
                if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.PayableAmount.value ?? "").Equals(""))

                    throw new InvalidCastException("80068-" + "LegalMonetaryTotal-PayableAmount value is mandatory");
                //if ((Root?.Invoice?.EInvoice?.LegalMonetaryTotal?.PayableAmount.currencyID ?? "").Equals(""))

                //    throw new InvalidCastException("80069-" + "LegalMonetaryTotal-PayableAmount currencyID is mandatory");

                //tax is not applicable for Export Invoice. So tax details will not be available.TaxTotal array will be empty only
                //if ((Root?.Invoice?.EInvoice?.TaxTotal.Count ?? 0).Equals(0))
                //    throw new InvalidCastException("80070-" + "TaxTotal is mandatory");

                //else
                if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxAmount.value == "")))
                    throw new InvalidCastException("80071-" + "TaxTotal TaxAmount value is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxAmount.currencyID == "")))
                    throw new InvalidCastException("80072-" + "TaxTotal TaxAmount currencyID is mandatory");

                if ((Root?.Invoice?.EInvoice?.TaxTotal.Count ?? 0).Equals(2))
                {
                      if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal[1].TaxSubtotal.Any((itm => itm?.TaxableAmount.value == ""))))  //ROOPA
                        throw new InvalidCastException("80073-" + "TaxTotal TaxSubtotal TaxableAmount value is mandatory");
                    else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal[1].TaxSubtotal.Any((itm => itm?.TaxableAmount.currencyID == ""))))  //ROOPA
                        throw new InvalidCastException("80074-" + "TaxTotal TaxSubtotal TaxableAmount currencyID is mandatory");
                    else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal[1].TaxSubtotal.Any(itm => itm.TaxAmount.value == "")))
                        throw new InvalidCastException("80075-" + "TaxTotal TaxSubtotal TaxAmount value is mandatory");
                    else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal[1].TaxSubtotal.Any(itm => itm.TaxAmount.currencyID == ""))) //ROOPA
                        throw new InvalidCastException("80076-" + "TaxTotal TaxSubtotal TaxAmount currencyID is mandatory");
                    else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal[1].TaxSubtotal.Any(itm => itm.TaxCategory.ID.arb.Equals("")))) //ROOPA
                        throw new InvalidCastException("80077-" + "TaxTotal TaxSubtotal TaxCategory ID is mandatory");
                    else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal[1].TaxSubtotal.Any(itm => itm.TaxCategory.Percent == ""))) //ROOPA
                        throw new InvalidCastException("80078-" + "TaxTotal TaxSubtotal TaxCategory Percent is mandatory");
                    else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal[1].TaxSubtotal.Any(itm => itm.TaxCategory.TaxScheme.ID.arb.Equals("")))) //ROOPA
                        throw new InvalidCastException("80081-" + "TaxTotal TaxSubtotal TaxCategory TaxScheme ID is mandatory");
                }
               
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(itm => itm?.TaxableAmount.value == ""))))
                //    throw new InvalidCastException("80073-" + "TaxTotal TaxSubtotal TaxableAmount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(itm => itm.TaxableAmount.currencyID == ""))))
                //    throw new InvalidCastException("80074-" + "TaxTotal TaxSubtotal TaxableAmount currencyID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(itm => itm.TaxAmount.value == ""))))
                //    throw new InvalidCastException("80075-" + "TaxTotal TaxSubtotal TaxAmount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(itm => itm.TaxAmount.currencyID == ""))))
                //    throw new InvalidCastException("80076-" + "TaxTotal TaxSubtotal TaxAmount currencyID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(itm => itm.TaxCategory.ID.arb == ""))))
                //    throw new InvalidCastException("80077-" + "TaxTotal TaxSubtotal TaxCategory ID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(itm => itm.TaxCategory.Percent == ""))))
                //    throw new InvalidCastException("80078-" + "TaxTotal TaxSubtotal TaxCategory Percent is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(item => item.TaxCategory.TaxExemptionReasonCode == ""))))
                //    throw new InvalidCastException("80079-" + "TaxTotal TaxSubtotal TaxCategory TaxExemptionReasonCode is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(item => item.TaxCategory.TaxExemptionReason.arb == ""))))
                //    throw new InvalidCastException("80080-" + "TaxTotal TaxSubtotal TaxCategory TaxExemptionReason is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.TaxTotal.Any(item => item.TaxSubtotal.Any(itm => itm.TaxCategory.TaxScheme.ID.arb == ""))))
                //    throw new InvalidCastException("80081-" + "TaxTotal TaxSubtotal TaxCategory TaxScheme ID is mandatory");

                if ((Root?.Invoice?.EInvoice?.InvoiceLine.Count ?? 0).Equals(0))
                    throw new InvalidCastException("80082-" + "InvoiceLine is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.ID == "")))
                    throw new InvalidCastException("80083-" + "InvoiceLine ID is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.InvoicedQuantity.value == "")))
                    throw new InvalidCastException("80084-" + "InvoiceLine InvoicedQuantity value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.InvoicedQuantity.unitCode == "")))
                //    throw new InvalidCastException("80085-" + "InvoiceLine InvoicedQuantity unitCode is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.LineExtensionAmount.value == "")))
                    throw new InvalidCastException("80086-" + "InvoiceLine LineExtensionAmount value is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.LineExtensionAmount.currencyID == "")))
                    throw new InvalidCastException("80087-" + "InvoiceLine LineExtensionAmount currencyID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.AllowanceCharge.Any(item => item.ChargeIndicator == ""))))
                //    throw new InvalidCastException("80088-" + "InvoiceLine AllowanceCharge TaxableAmount ChargeIndicator is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.AllowanceCharge.Any(item => item.MultiplierFactorNumeric == ""))))
                //    throw new InvalidCastException("80089-" + "InvoiceLine AllowanceCharge TaxableAmount MultiplierFactorNumeric is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.AllowanceCharge.Any(item => item.Amount.value == ""))))
                //    throw new InvalidCastException("80090-" + "InvoiceLine AllowanceCharge Amount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.AllowanceCharge.Any(item => item.Amount.currencyID == ""))))
                //    throw new InvalidCastException("80091-" + "InvoiceLine AllowanceCharge Amount currencyID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.AllowanceCharge.Any(item => item.BaseAmount.value == ""))))
                //    throw new InvalidCastException("80092-" + "InvoiceLine AllowanceCharge BaseAmount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.AllowanceCharge.Any(item => item.BaseAmount.currencyID == ""))))
                //    throw new InvalidCastException("80093-" + "InvoiceLine AllowanceCharge BaseAmount currencyID is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.TaxTotal?.TaxAmount.value == "")) && Root.Invoice.documentType==GatewayService.DOCTYPE.B2B)
                    throw new InvalidCastException("80094-" + "InvoiceLine TaxTotal TaxAmount value is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.TaxTotal?.TaxAmount.currencyID == "")))
                    throw new InvalidCastException("80095-" + "InvoiceLine TaxTotal TaxAmount currencyID is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.TaxTotal?.RoundingAmount.value == "")))
                    throw new InvalidCastException("80096-" + "InvoiceLine TaxTotal RoundingAmount value is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.TaxTotal?.RoundingAmount.currencyID == "")))
                    throw new InvalidCastException("80097-" + "InvoiceLine TaxTotal RoundingAmount currencyID is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Item?.Name.arb == ""  )))
                    throw new InvalidCastException("80098-" + "InvoiceLine Item Name is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Item?.BuyersItemIdentification.ID.arb == "")))
                //    throw new InvalidCastException("80099-" + "InvoiceLine Item BuyersItemIdentification ID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Item?.SellersItemIdentification.ID.arb == "")))
                //    throw new InvalidCastException("80100-" + "InvoiceLine Item SellersItemIdentification ID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Item?.StandardItemIdentification.ID.arb == "")))
                //    throw new InvalidCastException("80101-" + "InvoiceLine Item StandardItemIdentification ID is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Item?.ClassifiedTaxCategory.ID.arb == ""  )))
                    throw new InvalidCastException("80102-" + "InvoiceLine Item ClassifiedTaxCategory ID is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Item?.ClassifiedTaxCategory.Percent == "")))
                    throw new InvalidCastException("80103-" + "InvoiceLine Item ClassifiedTaxCategory Percent is mandatory");
                else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Item?.ClassifiedTaxCategory?.TaxScheme.ID.arb == ""  )))
                    throw new InvalidCastException("80104-" + "InvoiceLine Item ClassifiedTaxCategory TaxScheme ID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.PriceAmount.value == "")))
                //    throw new InvalidCastException("80105-" + "InvoiceLine Price PriceAmount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.PriceAmount.currencyID == "")))
                //    throw new InvalidCastException("80106-" + "InvoiceLine Price PriceAmount currencyID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.BaseQuantity.value == "")))
                //    throw new InvalidCastException("80107-" + "InvoiceLine Price BaseQuantity value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.BaseQuantity.unitCode == "")))
                //    throw new InvalidCastException("80108-" + "InvoiceLine Price BaseQuantity unitCode is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.AllowanceCharge.ChargeIndicator == "")))
                //    throw new InvalidCastException("80109-" + "InvoiceLine Price AllowanceCharge ChargeIndicator is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.AllowanceCharge.Amount.value == "")))
                //    throw new InvalidCastException("80110-" + "InvoiceLine Price AllowanceCharge Amount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.AllowanceCharge.Amount.currencyID == "")))
                //    throw new InvalidCastException("80111-" + "InvoiceLine Price AllowanceCharge Amount currencyID is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.AllowanceCharge.BaseAmount.value == "")))
                //    throw new InvalidCastException("80112-" + "InvoiceLine Price AllowanceCharge BaseAmount value is mandatory");
                //else if ((bool)(Root?.Invoice?.EInvoice?.InvoiceLine.Any(item => item.Price?.AllowanceCharge.BaseAmount.currencyID == "")))
                //    throw new InvalidCastException("80113-" + "InvoiceLine Price AllowanceCharge BaseAmount currencyID is mandatory");
                //All fields in json , but not available in Exel are mandatory

            }
            catch (Exception ex)
            {
                new Basepage().log("Do_Validations err:" + ex.ToString(), 1, tYP);
                validation_error = ex.Message;

            }

            return validation_error;



        }
    }
}