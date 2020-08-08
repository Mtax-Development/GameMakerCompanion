using System.Diagnostics;

namespace GMS2_RPC.Element
{
    internal static class ApplicationInstance
    {   
        /// <returns> The number of instances of applications running on the device with specified executable name. </returns>
        /// <param name="application"> Name of the application (without extension). </param>
        internal static int GetNumber(string application)
        {
            return Process.GetProcessesByName(application).Length;
        }

        /// <returns> The number of instances of applications running on the device with specified executable names. </returns>
        /// <param name="applicationList"> Array containing names of the application (without extension). </param>
        internal static int GetNumber(string[] applicationList)
        {
            int result = 0;

            for (int i = 0; i < applicationList.Length; i++)
            {
                result += Process.GetProcessesByName(applicationList[i]).Length;
            }

            return result;
        }
    }
}
