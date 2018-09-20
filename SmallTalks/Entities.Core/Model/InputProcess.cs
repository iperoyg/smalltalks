using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Core.Models
{
    public class InputProcess
    {
        public static string Non_Duplicable_Consonants = "wtypdfghjlçv";

        public static char Placeholder { get => '_'; }

        public string Input { get; set; }
        public string Output { get; set; }

        public string Data { get => string.IsNullOrEmpty(Output) ? Input : Output; }

        public static InputProcess FromString(string input)
        {
            return new InputProcess { Input = input };
        }

    }
}
