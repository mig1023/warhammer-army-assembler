using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarhammerArmyAssembler.Test
{
    public class Param
    {
        public enum TestType { Pass, Wound, Death };
        public enum RepeatType { Round, Once, Hit };

        public string Type { get; set; }
        public TestType Bet { get; set; }
        public RepeatType Repeat { get; set; } 


        public static Param[] Clone(Param[] allParams)
        {
            List<Param> newParams = new List<Param>();

            foreach(Param param in allParams)
            {
                Param newParamTest = new Param
                {
                    Type = param.Type,
                    Bet = param.Bet,
                    Repeat = param.Repeat,
                };

                newParams.Add(newParamTest);
            }

            return newParams.ToArray();
        } 
    }
}
