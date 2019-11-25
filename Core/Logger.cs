/// File: Logger.cs
/// Purpose: Defines the logger used in this application to log various messages. 
/// Version: 1.2
/// Date Modified: 11 - 24 - 2019
/// 
/// Changes
/// =======
/// Added automatic method name lookup in Log() to avoid writing function names in Log() calls. 
/// Added auto-delete of logs on Write() if they exceed the maximum log size. 
/// Removed WriteHeader() function as it could be embdedded in Init()
/// 

/* 
Copyright (c) 2019, All rights are reserved by Team WolverCode 
https://www.wolvercode.com

This program is licensed under the Apache License, Version 2.0 (the "License");
you may not download, install or use this program except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*/

using System;
using System.IO;

namespace XKeyboard.Core
{
    /// <summary>
    /// This class contains functions used to log, notify or show corresponding messages to the user.
    /// 
    /// </summary>
    class Logger
    {
        public const int MAX_LOG_SIZE = 1024 * 1024 * 10;   //10 MB only .
        //Log file
        static StreamWriter logFile;
        /// <summary>
        /// Initializes the logger. 
        /// </summary>
        public static void Init()
        {
            //Executed when the class is defined on the application level.
            //Check if log file exceeds max file size (1MB)
            if (logFile != null)
            {
                logFile.Close();
                logFile = null;
            }
            if (File.Exists("logs.log") && new FileInfo("logs.log").Length >= MAX_LOG_SIZE)
                File.Delete("logs.log");
            logFile = new StreamWriter("logs.log", true);
            logFile.AutoFlush = true;

            logFile.Write("XKeyboard".Fill(80, '-') + "\r\n");
        }
        /// <summary>
        /// Saves the current log file and closes it. 
        /// </summary>
        public static void Save()
        {
            if (logFile == null) return;
            logFile.Close();
            logFile.Dispose();
            logFile = null;
        }
        /// <summary>
        /// Logs the message in a file.
        /// </summary>
        /// <param name="output">The message to log.</param>
        public static void Log(string output, MessagePriority priority = MessagePriority.Low, MessageKind kind = MessageKind.Info)
        {
            if (logFile == null)
                Init();
            //Check if file has reached max log size
            if (logFile.BaseStream.Length >= MAX_LOG_SIZE)
                DeleteLog();
            string head = (priority == MessagePriority.Low) ? "[LOW]" : (priority == MessagePriority.Mid ? "[MIDIUM]" : "[HIGH]");
            head += kind == MessageKind.Info ? "[INFO]" : (kind == MessageKind.Warning ? "[WARNING]" : "[ERROR]");
            //Add the caller's information to the output to show function name who called the logger. 
            System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(1, true);
            head += $"[F: {sf.GetMethod().DeclaringType.FullName}.{sf.GetMethod().Name}()]";
            logFile.Write($"[{DateTime.Now}]{head}{output}\r\n");
        }  
        /// <summary>
        /// Notifies the user with a message using NotifyIcon and windows desktop alerts. 
        /// </summary>
        /// <param name="msg">The message to show in notification. </param>s
        public static void Notify(string msg, MessageKind kind)
        {
            /////TO-DO/////
            /// Call notify icon
            ///////////////
            NotificationManager.Show(msg, kind);
        }
        /// <summary>
        /// Shows a metro-styled message box to the user and returns the dialog result.
        /// </summary>
        /// <param name="msg">The message to show. </param>
        /// <param name="img">The message box icon.</param>
        /// <param name="btn">Message box buttons. </param>
        public static void ShowMessage(string msg, MessageKind kind)
        {
            UI.Metro.MessageBoxEx.Show(null, msg, kind);
        }
        /// <summary>
        /// Shows a message to the user, notifies via notify icon or logs the message. 
        /// </summary>
        /// <param name="msg">THe message. </param>
        /// <param name="priority">Priority of the message. </param>
        public static void Write(string msg, MessagePriority priority = MessagePriority.Low, MessageKind kind = MessageKind.Info)
        {
            Log(msg, priority, kind);
            switch (priority)
            {
                case MessagePriority.High:
                    //Show message to the user and log it as well 
                    ShowMessage(msg, kind);
                    break;
                case MessagePriority.Mid:
                    //Notify the message to the user
                    Notify(msg, kind);
                    break;
                case MessagePriority.Low:
                    //Log the message. 
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Delets the log file and creates a new one. 
        /// </summary>
        static void DeleteLog()
        {
            logFile.Close();
            logFile = null;
            File.Delete("logs.log");
            logFile = new StreamWriter("logs.log", false);
            logFile.AutoFlush = true;
            logFile.Write("XKeyboard".Fill(80, '-') + "\r\n");
        }
    }   
}
