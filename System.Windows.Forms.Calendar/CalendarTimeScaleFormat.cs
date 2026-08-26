using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms.Calendar
{
    /// <summary>
    /// Enumerates possible timescales for <see cref="Calendar"/> control
    /// </summary>
    public enum CalendarTimeScaleFormat
    {
        /// <summary>
        /// Makes calendar Time show in Military Time 1 - 23 hours
        /// </summary>
        MilitaryTime = 0,

        /// <summary>
        /// Makes calendar time show in Standard Time 1-12, 1-12
        /// </summary>
        StandardTime = 1,
    }
}
