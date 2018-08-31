using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersianCalendarWPF.Hijri
{
    /// <summary>

    /// اجزای ماه شمسی

    /// </summary>

    public class PersianMonth

    {

        /// <summary>

        /// اولین روز ماه شمسی

        /// </summary>

        public DateTime StartDate { set; get; }



        /// <summary>

        /// آخرین روز ماه شمسی

        /// </summary>

        public DateTime EndDate { set; get; }

    }
}
