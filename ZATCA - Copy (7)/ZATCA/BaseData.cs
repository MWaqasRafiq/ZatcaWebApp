using GatewayService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ZATCA
{
  public  class BaseData
    {

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class AccountingCustomerParty
        {
            public Party Party { get; set; }
        }

        public class AccountingSupplierParty
        {
            public Party Party { get; set; }
        }

        public class AdditionalStreetName
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class AllowanceCharge
        {
            public string ChargeIndicator { get; set; }
            public BaseAmount BaseAmount { get; set; }
            public Amount Amount { get; set; }
            public string MultiplierFactorNumeric { get; set; }
            public AllowanceChargeReason AllowanceChargeReason { get; set; }
            public string AllowanceChargeReasonCode { get; set; }
            public TaxCategory TaxCategory { get; set; }
        }

        public class AllowanceChargeReason
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class AllowanceTotalAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class Amount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class BaseAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class BaseQuantity
        {
            public string unitCode { get; set; }
            public string value { get; set; }
        }

        public class BillingReference
        {
            public InvoiceDocumentReference InvoiceDocumentReference { get; set; }
        }

        public class BuildingNumber
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class BuyersItemIdentification
        {
            public ID ID { get; set; }
        }

        public class CityName
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class CitySubdivisionName
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class ClassifiedTaxCategory
        {
            public ID ID { get; set; }
            public string Percent { get; set; }
            public TaxScheme TaxScheme { get; set; }
        }

        public class ContractDocumentReference
        {
            public ID ID { get; set; }
        }

        public class Country
        {
            public string IdentificationCode { get; set; }
        }

        public class CountrySubentity
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class CustomFields
        {
            public string TotalBoxes { get; set; }
            public string TotalWreight { get; set; }
        }

        public class Delivery
        {
            public string ActualDeliveryDate { get; set; }
            public string LatestDeliveryDate { get; set; }
        }

        public class EInvoice
        {
            public string ProfileID { get; set; }
            public ID ID { get; set; }
            public InvoiceTypeCode InvoiceTypeCode { get; set; }
            public string IssueDate { get; set; }
            public string IssueTime { get; set; }
            public List<Delivery> Delivery { get; set; }
            public List<BillingReference> BillingReference { get; set; }
            public OrderReference OrderReference { get; set; }
            public List<ContractDocumentReference> ContractDocumentReference { get; set; }
            public string DocumentCurrencyCode { get; set; }
            public string TaxCurrencyCode { get; set; }
            public AccountingSupplierParty AccountingSupplierParty { get; set; }
            public AccountingCustomerParty AccountingCustomerParty { get; set; }
            public List<InvoiceLine> InvoiceLine { get; set; }
            public List<AllowanceCharge> AllowanceCharge { get; set; } 
            public List<TaxTotal> TaxTotal { get; set; }
            public LegalMonetaryTotal LegalMonetaryTotal { get; set; }
            public List<PaymentMean> PaymentMeans { get; set; }
            public Note Note { get; set; }

           
        }

        public class ID
        {
            public string en { get; set; }
            public string ar { get; set; }
            [JsonIgnore]   public string arb   // property
            {
                get { return (ar?.Trim() ?? "") == "" ? en : ar; }
            }
            public string schemeID { get; set; }
            public string schemeAgencyID { get; set; }
            public string value { get; set; }
        }

        public class InstructionNote
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class InvoiceDocumentReference
        {
            public ID ID { get; set; }
        }

        public class InvoicedQuantity
        {
            public string unitCode { get; set; }
            public string value { get; set; }
        }

        public class InvoiceLine
        {
            public string ID { get; set; }
            public Item Item { get; set; }
            public Price Price { get; set; }
            public InvoicedQuantity InvoicedQuantity { get; set; }
            public List<AllowanceCharge> AllowanceCharge { get; set; }
            public LineExtensionAmount LineExtensionAmount { get; set; }
            public TaxTotal TaxTotal { get; set; }
        }

        public class InvoiceTypeCode
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class Item
        {
            public Name Name { get; set; }
            public BuyersItemIdentification BuyersItemIdentification { get; set; }
            public SellersItemIdentification SellersItemIdentification { get; set; }
            public StandardItemIdentification StandardItemIdentification { get; set; }
            public ClassifiedTaxCategory ClassifiedTaxCategory { get; set; }
        }

        public class LegalMonetaryTotal
        {
            public LineExtensionAmount LineExtensionAmount { get; set; }
            public AllowanceTotalAmount AllowanceTotalAmount { get; set; }
            public TaxExclusiveAmount TaxExclusiveAmount { get; set; }
            public TaxInclusiveAmount TaxInclusiveAmount { get; set; }
            public ChargeTotalAmount ChargeTotalAmount { get; set; }
            public PrepaidAmount PrepaidAmount { get; set; }
            public PayableAmount PayableAmount { get; set; }
        }

        public class LineExtensionAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class Name
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class Note
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class OrderReference
        {
            public ID ID { get; set; }
        }

        public class Party
        {
            public PartyLegalEntity PartyLegalEntity { get; set; }
            public PartyTaxScheme PartyTaxScheme { get; set; }
            public PartyIdentification PartyIdentification { get; set; }
            public PostalAddress PostalAddress { get; set; }
        }

        public class PartyIdentification
        {
            public ID ID { get; set; }
        }

        public class PartyLegalEntity
        {
            public RegistrationName RegistrationName { get; set; }
        }

        public class PartyTaxScheme
        {
            public string CompanyID { get; set; }
            public TaxScheme TaxScheme { get; set; }
        }

        public class PayableAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class PayeeFinancialAccount
        {
            public PaymentNote PaymentNote { get; set; }
            public ID ID { get; set; }
        }

        public class PaymentMean
        {
            public string PaymentMeansCode { get; set; }
            public InstructionNote InstructionNote { get; set; }
            public PayeeFinancialAccount PayeeFinancialAccount { get; set; }
        }

        public class PaymentNote
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class PlotIdentification
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class PostalAddress
        {
            public StreetName StreetName { get; set; }
            public AdditionalStreetName AdditionalStreetName { get; set; }
            public BuildingNumber BuildingNumber { get; set; }
            public PlotIdentification PlotIdentification { get; set; }
            public CityName CityName { get; set; }
            public CitySubdivisionName CitySubdivisionName { get; set; }
            public string PostalZone { get; set; }
            public CountrySubentity CountrySubentity { get; set; }
            public Country Country { get; set; }
        }

        public class PrepaidAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class Price
        {
            public AllowanceCharge AllowanceCharge { get; set; }
            public PriceAmount PriceAmount { get; set; }
            public BaseQuantity BaseQuantity { get; set; }
        }

        public class PriceAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class RegistrationName
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class Root
        { 
        public Invoice Invoice { get; set; }
        }
        public class Invoice
        {
            
            public EInvoice EInvoice { get; set; }
            public CustomFields CustomFields { get; set; }

            //myfields
            public DOCTYPE documentType { get; set; }
            public string InvoiceUUID { get; set; }
            public string ICV { get; set; }
            public string PIH { get; set; }
            public InvoiceType InvoiceType { get; set; }

        }

        public class RoundingAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class SellersItemIdentification
        {
            public ID ID { get; set; }
        }

        public class StandardItemIdentification
        {
            public ID ID { get; set; }
        }

        public class StreetName
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class TaxableAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class TaxAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class TaxCategory
        {
            public ID ID { get; set; }
            public string Percent { get; set; }
            public TaxScheme TaxScheme { get; set; }
            public string TaxExemptionReasonCode { get; set; }
            public TaxExemptionReason TaxExemptionReason { get; set; }
        }

        public class TaxExclusiveAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class TaxExemptionReason
        {
            public string en { get; set; }
             public string ar { get; set; }
             [JsonIgnore]  public string arb   // property
            {
                get { return (ar?.Trim()??"")==""?en:ar; }    
            }
        }

        public class TaxInclusiveAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }

        public class ChargeTotalAmount
        {
            public string currencyID { get; set; }
            public string value { get; set; }
        }


        public class TaxScheme
        {
            public ID ID { get; set; } 
        }

        public class TaxSubtotal
        {
            public TaxableAmount TaxableAmount { get; set; }
            public TaxAmount TaxAmount { get; set; }
            public TaxCategory TaxCategory { get; set; }
        }

        public class TaxTotal
        {
            public TaxAmount TaxAmount { get; set; }
            public RoundingAmount RoundingAmount { get; set; }
            public List<TaxSubtotal> TaxSubtotal { get; set; }
        }


    }
}
//public string generateXml_1(BaseData.Root baseData, LOGTYPE typ)
//{
//    string xml = null;
//    try
//    {


//        XmlDocument doc = new XmlDocument();
//        XmlElement rootXmlElement = doc.CreateElement("Invoice", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");

//        XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(doc.NameTable);




//      //  xmlNamespaceManager.AddNamespace("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
//        xmlNamespaceManager.AddNamespace("xmlns:cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
//        xmlNamespaceManager.AddNamespace("xmlns:cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
//        xmlNamespaceManager.AddNamespace("xmlns:ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");

//        //  rootXmlElement.SetAttribute("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
//        doc.AppendChild(rootXmlElement);


//        // staff XmlElements
//        XmlElement profileId = doc.CreateElement("cbc:ProfileID"); profileId.Prefix = "cbc";
//        profileId.AppendChild(doc.CreateTextNode("reporting:1.0"));
//        rootXmlElement.AppendChild(profileId);

//        XmlElement id = doc.CreateElement("cbc:ID");
//        id.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.ID.en));
//        rootXmlElement.AppendChild(id);

//        XmlElement uuid = doc.CreateElement("cbc:UUID");
//        uuid.AppendChild(doc.CreateTextNode(baseData.Invoice.InvoiceUUID));
//        rootXmlElement.AppendChild(uuid);

//        XmlElement issueDate = doc.CreateElement("cbc:IssueDate");
//        issueDate.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.IssueDate));
//        rootXmlElement.AppendChild(issueDate);

//        XmlElement issueTime = doc.CreateElement("cbc:IssueTime");
//        issueTime.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.IssueTime));
//        rootXmlElement.AppendChild(issueTime);

//        XmlElement invoiceTypeCode = doc.CreateElement("cbc:InvoiceTypeCode");
//        invoiceTypeCode.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.InvoiceTypeCode.value));
//        System.Xml.XmlAttribute nameAttr = doc.CreateAttribute("name");
//        nameAttr.Value = (baseData.Invoice.EInvoice.InvoiceTypeCode.name);
//        invoiceTypeCode.SetAttributeNode(nameAttr);
//        rootXmlElement.AppendChild(invoiceTypeCode);

//        XmlElement documentCurrencyCode = doc.CreateElement("cbc:DocumentCurrencyCode");
//        documentCurrencyCode.AppendChild(doc.CreateTextNode("SAR"));
//        rootXmlElement.AppendChild(documentCurrencyCode);

//        XmlElement taxCurrencyCode = doc.CreateElement("cbc:TaxCurrencyCode");
//        taxCurrencyCode.AppendChild(doc.CreateTextNode("SAR"));
//        rootXmlElement.AppendChild(taxCurrencyCode);

//        if (baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN)
//        {
//            XmlElement billingReference = doc.CreateElement("cac:BillingReference");
//            for (int i = 0; i < baseData.Invoice.EInvoice.BillingReference.Count; i++)
//            {
//                BaseData.BillingReference bb = baseData.Invoice.EInvoice.BillingReference[i];
//                XmlElement invoiceDocumentReference = doc.CreateElement("cac:InvoiceDocumentReference");
//                XmlElement invoiceDocumentReferenceId = doc.CreateElement("cbc:ID");
//                invoiceDocumentReferenceId.AppendChild(doc.CreateTextNode(bb.InvoiceDocumentReference.ID.en));
//                invoiceDocumentReference.AppendChild(invoiceDocumentReferenceId);
//                billingReference.AppendChild(invoiceDocumentReference);
//            }

//            //if (!"".Equals(PropertyAccessor.valueOrDefault(() -> baseData.getBillingReference().getIssueDate(), ""))) {
//            //                     XmlElement invoiceDocumentReferenceIssueDate = doc.CreateElement("cbc:IssueDate");
//            //                     invoiceDocumentReferenceIssueDate.AppendChild(doc.CreateTextNode(baseData.getBillingReference().getIssueDate()));
//            //                     invoiceDocumentReference.AppendChild(invoiceDocumentReferenceIssueDate);
//            //}

//            rootXmlElement.AppendChild(billingReference);
//        }

//        XmlElement additionalDocumentReferenceICV = doc.CreateElement("cac:AdditionalDocumentReference");
//        XmlElement additionalDocumentReferenceICVId = doc.CreateElement("cbc:ID");
//        additionalDocumentReferenceICVId.AppendChild(doc.CreateTextNode("ICV"));
//        additionalDocumentReferenceICV.AppendChild(additionalDocumentReferenceICVId);
//        XmlElement additionalDocumentReferenceICVUuid = doc.CreateElement("cbc:UUID");
//        additionalDocumentReferenceICVUuid.AppendChild(doc.CreateTextNode(baseData.Invoice.InvoiceUUID));
//        additionalDocumentReferenceICV.AppendChild(additionalDocumentReferenceICVUuid);
//        rootXmlElement.AppendChild(additionalDocumentReferenceICV);

//        XmlElement additionalDocumentReferencePIH = doc.CreateElement("cac:AdditionalDocumentReference");
//        XmlElement additionalDocumentReferencePIHId = doc.CreateElement("cbc:ID");
//        additionalDocumentReferencePIHId.AppendChild(doc.CreateTextNode("PIH"));
//        additionalDocumentReferencePIH.AppendChild(additionalDocumentReferencePIHId);
//        XmlElement additionalDocumentReferencePIHAttachment = doc.CreateElement("cac:Attachment");
//        XmlElement embeddedDocumentBinaryObjectPIH = doc.CreateElement("cbc:EmbeddedDocumentBinaryObject");
//        embeddedDocumentBinaryObjectPIH.AppendChild(doc.CreateTextNode(baseData.Invoice.PIH));
//        System.Xml.XmlAttribute mimeCodePIH = doc.CreateAttribute("mimeCode");
//        mimeCodePIH.Value = ("text/plain");
//        embeddedDocumentBinaryObjectPIH.SetAttributeNode(mimeCodePIH);
//        additionalDocumentReferencePIHAttachment.AppendChild(embeddedDocumentBinaryObjectPIH);
//        additionalDocumentReferencePIH.AppendChild(additionalDocumentReferencePIHAttachment);
//        rootXmlElement.AppendChild(additionalDocumentReferencePIH);

//        // XmlElement additionalDocumentReferenceQR = doc.CreateElement("cac:AdditionalDocumentReference");
//        // XmlElement additionalDocumentReferenceQRId = doc.CreateElement("cbc:ID");
//        //  additionalDocumentReferenceQRId.AppendChild(doc.CreateTextNode("QR"));
//        //additionalDocumentReferenceQR.AppendChild(additionalDocumentReferenceQRId);
//        // XmlElement additionalDocumentReferenceQRAttachment = doc.CreateElement("cac:Attachment");
//        //     XmlElement embeddedDocumentBinaryObjectQR = doc.CreateElement("cbc:EmbeddedDocumentBinaryObject");
//        //     embeddedDocumentBinaryObjectQR.AppendChild(doc.CreateTextNode(baseData.getAdditionalDocumentReference().getEmbeddedDocumentBinaryObject()));
//        //        Attr mimeCodeQR = doc.CreateAttribute("mimeCode");
//        //        mimeCodeQR.Value=("text/plain");
//        //        embeddedDocumentBinaryObjectPIH.SetAttributeNode(mimeCodeQR);
//        //    additionalDocumentReferenceQRAttachment.AppendChild(embeddedDocumentBinaryObjectQR);
//        // additionalDocumentReferenceQR.AppendChild(additionalDocumentReferenceQRAttachment);
//        // rootXmlElement.AppendChild(additionalDocumentReferenceQR);

//        XmlElement accountingSupplierParty = doc.CreateElement("cac:AccountingSupplierParty");
//        XmlElement party = doc.CreateElement("cac:Party");
//        // if (!"".Equals(baseData.getAccountingSupplierParty().getPartyIdentification().getId(), "")))
//        {
//            XmlElement partyIdentification = doc.CreateElement("cac:PartyIdentification");
//            XmlElement partyIdentificationId = doc.CreateElement("cbc:ID");
//            partyIdentificationId.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PartyIdentification.ID.en));
//            //    if (!"".Equals(baseData.getAccountingSupplierParty().getPartyIdentification().getIdSchemeId(), "")))
//            {
//                XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
//                schemeIDAttr.Value = (baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PartyIdentification.ID.schemeID);
//                partyIdentificationId.SetAttributeNode(schemeIDAttr);
//            }
//            partyIdentification.AppendChild(partyIdentificationId);
//            party.AppendChild(partyIdentification);
//        }
//        XmlElement postalAddress = doc.CreateElement("cac:PostalAddress");
//        XmlElement streetName = doc.CreateElement("cbc:StreetName");
//        streetName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.StreetName.en));
//        postalAddress.AppendChild(streetName);
//        XmlElement buildingNumber = doc.CreateElement("cbc:BuildingNumber");
//        buildingNumber.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.BuildingNumber.en));
//        postalAddress.AppendChild(buildingNumber);
//        XmlElement plotIdentification = doc.CreateElement("cbc:PlotIdentification");
//        plotIdentification.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.PlotIdentification.en));
//        postalAddress.AppendChild(plotIdentification);
//        XmlElement citySubdivisionName = doc.CreateElement("cbc:CitySubdivisionName");
//        citySubdivisionName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.CitySubdivisionName.en));
//        postalAddress.AppendChild(citySubdivisionName);
//        XmlElement cityName = doc.CreateElement("cbc:CityName");
//        cityName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.CityName.en));
//        postalAddress.AppendChild(cityName);
//        XmlElement postalZone = doc.CreateElement("cbc:PostalZone");
//        postalZone.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.PostalZone));
//        postalAddress.AppendChild(postalZone);
//        //             XmlElement countrySubentity = doc.CreateElement("cbc:CountrySubentity");
//        //             countrySubentity.AppendChild(doc.CreateTextNode(baseData.getAccountingSupplierParty().getPostalAddress().getCountrySubentity()));
//        //             postalAddress.AppendChild(countrySubentity);

//        XmlElement country = doc.CreateElement("cac:Country");
//        XmlElement identificationCode = doc.CreateElement("cbc:IdentificationCode");
//        identificationCode.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.Country.IdentificationCode));
//        country.AppendChild(identificationCode);
//        postalAddress.AppendChild(country);
//        party.AppendChild(postalAddress);
//        XmlElement partyTaxScheme = doc.CreateElement("cac:PartyTaxScheme");
//        XmlElement partyTaxSchemePartyCompanyID = doc.CreateElement("cbc:CompanyID");
//        partyTaxSchemePartyCompanyID.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PartyTaxScheme.CompanyID));
//        partyTaxScheme.AppendChild(partyTaxSchemePartyCompanyID);
//        XmlElement taxScheme = doc.CreateElement("cac:TaxScheme");
//        XmlElement taxSchemeID = doc.CreateElement("cbc:ID");
//        taxSchemeID.AppendChild(doc.CreateTextNode("VAT"));
//        taxScheme.AppendChild(taxSchemeID);
//        partyTaxScheme.AppendChild(taxScheme);
//        party.AppendChild(partyTaxScheme);
//        XmlElement partyLegalEntity = doc.CreateElement("cac:PartyLegalEntity");
//        XmlElement partyLegalEntityRegistrationName = doc.CreateElement("cbc:RegistrationName");
//        partyLegalEntityRegistrationName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PartyLegalEntity.RegistrationName.en));
//        partyLegalEntity.AppendChild(partyLegalEntityRegistrationName);
//        party.AppendChild(partyLegalEntity);
//        accountingSupplierParty.AppendChild(party);
//        rootXmlElement.AppendChild(accountingSupplierParty);

//        //needs to be present for all, but   pass empty fields.																	
//        //if (baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE) 
//        {
//            XmlElement accountingCustomerParty = doc.CreateElement("cac:AccountingCustomerParty");
//            XmlElement accountingCustomerPartyParty = doc.CreateElement("cac:Party");
//            if (!"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyIdentification.ID.en))
//            {
//                XmlElement partyIdentification = doc.CreateElement("cac:PartyIdentification");
//                XmlElement partyIdentificationId = doc.CreateElement("cbc:ID");
//                partyIdentificationId.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyIdentification.ID.en));
//                if (!"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyIdentification.ID.schemeID))
//                {
//                    XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
//                    schemeIDAttr.Value = (baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyIdentification.ID.schemeID);
//                    partyIdentificationId.SetAttributeNode(schemeIDAttr);
//                }
//                partyIdentification.AppendChild(partyIdentificationId);
//                accountingCustomerPartyParty.AppendChild(partyIdentification);
//            }
//            XmlElement accountingCustomerPartyPostalAddress = doc.CreateElement("cac:PostalAddress");
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.StreetName.en))
//                 || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) &&
//                 !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.StreetName.en))
//                 || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyStreetName = doc.CreateElement("cbc:StreetName");
//                accountingCustomerPartyStreetName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.StreetName.en));
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyStreetName);
//            }
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.AdditionalStreetName.en)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.AdditionalStreetName.en)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyAdditionalStreetName = doc.CreateElement("cbc:AdditionalStreetName");
//                accountingCustomerPartyAdditionalStreetName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.AdditionalStreetName.en));
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyAdditionalStreetName);
//            }
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingNumber.en)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingNumber.en)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyBuildingNumber = doc.CreateElement("cbc:BuildingNumber");
//                accountingCustomerPartyBuildingNumber.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingNumber.en));
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyBuildingNumber);
//            }
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName.en)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName.en)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyCitySubdivisionName = doc.CreateElement("cbc:CitySubdivisionName");
//                accountingCustomerPartyCitySubdivisionName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName.en));
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyCitySubdivisionName);
//            }
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PlotIdentification.en)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PlotIdentification.en)) || ((baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PlotIdentification.en)))
//            {
//                XmlElement accountingCustomerPartyPlotIdentification = doc.CreateElement("cbc:PlotIdentification");
//                accountingCustomerPartyPlotIdentification.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PlotIdentification.en));
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyPlotIdentification);
//            }
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CityName.en)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CityName.en)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyCityName = doc.CreateElement("cbc:CityName");
//                accountingCustomerPartyCityName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CityName.en));
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyCityName);
//            }
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PostalZone)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PostalZone)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyPostalZone = doc.CreateElement("cbc:PostalZone");
//                accountingCustomerPartyPostalZone.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PostalZone));
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyPostalZone);
//            }
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentity.en)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentity)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyCountrySubentity = doc.CreateElement("cbc:CountrySubentity");
//                accountingCustomerPartyCountrySubentity.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentity.en));
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyCountrySubentity);
//            }

//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyCountry = doc.CreateElement("cac:Country");
//                XmlElement accountingCustomerPartyIdentificationCode = doc.CreateElement("cbc:IdentificationCode");
//                accountingCustomerPartyIdentificationCode.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode));
//                accountingCustomerPartyCountry.AppendChild(accountingCustomerPartyIdentificationCode);
//                accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyCountry);
//            }
//            accountingCustomerPartyParty.AppendChild(accountingCustomerPartyPostalAddress);
//            XmlElement accountingCustomerPartyPartyTaxScheme = doc.CreateElement("cac:PartyTaxScheme");
//            XmlElement accountingCustomerPartyTaxScheme = doc.CreateElement("cac:TaxScheme");
//            XmlElement accountingCustomerPartyTaxSchemeID = doc.CreateElement("cbc:ID");
//            accountingCustomerPartyTaxSchemeID.AppendChild(doc.CreateTextNode("VAT"));
//            accountingCustomerPartyTaxScheme.AppendChild(accountingCustomerPartyTaxSchemeID);
//            accountingCustomerPartyPartyTaxScheme.AppendChild(accountingCustomerPartyTaxScheme);
//            accountingCustomerPartyParty.AppendChild(accountingCustomerPartyPartyTaxScheme);
//            if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_INVOICE) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
//            {
//                XmlElement accountingCustomerPartyLegalEntity = doc.CreateElement("cac:PartyLegalEntity");
//                XmlElement accountingCustomerPartyRegistrationName = doc.CreateElement("cbc:RegistrationName");
//                accountingCustomerPartyRegistrationName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName.en));
//                accountingCustomerPartyLegalEntity.AppendChild(accountingCustomerPartyRegistrationName);
//                accountingCustomerPartyParty.AppendChild(accountingCustomerPartyLegalEntity);
//            }
//            accountingCustomerParty.AppendChild(accountingCustomerPartyParty);
//            rootXmlElement.AppendChild(accountingCustomerParty);
//        }

//        //             XmlElement delivery = doc.CreateElement("cac:Delivery");
//        //             if (!"".Equals(PropertyAccessor.valueOrDefault(() -> baseData.getActualDeliveryDate(), ""))) {
//        //                 XmlElement actualDeliveryDate = doc.CreateElement("cbc:ActualDeliveryDate");
//        //                 actualDeliveryDate.AppendChild(doc.CreateTextNode(baseData.getActualDeliveryDate()));
//        //                 delivery.AppendChild(actualDeliveryDate);
//        //             }
//        //             if (!"".Equals(PropertyAccessor.valueOrDefault(baseData::getLatestDeliveryDate, ""))) {
//        //                 XmlElement latestDeliveryDate = doc.CreateElement("cbc:LatestDeliveryDate");
//        //                 latestDeliveryDate.AppendChild(doc.CreateTextNode(baseData.getLatestDeliveryDate()));
//        //                 delivery.AppendChild(latestDeliveryDate);
//        //             }
//        //             rootXmlElement.AppendChild(delivery);

//        for (int i = 0; i < baseData.Invoice.EInvoice.PaymentMeans.Count; i++)
//        {
//            BaseData.PaymentMean pm = baseData.Invoice.EInvoice.PaymentMeans[i];
//            XmlElement paymentMeans = doc.CreateElement("cac:PaymentMeans");
//            // if ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_INVOICE && !"".Equals(PropertyAccessor.valueOrDefault(() -> baseData.getPaymentMeans().getPaymentMeansCode(), ""))) || baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE) {
//            XmlElement paymentMeansCode = doc.CreateElement("cbc:PaymentMeansCode");
//            paymentMeansCode.AppendChild(doc.CreateTextNode(pm.PaymentMeansCode));
//            paymentMeans.AppendChild(paymentMeansCode);

//            //}

//            if (baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN)
//            {
//                XmlElement InstructionNote = doc.CreateElement("cbc:InstructionNote");
//                InstructionNote.AppendChild(doc.CreateTextNode(pm.InstructionNote.en));
//                paymentMeans.AppendChild(InstructionNote);
//            }


//            rootXmlElement.AppendChild(paymentMeans);
//        }




//        if (baseData.Invoice.EInvoice.AllowanceCharge.Count > 0)
//        {
//            for (int i = 0; i < baseData.Invoice.EInvoice.PaymentMeans.Count; i++)
//            {
//                BaseData.AllowanceCharge ac = baseData.Invoice.EInvoice.AllowanceCharge[i];
//                XmlElement allowanceCharge = doc.CreateElement("cac:AllowanceCharge");
//                if (!"".Equals(ac.ChargeIndicator))
//                {
//                    XmlElement chargeIndicator = doc.CreateElement("cbc:ChargeIndicator");
//                    chargeIndicator.AppendChild(doc.CreateTextNode(ac.ChargeIndicator));
//                    allowanceCharge.AppendChild(chargeIndicator);
//                }
//                if (!"".Equals(ac.MultiplierFactorNumeric))
//                {
//                    XmlElement multiplierFactorNumeric = doc.CreateElement("cbc:MultiplierFactorNumeric");
//                    multiplierFactorNumeric.AppendChild(doc.CreateTextNode(ac.MultiplierFactorNumeric));
//                    allowanceCharge.AppendChild(multiplierFactorNumeric);
//                }
//                if (!"".Equals(ac.Amount))
//                {
//                    XmlElement amount = doc.CreateElement("cbc:Amount");
//                    amount.AppendChild(doc.CreateTextNode(ac.Amount.value));
//                    if (!"".Equals(ac.Amount.currencyID))
//                    {
//                        XmlAttribute amountcurrencyIDAttr = doc.CreateAttribute("currencyID");
//                        amountcurrencyIDAttr.Value = (ac.Amount.currencyID);
//                        amount.SetAttributeNode(amountcurrencyIDAttr);
//                    }
//                    allowanceCharge.AppendChild(amount);
//                }
//                if (!"".Equals(ac.BaseAmount.value))
//                {
//                    XmlElement baseAmount = doc.CreateElement("cbc:BaseAmount");
//                    baseAmount.AppendChild(doc.CreateTextNode(ac.BaseAmount.value));
//                    if (!"".Equals(ac.BaseAmount.currencyID))
//                    {
//                        XmlAttribute baseAmountcurrencyIDAttr = doc.CreateAttribute("currencyID");
//                        baseAmountcurrencyIDAttr.Value = (ac.BaseAmount.currencyID);
//                        baseAmount.SetAttributeNode(baseAmountcurrencyIDAttr);
//                    }
//                    allowanceCharge.AppendChild(baseAmount);
//                }

//                if (!"".Equals(ac.TaxCategory.Percent))
//                {
//                    XmlElement taxCategoryElement = doc.CreateElement("cac:TaxCategory");
//                    if (!"".Equals(ac.TaxCategory.ID.en))
//                    {
//                        XmlElement taxCategoryIdElement = doc.CreateElement("cbc:ID");
//                        taxCategoryIdElement.AppendChild(doc.CreateTextNode(ac.TaxCategory.ID.en));
//                        if (!"".Equals(ac.TaxCategory.ID.schemeAgencyID))
//                        {
//                            XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
//                            schemeAgencyIDAttr.Value = (ac.TaxCategory.ID.schemeAgencyID);
//                            taxCategoryIdElement.SetAttributeNode(schemeAgencyIDAttr);
//                        }
//                        if (!"".Equals(ac.TaxCategory.ID.schemeID))
//                        {
//                            XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
//                            schemeIDAttr.Value = (ac.TaxCategory.ID.schemeID);
//                            taxCategoryIdElement.SetAttributeNode(schemeIDAttr);
//                        }
//                        taxCategoryElement.AppendChild(taxCategoryIdElement);
//                    }
//                    if (!"".Equals(ac.TaxCategory.Percent))
//                    {
//                        XmlElement percentXmlElement = doc.CreateElement("cbc:Percent");
//                        percentXmlElement.AppendChild(doc.CreateTextNode(ac.TaxCategory.Percent));
//                        taxCategoryElement.AppendChild(percentXmlElement);
//                    }
//                    if (!"".Equals(ac.TaxCategory.TaxScheme.ID.en))
//                    {
//                        XmlElement taxSchemeXmlElement = doc.CreateElement("cac:TaxScheme");
//                        XmlElement taxSchemeIdXmlElement = doc.CreateElement("cbc:ID");
//                        taxSchemeIdXmlElement.AppendChild(doc.CreateTextNode(ac.TaxCategory.TaxScheme.ID.en));

//                        //if (!"".Equals(ac.TaxCategory.TaxScheme.ID.schemeAgencyID))
//                        //{
//                        //    XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
//                        //    schemeAgencyIDAttr.Value = (ac.TaxCategory.TaxScheme.ID.schemeAgencyID);
//                        //    taxSchemeIdXmlElement.SetAttributeNode(schemeAgencyIDAttr);
//                        //}
//                        //if (!"".Equals(ac.TaxCategory.TaxScheme.ID.schemeID))
//                        //{
//                        //    XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
//                        //    schemeIDAttr.Value = (ac.TaxCategory.TaxScheme.ID.schemeID);
//                        //    taxSchemeIdXmlElement.SetAttributeNode(schemeIDAttr);
//                        //}

//                        taxSchemeXmlElement.AppendChild(taxSchemeIdXmlElement);
//                        taxCategoryElement.AppendChild(taxSchemeXmlElement);
//                    }
//                    allowanceCharge.AppendChild(taxCategoryElement);
//                }
//                rootXmlElement.AppendChild(allowanceCharge);
//            }
//        }
//        if (baseData.Invoice.EInvoice.TaxTotal.Count != 2)
//            throw new InvalidCastException(ERR.custerr_Only_two_TaxTotal_are_allowed_in_request_json.ToString());

//        for (int k = 0; k < baseData.Invoice.EInvoice.TaxTotal.Count; k++)
//        {
//            BaseData.TaxTotal tt = baseData.Invoice.EInvoice.TaxTotal[k];
//            XmlElement taxTotalElement = doc.CreateElement("cac:TaxTotal");
//            XmlElement taxAmountElement = doc.CreateElement("cbc:TaxAmount");
//            taxAmountElement.AppendChild(doc.CreateTextNode(tt.TaxAmount.value));
//            XmlAttribute taxAmountElementCurrencyIDAttr = doc.CreateAttribute("currencyID");
//            taxAmountElementCurrencyIDAttr.Value = (tt.TaxAmount.currencyID);
//            taxAmountElement.SetAttributeNode(taxAmountElementCurrencyIDAttr);
//            taxTotalElement.AppendChild(taxAmountElement);


//            if (tt.TaxSubtotal != null && tt.TaxSubtotal.Count > 0)
//            {

//                for (int j = 0; j < tt.TaxSubtotal.Count; j++)
//                {
//                    BaseData.TaxSubtotal tst = tt.TaxSubtotal[j];
//                    XmlElement taxSubtotal = doc.CreateElement("cac:TaxSubtotal");
//                    XmlElement taxableAmount = doc.CreateElement("cbc:TaxableAmount");
//                    taxableAmount.AppendChild(doc.CreateTextNode(tst.TaxableAmount.value));
//                    XmlAttribute taxableAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
//                    taxableAmountCurrencyIDAttr.Value = (tst.TaxableAmount.currencyID);
//                    taxableAmount.SetAttributeNode(taxableAmountCurrencyIDAttr);
//                    taxSubtotal.AppendChild(taxableAmount);

//                    XmlElement taxSubtotalTaxAmount = doc.CreateElement("cbc:TaxAmount");
//                    taxSubtotalTaxAmount.AppendChild(doc.CreateTextNode(tst.TaxAmount.value));
//                    XmlAttribute taxSubtotalTaxAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
//                    taxSubtotalTaxAmountCurrencyIDAttr.Value = (tst.TaxAmount.currencyID);
//                    taxSubtotalTaxAmount.SetAttributeNode(taxSubtotalTaxAmountCurrencyIDAttr);
//                    taxSubtotal.AppendChild(taxSubtotalTaxAmount);

//                    XmlElement taxSubtotaltaxCategoryElement = doc.CreateElement("cac:TaxCategory");
//                    XmlElement taxSubtotaltaxCategoryIdElement = doc.CreateElement("cbc:ID");
//                    taxSubtotaltaxCategoryIdElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.ID.value));
//                    if (!"".Equals(tst.TaxCategory.ID.schemeAgencyID))
//                    {
//                        XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
//                        schemeAgencyIDAttr.Value = (tst.TaxCategory.ID.schemeAgencyID);
//                        taxSubtotaltaxCategoryIdElement.SetAttributeNode(schemeAgencyIDAttr);
//                    }
//                    if (!"".Equals(tst.TaxCategory.ID.schemeID))
//                    {
//                        XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
//                        schemeIDAttr.Value = (tst.TaxCategory.ID.schemeID);
//                        taxSubtotaltaxCategoryIdElement.SetAttributeNode(schemeIDAttr);
//                    }
//                    taxSubtotaltaxCategoryElement.AppendChild(taxSubtotaltaxCategoryIdElement);

//                    XmlElement taxSubtotalPercentXmlElement = doc.CreateElement("cbc:Percent");
//                    taxSubtotalPercentXmlElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.Percent));
//                    taxSubtotaltaxCategoryElement.AppendChild(taxSubtotalPercentXmlElement);

//                    if (!"".Equals(tst.TaxCategory?.TaxExemptionReasonCode ?? ""))
//                    {
//                        XmlElement taxExemptionReasonCodeXmlElement = doc.CreateElement("cbc:TaxExemptionReasonCode");
//                        taxExemptionReasonCodeXmlElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.TaxExemptionReasonCode));
//                        taxSubtotaltaxCategoryElement.AppendChild(taxExemptionReasonCodeXmlElement);
//                    }

//                    if (!"".Equals(tst.TaxCategory?.TaxExemptionReason?.en ?? ""))
//                    {
//                        XmlElement taxExemptionReasonXmlElement = doc.CreateElement("cbc:TaxExemptionReason");
//                        taxExemptionReasonXmlElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.TaxExemptionReason.en));
//                        taxSubtotaltaxCategoryElement.AppendChild(taxExemptionReasonXmlElement);
//                    }

//                    XmlElement taxSubtotalTaxSchemeXmlElement = doc.CreateElement("cac:TaxScheme");
//                    XmlElement taxSubtotalTaxSchemeIdXmlElement = doc.CreateElement("cbc:ID");
//                    if (!"".Equals(tst.TaxCategory.TaxScheme.ID.schemeAgencyID))
//                    {
//                        XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
//                        schemeAgencyIDAttr.Value = (tst.TaxCategory.TaxScheme.ID.schemeAgencyID);
//                        taxSubtotalTaxSchemeIdXmlElement.SetAttributeNode(schemeAgencyIDAttr);
//                    }
//                    if (!"".Equals(tst.TaxCategory.TaxScheme.ID.schemeID))
//                    {
//                        XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
//                        schemeIDAttr.Value = (tst.TaxCategory.TaxScheme.ID.schemeID);
//                        taxSubtotalTaxSchemeIdXmlElement.SetAttributeNode(schemeIDAttr);
//                    }
//                    taxSubtotalTaxSchemeIdXmlElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.TaxScheme.ID.value));
//                    taxSubtotalTaxSchemeXmlElement.AppendChild(taxSubtotalTaxSchemeIdXmlElement);
//                    taxSubtotaltaxCategoryElement.AppendChild(taxSubtotalTaxSchemeXmlElement);


//                    taxSubtotal.AppendChild(taxSubtotaltaxCategoryElement);
//                    taxTotalElement.AppendChild(taxSubtotal);
//                }
//            }
//            rootXmlElement.AppendChild(taxTotalElement);

//        }



//        XmlElement legalMonetaryTotal = doc.CreateElement("cac:LegalMonetaryTotal");
//        XmlElement lineExtensionAmount = doc.CreateElement("cbc:LineExtensionAmount");
//        lineExtensionAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.LineExtensionAmount.value));
//        XmlAttribute currencyIDAttr = doc.CreateAttribute("currencyID");
//        currencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.LineExtensionAmount.currencyID);
//        lineExtensionAmount.SetAttributeNode(currencyIDAttr);
//        legalMonetaryTotal.AppendChild(lineExtensionAmount);

//        XmlElement taxExclusiveAmount = doc.CreateElement("cbc:TaxExclusiveAmount");
//        taxExclusiveAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxExclusiveAmount.value));
//        XmlAttribute taxExclusiveAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
//        taxExclusiveAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxExclusiveAmount.currencyID);
//        taxExclusiveAmount.SetAttributeNode(taxExclusiveAmountCurrencyIDAttr);
//        legalMonetaryTotal.AppendChild(taxExclusiveAmount);
//        XmlElement taxInclusiveAmount = doc.CreateElement("cbc:TaxInclusiveAmount");
//        taxInclusiveAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxInclusiveAmount.value));
//        XmlAttribute taxInclusiveAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
//        taxInclusiveAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxInclusiveAmount.currencyID);
//        taxInclusiveAmount.SetAttributeNode(taxInclusiveAmountCurrencyIDAttr);
//        legalMonetaryTotal.AppendChild(taxInclusiveAmount);

//        if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.AllowanceTotalAmount.value))
//        {
//            XmlElement allowanceTotalAmount = doc.CreateElement("cbc:AllowanceTotalAmount");
//            allowanceTotalAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.AllowanceTotalAmount.value));
//            if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.AllowanceTotalAmount.currencyID))
//            {
//                XmlAttribute allowanceTotalAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
//                allowanceTotalAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.AllowanceTotalAmount.currencyID);
//                allowanceTotalAmount.SetAttributeNode(allowanceTotalAmountCurrencyIDAttr);
//            }
//            legalMonetaryTotal.AppendChild(allowanceTotalAmount);
//        }
//        //if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.getChargeTotalAmount(), "")))
//        //{
//        //    XmlElement chargeTotalAmount = doc.CreateElement("cbc:ChargeTotalAmount");
//        //    chargeTotalAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.getChargeTotalAmount()));
//        //    if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.getChargeTotalAmountCurrencyId(), "")))
//        //    {
//        //        Attr chargeTotalAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
//        //        chargeTotalAmountCurrencyIDAttr.Value=(baseData.Invoice.EInvoice.LegalMonetaryTotal.getChargeTotalAmountCurrencyId());
//        //        chargeTotalAmount.SetAttributeNode(chargeTotalAmountCurrencyIDAttr);
//        //    }
//        //    legalMonetaryTotal.AppendChild(chargeTotalAmount);
//        //}
//        if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.PrepaidAmount.value))
//        {
//            XmlElement prepaidAmount = doc.CreateElement("cbc:PrepaidAmount");
//            prepaidAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.PrepaidAmount.value));
//            if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.PrepaidAmount.currencyID))
//            {
//                XmlAttribute prepaidAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
//                prepaidAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.PrepaidAmount.currencyID);
//                prepaidAmount.SetAttributeNode(prepaidAmountCurrencyIDAttr);
//            }
//            legalMonetaryTotal.AppendChild(prepaidAmount);
//        }
//        XmlElement payableAmount = doc.CreateElement("cbc:PayableAmount");
//        payableAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.PayableAmount.value));
//        if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.PayableAmount.currencyID))
//        {
//            XmlAttribute payableAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
//            payableAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.PayableAmount.currencyID);
//            payableAmount.SetAttributeNode(payableAmountCurrencyIDAttr);
//        }
//        legalMonetaryTotal.AppendChild(payableAmount);
//        rootXmlElement.AppendChild(legalMonetaryTotal);


//        doc = addXmlElementForInvoiceLine(baseData, doc, rootXmlElement);


//        string fname = Basepage.invoiceXML_folder + "unsigned//" + "u" + GetFileName_withoutext(baseData);

//        doc.Normalize();
//        doc.Save(fname);

//        //var nsmgr = new XmlNamespaceManager(doc.NameTable);
//        // nsmgr.AddNamespace("app", "http://www.weather.gov/forecasts/xml/OGC_services");

//        // doc.getDocumentXmlElement().normalize();
//        //  Transformer transformer=new service.HashingGenerationService().getTransformer();
//        //DOMSource source = new DOMSource(doc);
//        //    StreamResult result = new StreamResult(new File(fname));

//        //    TransformerFactory transformerFactory = tra.newInstance();
//        //    Transformer transformer = transformerFactory.newTransformer();
//        //    //             transformer.setOutputProperty(OutputKeys.INDENT, "yes");
//        //    //            // transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "2");
//        //    //             transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
//        //    //             transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
//        //    transformer.setOutputProperty("encoding", "UTF-8");
//        //    transformer.setOutputProperty("indent", "yes");
//        //    transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "4");
//        //    transformer.setOutputProperty("omit-xml-declaration", "yes");
//        //    transformer.transform(source, result);
//        //    util.Util.log("XML file created successfully");


//        //while returning xml and signing thexml string directly, it gives error  "hashedXml does not match with qr code hashedXml"
//        //So doing the same way as SDK, saving the generated invoice xml and then load the xml from file for signing.

//        //              StringWriter stringWriter = new StringWriter();
//        //              transformer.transform(source, new StreamResult(stringWriter)); 
//        //                 xml = stringWriter.toString();
//        //                 xml=xml.trim();

//        xml = System.IO.File.ReadAllText(fname, Encoding.UTF8);

//        //                                  byte [] bb=xml.getBytes(StandardCharsets.UTF_8);
//        //                  xml= canonicalizeXml(bb);  // it is needed to make sure last new line is removed.
//        //                    util.Util.logEx("xml canonicalized");
//        return fname;

//    }
//    catch (Exception ex)
//    {

//        bp.log("generateXML error:" + ex.ToString(), 1, typ);
//        throw new InvalidCastException(ex.ToString());
//    }

//}