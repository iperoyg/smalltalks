using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Takenet.Textc;
using Takenet.Textc.Csdl;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;

namespace SmallTalks.Core.Services
{
    public class DateTimeParser
    {
        public void Build()
        {
            var tomorrow = "amanhã";
            var today = "hoje,hj";
            var yesterday = "ontem";
            var months = new List<string>
            {
                "janeiro,jan",
                "fevereiro,fev",
                "março,mar",
                "abril,abr",
                "maio,mai",
                "junho,jun",
                "julho,jul",
                "agosto,ago",
                "setembro,set",
                "outubro,out",
                "novembro,nov",
                "dezembro,dez"
            };
            var date = "";


            
        }

    }
    

}
