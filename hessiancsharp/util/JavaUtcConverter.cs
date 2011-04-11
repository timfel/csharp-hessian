/*
***************************************************************************************************** 
* HessianCharp - The .Net implementation of the Hessian Binary Web Service Protocol (www.caucho.com) 
* Copyright (C) 2004-2005  by D. Minich, V. Byelyenkiy, A. Voltmann, M. Wuttke
* http://www.hessiancsharp.com
*
* This library is free software; you can redistribute it and/or
* modify it under the terms of the GNU Lesser General Public
* License as published by the Free Software Foundation; either
* version 2.1 of the License, or (at your option) any later version.
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public
* License along with this library; if not, write to the Free Software
* Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
* 
* You can find the GNU Lesser General Public here
* http://www.gnu.org/licenses/lgpl.html
* or in the license.txt file in your source directory.
******************************************************************************************************  
* You can find all contact information on http://www.hessiancsharp.com	
******************************************************************************************************
*
*
******************************************************************************************************
* 2011-03-31 first version; wuttke
******************************************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace hessiancsharp.util
{

    /// <summary>
    /// Konvertiert UTC und Local Time, so wie Java das macht.
    /// Leider kann Windows das nicht so gut.
    /// </summary>
    public class JavaUtcConverter
    {

        /// <summary>
        /// Konvertiert die Java-UTC-Ticks in lokale Java-Ticks.
        /// </summary>
        public static long ConvertJavaUtcTicksToLocalTicks(long ticks)
        {
            // hier könnten weitere Zeitzonen erweitert werden
            return CENTRAL_EUROPEAN_STANDARD_TIME.getOffset(ticks) + ticks;
        }

        /// <summary>
        /// Konvertiert lokale Java-Ticks in ein C#-DateTime-Objekt mit lokaler Zeit.
        /// </summary>
        public static DateTime ConvertJavaLocalTicksToLocalDateTime(long localTicks)
        {
            const long timeShift = 62135596800000;
            long ticks = (localTicks + timeShift) * 10000;
            return new DateTime(ticks, DateTimeKind.Local);
        }

        /// <summary>
        /// Konvertiert ein C#-DateTime-Objekt mit lokaler Zeit in lokale Java-Ticks.
        /// </summary>
        public static long ConvertLocalDateTimeToLocalJavaTicks(DateTime dt)
        {
            const long timeShift = 62135596800000;
            long javaTime = dt.Ticks / 10000 - timeShift;
            return javaTime;
        }

        /// <summary>
        /// Konvertiert lokale Java-Ticks in Java-UTC-Ticks.
        /// </summary>
        public static long ConvertLocalJavaTicksToJavaUtcTicks(long localTicks)
        {
            // mit UTC-Wert Zone abfragen.
            int prevOffset = CENTRAL_EUROPEAN_STANDARD_TIME.getOffset(localTicks);
            int myOffset = CENTRAL_EUROPEAN_STANDARD_TIME.getOffset(localTicks - prevOffset);
            return localTicks - myOffset;
        }

        /// <summary>
        /// Konvertiert ein lokales C#-DateTime-Objekt in Java-UTC-Ticks.
        /// </summary>
        public static long ConvertLocalDateTimeToJavaUtcTicks(DateTime dt)
        {
            return ConvertLocalJavaTicksToJavaUtcTicks(ConvertLocalDateTimeToLocalJavaTicks(dt));
        }

        /// <summary>
        /// Konvertiert Java-UTC-Ticks in ein lokales C#-DateTime-Objekt.
        /// </summary>
        public static DateTime ConvertJavaUtcTicksToLocalDateTime(long utcTicks)
        {
            return ConvertJavaLocalTicksToLocalDateTime(ConvertJavaUtcTicksToLocalTicks(utcTicks));
        }

        // für Europe/Berlin aus sun.util.calendar.ZoneInfo und C:\Program Files\Java\jdk1.6.0_18\jre\lib\zi\Europe\Berlin extrahiert
        private static long[] CENTRAL_EUROPEAN_TRANSITIONS = new long[] { -9048018124800000, -6937421414399967, -6883260825600000, -6813514137599967, -6759014400000000, -6684696575999967, -6630196838400000, -3845755699199967, -3511325491200000, -3459303014399967, -3392416972800000, -3328008191999967, -3263599411200000, -3199190630399967, -3180802867199981, -3137273855999967, -3117794918400000, -3065772441599967, -3003487027200000, -2939417395199967, -2927045836799981, -2909719756799967, -2875023360000000, -2805660057599967, -2746205798400000, -2679319756799967, -2617388236800000, 1326410956800033, 1388342476800000, 1452751257600033, 1517160038400000, 1581568819200033, 1645977600000000, 1710386380800033, 1774795161600000, 1839203942400033, 1906089984000000, 1970498764800033, 2034907545600000, 2099316326400033, 2163725107200000, 2228133888000033, 2292542668800000, 2356951449600033, 2421360230400000, 2485769011200033, 2550177792000000, 2614586572800033, 2681472614400000, 2745881395200033, 2810290176000000, 2874698956800033, 2939107737600000, 3003516518400033, 3067925299200000, 3132334080000033, 3196742860800000, 3261151641600033, 3325560422400000, 3392446464000033, 3466764288000000, 3521264025600033, 3595581849600000, 3650081587200033, 3724399411200000, 3778899148800033, 3855694233600000, 3907716710400033, 3984511795200000, 4036534272000033, 4113329356800000, 4167829094400033, 4242146918400000, 4296646656000033, 4370964480000000, 4425464217600033, 4502259302400000, 4554281779200033, 4631076864000000, 4683099340800033, 4759894425600000, 4811916902400033, 4888711987200000, 4943211724800033, 5017529548800000, 5072029286400033, 5146347110400000, 5200846848000033, 5277641932800000, 5329664409600033, 5406459494400000, 5458481971200033, 5535277056000000, 5589776793600033, 5664094617600000, 5718594355200033, 5792912179200000, 5847411916800033, 5921729740800000, 5976229478400033, 6053024563200000, 6105047040000033, 6181842124800000, 6233864601600033, 6310659686400000, 6365159424000033, 6439477248000000, 6493976985600033, 6568294809600000, 6622794547200033, 6699589632000000, 6751612108800033, 6828407193600000, 6880429670400033, 6957224755200000, 7011724492800033, 7086042316800000, 7140542054400033, 7214859878400000, 7269359616000033, 7343677440000000, 7398177177600033, 7474972262400000, 7526994739200033, 7603789824000000, 7655812300800033, 7732607385600000, 7787107123200033, 7861424947200000, 7915924684800033, 7990242508800000, 8044742246400033, 8121537331200000, 8173559808000033, 8250354892800000, 8302377369600033, 8379172454400000, 8431194931200033, 8507990016000000, 8562489753600033, 8636807577600000, 8691307315200033, 8765625139200000 };
        private static int[] CENTRAL_EUROPEAN_OFFSETS = new int[] { 3600000, 7200000, 3600000, 10800000 };
        private const int CENTRAL_EUROPEAN_RAW_OFFSET = 3600000;
        private static JavaZoneInfo CENTRAL_EUROPEAN_STANDARD_TIME = new JavaZoneInfo(CENTRAL_EUROPEAN_RAW_OFFSET, CENTRAL_EUROPEAN_TRANSITIONS, CENTRAL_EUROPEAN_OFFSETS);

    }

    /// <summary>
    /// UTC-Umrechnung ähnlich wie Java.
    /// In Anlehnung an die Java ZoneInfo-Klasse.
    /// </summary>
    class JavaZoneInfo
    {

        private const int UTC_TIME = 0;
        private const int STANDARD_TIME = 1;
        private const int WALL_TIME = 2;
      
        private const long OFFSET_MASK = 0x0fL;
        private const long DST_MASK = 0xf0L;
        private const int DST_NSHIFT = 4;
        private const int TRANSITION_NSHIFT = 12;

        public JavaZoneInfo(int rawOffset, long[] transitions, int[] offsets)
        {
            this.rawOffset = rawOffset;
            this.transitions = transitions;
            this.offsets = offsets;
        }

        /**
         * The raw GMT offset in milliseconds between this zone and GMT.
         * Negative offsets are to the west of Greenwich.  To obtain local
         * <em>standard</em> time, add the offset to GMT time.
         */
        private int rawOffset;
     
        /**
         * This array describes transitions of GMT offsets of this time
         * zone, including both raw offset changes and daylight saving
         * time changes.
         * A long integer consists of four bit fields.
         * <ul>
         * <li>The most significant 52-bit field represents transition
         * time in milliseconds from Gregorian January 1 1970, 00:00:00
         * GMT.</li>
         * <li>The next 4-bit field is reserved and must be 0.</li>
         * <li>The next 4-bit field is an index value to {@link #offsets
         * offsets[]} for the amount of daylight saving at the
         * transition. If this value is zero, it means that no daylight
         * saving, not the index value zero.</li>
         * <li>The least significant 4-bit field is an index value to
         * {@link #offsets offsets[]} for <em>total</em> GMT offset at the
         * transition.</li>
         * </ul>
         * If this time zone doesn't observe daylight saving time and has
         * never changed any GMT offsets in the past, this value is null.
         */
        private long[] transitions;
     
        /**
         * This array holds all unique offset values in
         * milliseconds. Index values to this array are stored in the
         * transitions array elements.
         */
        private int[] offsets;
    
         /**
          * Returns the difference in milliseconds between local time and UTC
          * of given time, taking into account both the raw offset and the
          * effect of daylight savings.
          * @param date the milliseconds in UTC
          * @return the milliseconds to add to UTC to get local wall time
          */
         public int getOffset(long date) {
             return getOffsets(date, UTC_TIME);
         }
     
         private int getOffsets(long date, int type) {
            // if dst is never observed, there is no transition.
            if (transitions == null)
                return rawOffset;
     
            int index = getTransitionIndex(date, type);
     
            // outside of the transition table, returns the raw offset.
            // should support LMT.
            if (index < 0 || index >= transitions.Length)
                return rawOffset;
     
            long val = transitions[index];
            int offset = this.offsets[(int)(val & OFFSET_MASK)];
            int dst = (int)((val >> DST_NSHIFT) & 0xf0L);
            return offset;
        }
     
        private int getTransitionIndex(long date, int type) {
            int low = 0;
            int high = transitions.Length - 1;
     
            while (low <= high) {
                int mid = (low + high) / 2;
                long val = transitions[mid];
                long midVal = val >> TRANSITION_NSHIFT; // sign extended
                if (type != UTC_TIME) {
                    midVal += offsets[(int)(val & OFFSET_MASK)]; // wall time
                }
                if (type == STANDARD_TIME) {
                    int dstIndex = (int)((val >> DST_NSHIFT) & 0xfL);
                    if (dstIndex != 0)
                        midVal -= offsets[dstIndex]; // make it standard time
                }
                if (midVal < date)
                    low = mid + 1;
                else if (midVal > date)
                    high = mid - 1;
                else
                    return mid;
            }
     
            // if beyond the transitions, returns that index.
            if (low >= transitions.Length)
                return low;
            return low - 1;
        }
     
    }
}
