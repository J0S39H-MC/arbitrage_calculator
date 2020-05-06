using System;
using System.Collections.Generic;
using System.Text;

namespace IndexArb.CommonModel
{
    public class LiborUSD
    {
        public DateTime Date { get; set; }
        public decimal OneMonthRate { get; set; }
        public decimal SixthMontRate { get; set; }
        public decimal SecondMontRate { get; set; }
        public decimal ThirdMonthRate { get; set; }
        public decimal TwelthMonthRate { get; set; }
    }
}
