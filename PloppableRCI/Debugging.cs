using System.Text;
using UnityEngine;
using ColossalFramework.UI;

namespace PloppableRICO
{
    class Debugging
    {
        // Buffer errors (used during loading to display an error modal dialog after loading has completed).
        public static StringBuilder ErrorBuffer = new StringBuilder();


        // Display and log any buffered errors.
        public static void ReportErrors()
        {
            if (ErrorBuffer.Length > 0)
            {
                Debug.Log("RICO Revisited: errors encountered:\r\n" + ErrorBuffer.ToString());
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("RICO Revisited",  ErrorBuffer.ToString(), false);

                // Clear buffer.
                ErrorBuffer.Remove(0, ErrorBuffer.Length);
            }
        }
    }
}
