using System;
using System.Collections.Generic;
using System.Text;

/*
    * Autor.....: Brenno Sant'Anna Scarpelini
    * Data......: 11/06/2007
    * Descrição.: Classe para escrever números por extenso
    * Copyrigth®: Todos os direitos reservados
*/

namespace Entities.Domain.Extensions
{
    //TODO: Remove and write a new solution
    public static class NumberToTextParserBR
    {

        private static string[] strUnidades = { "", "um", "dois", "tres", "quatro", "cinco", "seis", "sete", "oito", "nove", "dez", "onze", "doze", "treze", "quatorze", "quinze", "dezesseis", "dezessete", "dezoito", "dezenove" };

        private static string[] strDezenas = { "", "dez", "vinte", "trinta", "quarenta", "cinqüenta", "sessenta", "setenta", "oitenta", "noventa" };

        private static string[] strCentenas = { "", "cem", "duzentos", "trezentos", "quatrocentos", "quinhentos", "seiscentos", "setecentos", "oitocentos", "novecentos" };

        private static string strErrorString = "Valor fora da faixa";

        private static decimal decMin = 0.01M;

        private static decimal decMax = 999999999999999.99M;

        private static string ConversaoRecursiva(Int64 intNumero)

        {

            Int64 intResto = 0;

            if ((intNumero >= 1) && (intNumero <= 19))

                return strUnidades[intNumero];

            else if ((intNumero == 20) || (intNumero == 30) || (intNumero == 40) ||

            (intNumero == 50) || (intNumero == 60) || (intNumero == 70) ||

            (intNumero == 80) || (intNumero == 90))

                return strDezenas[Math.DivRem(intNumero, 10, out intResto)] + " ";

            else if ((intNumero >= 21) && (intNumero <= 29) ||

            (intNumero >= 31) && (intNumero <= 39) ||

            (intNumero >= 41) && (intNumero <= 49) ||

            (intNumero >= 51) && (intNumero <= 59) ||

            (intNumero >= 61) && (intNumero <= 69) ||

            (intNumero >= 71) && (intNumero <= 79) ||

            (intNumero >= 81) && (intNumero <= 89) ||

            (intNumero >= 91) && (intNumero <= 99))

                return strDezenas[Math.DivRem(intNumero, 10, out intResto)] + " e " + ConversaoRecursiva(intNumero % 10);

            else if ((intNumero == 100) || (intNumero == 200) || (intNumero == 300) ||

            (intNumero == 400) || (intNumero == 500) || (intNumero == 600) ||

            (intNumero == 700) || (intNumero == 800) || (intNumero == 900))

                return strCentenas[Math.DivRem(intNumero, 100, out intResto)] + " ";

            else if ((intNumero >= 101) && (intNumero <= 199))

                return " cento e " + ConversaoRecursiva(intNumero % 100);

            else if ((intNumero >= 201) && (intNumero <= 299) ||

            (intNumero >= 301) && (intNumero <= 399) ||

            (intNumero >= 401) && (intNumero <= 499) ||

            (intNumero >= 501) && (intNumero <= 599) ||

            (intNumero >= 601) && (intNumero <= 699) ||

            (intNumero >= 701) && (intNumero <= 799) ||

            (intNumero >= 801) && (intNumero <= 899) ||

            (intNumero >= 901) && (intNumero <= 999))

                return strCentenas[Math.DivRem(intNumero, 100, out intResto)] + " e " + ConversaoRecursiva(intNumero % 100);

            else if ((intNumero >= 1000) && (intNumero <= 999999))
            {
                var result = ConversaoRecursiva(Math.DivRem(intNumero, 1000, out intResto)) + " mil " + ConversaoRecursiva(intNumero % 1000);
                result = result.Replace("um mil", "mil");
                return result;
            }
            else if ((intNumero >= 1000000) && (intNumero <= 1999999))
            {
                var result = ConversaoRecursiva(Math.DivRem(intNumero, 1000000, out intResto)) + " milhao " + ConversaoRecursiva(intNumero % 1000000);
                return result;
            }
            else if ((intNumero >= 2000000) && (intNumero <= 999999999))

                return ConversaoRecursiva(Math.DivRem(intNumero, 1000000, out intResto)) + " milhoes " + ConversaoRecursiva(intNumero % 1000000);

            else if ((intNumero >= 1000000000) && (intNumero <= 1999999999))

                return ConversaoRecursiva(Math.DivRem(intNumero, 1000000000, out intResto)) + " bilhao " + ConversaoRecursiva(intNumero % 1000000000);

            else if ((intNumero >= 2000000000) && (intNumero <= 999999999999))

                return ConversaoRecursiva(Math.DivRem(intNumero, 1000000000, out intResto)) + " bilhoes " + ConversaoRecursiva(intNumero % 1000000000);

            else if ((intNumero >= 1000000000000) && (intNumero <= 1999999999999))

                return ConversaoRecursiva(Math.DivRem(intNumero, 1000000000000, out intResto)) + " trilhao " + ConversaoRecursiva(intNumero % 1000000000000);

            else if ((intNumero >= 2000000000000) && (intNumero <= 999999999999999))

                return ConversaoRecursiva(Math.DivRem(intNumero, 1000000000000, out intResto)) + " trilhoes " + ConversaoRecursiva(intNumero % 1000000000000);

            else

                return "";

        }

        private static string LimpaEspacos(string strTexto)

        {

            string strRetorno = "";

            bool booUltIs32 = false;

            foreach (char chrChar in strTexto)

            {

                if ((int)chrChar != 32)

                {

                    strRetorno += chrChar;

                    booUltIs32 = false;

                }

                else if (!booUltIs32)

                {

                    strRetorno += chrChar;

                    booUltIs32 = true;

                }

            }

            return strRetorno.Trim();

        }

        public static string NumeroParaExtenso(decimal decNumero)
        {
            string strRetorno = "";
            if ((decNumero >= decMin) && (decNumero <= decMax))
            {
                Int64 intInteiro = Convert.ToInt64(Math.Truncate(decNumero));
                strRetorno += ConversaoRecursiva(intInteiro);
            }
            else
                throw new Exception(strErrorString);

            return LimpaEspacos(strRetorno);

        }

    }

}
