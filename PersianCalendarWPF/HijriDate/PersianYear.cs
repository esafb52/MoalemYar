using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersianCalendarWPF
{
    /// <summary>

    /// اجزای سال شمسی

    /// </summary>

    public class PersianYear

    {

        /// <summary>

        /// اولین روز سال شمسی

        /// </summary>

        public DateTime StartDate { set; get; }



        /// <summary>

        /// آخرین روز سال شمسی

        /// </summary>

        public DateTime EndDate { set; get; }

    }
}
