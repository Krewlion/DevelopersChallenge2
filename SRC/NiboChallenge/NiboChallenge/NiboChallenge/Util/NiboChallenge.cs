using NiboChallenge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace NiboChallenge.Util
{
    public class NiboChallenge
    {

        public enum PartDateTime
        {
            DAY,
            MONTH,
            YEAR,
            HOUR,
            MINUTE,
            SECOND
        }


        public void ReadOfx(byte[] b, List<ofxData> lista)
        {
            String linha;
            List<ofxData> extrato = new List<ofxData>();
            MemoryStream memoryStream = new MemoryStream(b);
            bool getSTMTRS = false;
            using (var reader = new StreamReader(memoryStream))
            {
                ofxData ofx = new ofxData();
                while ((linha = reader.ReadLine()) != null)
                {
                    linha = linha.Trim();

                    if (linha.Contains("<STMTTRN>") && getSTMTRS == false)
                    {
                        getSTMTRS = true;
                    }

                    if (getSTMTRS)
                    {
                        if (linha.Contains("</STMTTRN>"))
                        {
                            getSTMTRS = false;
                            if (lista.Where(x => x.Lugar == ofx.Lugar && x.Tipo == ofx.Tipo && x.Valor == ofx.Valor && x.Data == ofx.Data).Count() == 0)
                            {
                                lista.Add(ofx);
                            }
                            ofx = new ofxData();
                        }
                        if (linha.Contains("<DTPOSTED>"))
                        {
                            ofx.Data = ConvertOfxDateToDateTime(linha.Replace("<DTPOSTED>", "")).ToShortDateString();
                        }
                        if (linha.Contains("<MEMO>"))
                        {
                            ofx.Lugar = linha.Replace("<MEMO>", "");
                        }
                        if (linha.Contains("<TRNTYPE>"))
                        {
                            ofx.Tipo = linha.Replace("<TRNTYPE>", "");
                        }
                        if (linha.Contains("<TRNAMT>"))
                        {
                            ofx.Valor = Decimal.Parse(linha.Replace("<TRNAMT>", "").Replace(".", ","));
                        }
                    }
                }
            }

            DateTime ConvertOfxDateToDateTime(String ofxDate)
            {
                DateTime dateTimeReturned = DateTime.MinValue;

                int year = GetPartOfOfxDate(ofxDate, PartDateTime.YEAR);
                int month = GetPartOfOfxDate(ofxDate, PartDateTime.MONTH);
                int day = GetPartOfOfxDate(ofxDate, PartDateTime.DAY);
                int hour = GetPartOfOfxDate(ofxDate, PartDateTime.HOUR);
                int minute = GetPartOfOfxDate(ofxDate, PartDateTime.MINUTE);
                int second = GetPartOfOfxDate(ofxDate, PartDateTime.SECOND);

                dateTimeReturned = new DateTime(year, month, day, hour, minute, second);

                return dateTimeReturned;
            }

            int GetPartOfOfxDate(String ofxDate, PartDateTime partDateTime)
            {
                int result = 0;

                if (partDateTime == PartDateTime.YEAR)
                {
                    result = Int32.Parse(ofxDate.Substring(0, 4));

                }
                else if (partDateTime == PartDateTime.MONTH)
                {
                    result = Int32.Parse(ofxDate.Substring(4, 2));

                }
                if (partDateTime == PartDateTime.DAY)
                {
                    result = Int32.Parse(ofxDate.Substring(6, 2));

                }
                if (partDateTime == PartDateTime.HOUR)
                {
                    if (ofxDate.Length >= 10)
                    {
                        result = Int32.Parse(ofxDate.Substring(8, 2));
                    }
                    else
                    {
                        result = 0;
                    }

                }
                if (partDateTime == PartDateTime.MINUTE)
                {
                    if (ofxDate.Length >= 12)
                    {
                        result = Int32.Parse(ofxDate.Substring(10, 2));
                    }
                    else
                    {
                        result = 0;
                    }

                }
                if (partDateTime == PartDateTime.SECOND)
                {
                    if (ofxDate.Length >= 14)
                    {
                        result = Int32.Parse(ofxDate.Substring(12, 2));
                    }
                    else
                    {
                        result = 0;
                    }
                }
                return result;
            }
        }
    }
}