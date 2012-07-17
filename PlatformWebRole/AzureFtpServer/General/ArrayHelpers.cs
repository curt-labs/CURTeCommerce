using System;
using System.Diagnostics;

namespace AzureFtpServer.General
{
    public sealed class ArrayHelpers
    {
        private ArrayHelpers()
        {
        }

        public static Array Add(Array aFirst, Array aSecond)
        {
            if (aFirst == null)
            {
                return aSecond.Clone() as Array;
            }

            if (aSecond == null)
            {
                return aFirst.Clone() as Array;
            }

            Type typeFirst = aFirst.GetType().GetElementType();
            Type typeSecond = aSecond.GetType().GetElementType();

            Debug.Assert(typeFirst == typeSecond);

            Array aNewArray = Array.CreateInstance(typeFirst, aFirst.Length + aSecond.Length);
            aFirst.CopyTo(aNewArray, 0);
            aSecond.CopyTo(aNewArray, aFirst.Length);

            return aNewArray;
        }
    }
}