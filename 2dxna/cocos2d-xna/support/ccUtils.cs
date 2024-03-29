/****************************************************************************
Copyright (c) 2010 cocos2d-x.org
Copyright (c) 2011-2012 openxlive.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace cocos2d
{
    public class ccUtils
    {
        /// <summary>
        /// Returns the Cardinal Spline position for a given set of control points, tension and time
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="tension"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CCPoint ccCardinalSplineAt(CCPoint p0, CCPoint p1, CCPoint p2, CCPoint p3, float tension, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            /*
             * Formula: s(-ttt + 2tt - t)P1 + s(-ttt + tt)P2 + (2ttt - 3tt + 1)P2 + s(ttt - 2tt + t)P3 + (-2ttt + 3tt)P3 + s(ttt - tt)P4
             */
            float s = (1 - tension) / 2;

            float b1 = s * ((-t3 + (2 * t2)) - t);                      // s(-t3 + 2 t2 - t)P1
            float b2 = s * (-t3 + t2) + (2 * t3 - 3 * t2 + 1);          // s(-t3 + t2)P2 + (2 t3 - 3 t2 + 1)P2
            float b3 = s * (t3 - 2 * t2 + t) + (-2 * t3 + 3 * t2);      // s(t3 - 2 t2 + t)P3 + (-2 t3 + 3 t2)P3
            float b4 = s * (t3 - t2);                                   // s(t3 - t2)P4

            float x = (p0.x * b1 + p1.x * b2 + p2.x * b3 + p3.x * b4);
            float y = (p0.y * b1 + p1.y * b2 + p2.y * b3 + p3.y * b4);

            return new CCPoint(x, y);
        }

        /// <summary>
        /// Parses an int value using the default number style and the invariant culture parser.
        /// </summary>
        /// <param name="toParse">The value to parse</param>
        /// <returns>The int value of the string</returns>
        public static int ccParseInt(string toParse)
        {
            // http://www.cocos2d-x.org/boards/17/topics/11690
            // Issue #17
            // https://github.com/cocos2d/cocos2d-x-for-xna/issues/17
            return (int.Parse(toParse, System.Globalization.CultureInfo.InvariantCulture));
        }
        /// <summary>
        /// Parses aint value for the given string using the given number style and using
        /// the invariant culture parser.
        /// </summary>
        /// <param name="toParse">The value to parse.</param>
        /// <param name="ns">The number style used to parse the int value.</param>
        /// <returns>The int value of the string.</returns>
        public static int ccParseInt(string toParse, NumberStyles ns)
        {
            // http://www.cocos2d-x.org/boards/17/topics/11690
            // Issue #17
            // https://github.com/cocos2d/cocos2d-x-for-xna/issues/17
            return (int.Parse(toParse, ns, System.Globalization.CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Parses a float value using the default number style and the invariant culture parser.
        /// </summary>
        /// <param name="toParse">The value to parse</param>
        /// <returns>The float value of the string.</returns>
        public static float ccParseFloat(string toParse)
        {
            // http://www.cocos2d-x.org/boards/17/topics/11690
            // Issue #17
            // https://github.com/cocos2d/cocos2d-x-for-xna/issues/17
            return (float.Parse(toParse, System.Globalization.CultureInfo.InvariantCulture));
        }
        /// <summary>
        /// Parses a float value for the given string using the given number style and using
        /// the invariant culture parser.
        /// </summary>
        /// <param name="toParse">The value to parse.</param>
        /// <param name="ns">The number style used to parse the float value.</param>
        /// <returns>The float value of the string.</returns>
        public static float ccParseFloat(string toParse, NumberStyles ns)
        {
            // http://www.cocos2d-x.org/boards/17/topics/11690
            // https://github.com/cocos2d/cocos2d-x-for-xna/issues/17
            return (float.Parse(toParse, ns, System.Globalization.CultureInfo.InvariantCulture));
        }
        
        /// <summary>
        /// Returns the next Power of Two for the given value. If x = 3, then this returns 4.
        /// If x = 4 then 4 is returned. If the value is a power of two, then the same value
        /// is returned.
        /// </summary>
        /// <param name="x">The base of the POT test</param>
        /// <returns>The next power of 2 (1, 2, 4, 8, 16, 32, 64, 128, etc)</returns>
        public static long ccNextPOT(long x)
        {
            x = x - 1;
            x = x | (x >> 1);
            x = x | (x >> 2);
            x = x | (x >> 4);
            x = x | (x >> 8);
            x = x | (x >> 16);
            return x + 1;
        }
    }
}
