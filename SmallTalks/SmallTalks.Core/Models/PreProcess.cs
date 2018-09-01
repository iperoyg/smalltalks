using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core.Models
{
    public class PreProcess
    {
        public static string Non_Duplicable_Consonants = "wtypdfghjlçv";


        public string Input { get; set; }
        public string Output { get; set; }

        public string Data { get => string.IsNullOrEmpty(Output) ? Input : Output; }
    }
}
