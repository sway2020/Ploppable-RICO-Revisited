using System;
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
                Debug.Log(PloppableRICOMod.ModName+ ": errors encountered:\r\n" + ErrorBuffer.ToString());
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(PloppableRICOMod.ModName,  ErrorBuffer.ToString(), false);

                // Clear buffer.
                ErrorBuffer.Remove(0, ErrorBuffer.Length);
            }
        }


        /// <summary>
        /// Prints a single-line debugging message to the Unity output log if (and only if) the 'additional debugging logging' option is selected.
        /// </summary>
        /// <param name="message">Message to log</param>
        internal static void OptionalMessage(string message)
        {
            if (ModSettings.debugLogging)
            {
                Message(message);
            }
        }


        /// <summary>
        /// Prints a single-line debugging message to the Unity output log.
        /// </summary>
        /// <param name="message">Message to log</param>
        internal static void Message(string message)
        {
            Debug.Log(PloppableRICOMod.ModName + ": " + message + ".");
        }


        /// <summary>
        /// Prints an exception message to the Unity output log.
        /// </summary>
        /// <param name="message">Message to log</param>
        internal static void LogException(Exception exception)
        {
            // Use StringBuilder for efficiency since we're doing a lot of manipulation here.
            StringBuilder message = new StringBuilder();

            message.AppendLine("caught exception!");
            message.AppendLine("Exception:");
            message.AppendLine(exception.Message);
            message.AppendLine(exception.Source);
            message.AppendLine(exception.StackTrace);

            // Log inner exception as well, if there is one.
            if (exception.InnerException != null)
            {
                message.AppendLine("Inner exception:");
                message.AppendLine(exception.InnerException.Message);
                message.AppendLine(exception.InnerException.Source);
                message.AppendLine(exception.InnerException.StackTrace);
            }

            // Write to log.
            Message(message.ToString());
        }
    }
}
