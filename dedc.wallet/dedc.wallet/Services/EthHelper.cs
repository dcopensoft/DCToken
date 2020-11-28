using System;
using System.Collections.Generic;
using System.Text;

namespace DCToken.Services
{
    public class EthHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strIN"></param>
        /// <returns></returns>
        public static string ParameEncod(List<object> strIN)
        {

            string result = "";

            Nethereum.ABI.FunctionEncoding.ParametersEncoder p = new Nethereum.ABI.FunctionEncoding.ParametersEncoder();
            Nethereum.ABI.Model.Parameter[] paary = new Nethereum.ABI.Model.Parameter[strIN.Count];
            object[] datas = new object[strIN.Count];
            for (int i = 0; i < strIN.Count; i++)
            {

                if (strIN[i].ToString().ToLower().StartsWith("0x") && strIN[i].ToString().Length > 40 && strIN[i].ToString().Length < 64)
                    paary[i] = new Nethereum.ABI.Model.Parameter("address");
                else if (strIN[i].GetType() == typeof(string))
                    paary[i] = new Nethereum.ABI.Model.Parameter("string");
                else
                    paary[i] = new Nethereum.ABI.Model.Parameter("uint256");

                datas[i] = strIN[i];
            }

            byte[] arrByte = p.EncodeParameters(paary, datas);

            for (int i = 0; i < arrByte.Length; i++)
            {
                result += BitConverter.ToString(arrByte, i, 1);
            }
            return result;
        }
        public static string FuncionSign(string function)
        {
            return Nethereum.Web3.Web3.Sha3(function).Substring(0, 8);
        }

        public static string CallContractFunData(string funname, List<object> strIN)
        {
           return "0x" + FuncionSign(funname) +ParameEncod(strIN);
        }
    }
}
