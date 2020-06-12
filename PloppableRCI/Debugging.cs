using System.Text;
using UnityEngine;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Debugging utility class.
    /// </summary>
    internal static class Debugging
    {
        // Buffer errors (used during loading to display an error modal dialog after loading has completed).
        internal static StringBuilder ErrorBuffer = new StringBuilder();


        // Display and log any buffered errors.
        internal static void ReportErrors()
        {
            if (ErrorBuffer.Length > 0)
            {
                Debug.Log("RICO Revisited: errors encountered:\r\n" + ErrorBuffer.ToString());
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("RICO Revisited",  ErrorBuffer.ToString(), false);

                // Clear buffer.
                ErrorBuffer.Remove(0, ErrorBuffer.Length);
            }
        }

        
        /// <summary>
        /// Prints a single-line debugging message to the Unity output log.
        /// </summary>
        /// <param name="message"></param>
        internal static void Message(string message)
        {
            Debug.Log("RICO Revisited: " + message + ".");
        }
    }
}
