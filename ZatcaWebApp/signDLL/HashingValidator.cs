 
using SDKNETFrameWorkLib.GeneralLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace ZatcaWebApp.signDLL
{
    public class HashingValidator
    {
        public Result GenerateEInvoiceHashing(string xmlFilePath)
        {
            Result einvoiceHashing = new Result();
            einvoiceHashing.Operation = "Generate Invoice Hashing";
            einvoiceHashing.IsValid = false;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.Load(xmlFilePath);
                }
                catch (Exception ex)
                {
                    einvoiceHashing.ErrorMessage = "Can not load XML file";
                    new Basepage().log("Can not load XML file::" + ex.ToString(), 1, LOGTYPE.CMPL);
                    return einvoiceHashing;
                }
                if (string.IsNullOrEmpty(xmlDocument.OuterXml))
                {
                    einvoiceHashing.ErrorMessage = "Invalid invoice XML content";
                    new Basepage().log("Invalid invoice XML content::" + xmlDocument.OuterXml, 1, LOGTYPE.CMPL);
                    return einvoiceHashing;
                }
                string s;
                try
                {
                    s = Utility.ApplyXSLT(xmlFilePath, SettingsParams.Embeded_InvoiceXSLFileForHashing);

                    //WORKING UPDATE-After 2 days troubleshoot************
                    s = s.Replace("CommonExtensionComponents-2\">\r\n  <cbc:ProfileID>", "CommonExtensionComponents-2\">\r\n  \r\n  <cbc:ProfileID>");
                    //s = File.ReadAllText(xmlFilePath, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    einvoiceHashing.ErrorMessage = "Can not apply XSL file";
                    new Basepage().log("Can not apply XSL file::" + ex.ToString(), 1, LOGTYPE.CMPL);
                    return einvoiceHashing;
                }
                if (string.IsNullOrEmpty(s))
                {
                    einvoiceHashing.ErrorMessage = "Error In applying XSL file";
                    new Basepage().log("Error In applying XSL file::" + s, 1, LOGTYPE.CMPL);
                    return einvoiceHashing;
                }
                // s = s.Replace( "  <cac:AccountingSupplierParty>","<cac:AccountingSupplierParty>");
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
                {
                    XmlDsigC14NTransform dsigC14Ntransform = new XmlDsigC14NTransform(false);
                    dsigC14Ntransform.LoadInput((object)memoryStream);
                    sbyte[] array = ((IEnumerable<byte>)Utility.Sha256_hashAsBytes(Encoding.UTF8.GetString((dsigC14Ntransform.GetOutput() as MemoryStream).ToArray()))).Select<byte, sbyte>((Func<byte, sbyte>)(x => (sbyte)x)).ToArray<sbyte>();
                    einvoiceHashing.ResultedValue = Utility.ToBase64Encode((byte[])(Array)array);
                    einvoiceHashing.IsValid = true;


                    //generates same invoice hash as above.
                    // var _IHashingValidator = new SDKNETFrameWorkLib.BLL.HashingValidator();
                    //SDKNETFrameWorkLib.GeneralLogic.Result res = _IHashingValidator.GenerateEInvoiceHashing(xmlFilePath);
                    //einvoiceHashing.ResultedValue = res.ResultedValue;
                }
                return einvoiceHashing;
            }
            catch (Exception ex)
            {
                einvoiceHashing.ErrorMessage = ex.ToString();
                new Basepage().log("GenerateEInvoiceHashing error::" + ex.ToString(), 1, LOGTYPE.CMPL);
                return einvoiceHashing;
            }
        }

        public Result ValidateEInvoiceHashing(string xmlFilePath)
        {
            Result result = new Result();
            result.Operation = "Validating Invoice Hashing";
            result.IsValid = false;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xmlFilePath);
            }
            catch
            {
                result.ErrorMessage = "Can not load XML file";
                return result;
            }
            string nodeInnerText = Utility.GetNodeInnerText(doc, SettingsParams.Hash_XPATH);
            if (string.IsNullOrEmpty(nodeInnerText))
            {
                result.ErrorMessage = "There is no Hashing node value in this XML file";
                return result;
            }
            Result einvoiceHashing = this.GenerateEInvoiceHashing(xmlFilePath);
            if (!einvoiceHashing.IsValid)
            {
                result.ErrorMessage = einvoiceHashing.ErrorMessage;
                return result;
            }
            if (nodeInnerText != einvoiceHashing.ResultedValue)
            {
                result.ErrorMessage = "The generated Hashing is different of the one exists in the XML file.";
                return result;
            }
            result.IsValid = true;
            return result;
        }
    }
}
