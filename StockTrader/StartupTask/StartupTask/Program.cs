using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Data;
using System.Data.SqlClient;

namespace StartupTask
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower().Equals("start"))
                {

                    ProcessStartInfo startInfo = new ProcessStartInfo("startup\\startuptask.exe");
                    startInfo.CreateNoWindow = true;
                    try
                    {
                        Process exeProcess = Process.Start(startInfo);
                    }
                    catch 
                    {
                       
                    }
                    return;
                }
            }
            else
            {
                const string semaphoreName = "SemaphoreShutDown";
                Semaphore sem = null;
                bool doesNotExist = false;
                // Attempt to open the named semaphore.
                try
                {
                    sem = Semaphore.OpenExisting(semaphoreName);
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    doesNotExist = true;
                }

                if (doesNotExist)
                {
                    // The semaphore does not exist, so create it.
                    //
                    // The value of this variable is set by the semaphore
                    // constructor. It is true if the named system semaphore was
                    // created, and false if the named semaphore already existed.
                    //
                    bool semaphoreWasCreated;


                    string user = "Everyone";
                    SemaphoreSecurity semSec = new SemaphoreSecurity();

                    SemaphoreAccessRule rule = new SemaphoreAccessRule(user,
                        SemaphoreRights.Synchronize | SemaphoreRights.Modify,
                        AccessControlType.Allow);
                    semSec.AddAccessRule(rule);

                    // Create a Semaphore object 
                    sem = new Semaphore(1, 1, semaphoreName,
                        out semaphoreWasCreated, semSec);
                    if (!semaphoreWasCreated)
                        return;
                }

                // Enter the semaphore, and hold it until the program
                // exits.
                //
                try
                {
                    sem.WaitOne();
                    sem.WaitOne();
                    ProcessStartInfo startInfo = new ProcessStartInfo("NET");
                    startInfo.Arguments = "STOP W3SVC";
                    startInfo.CreateNoWindow = false;
                    try
                    {
                        // Start the process with the info we specified.
                        // Call WaitForExit and then the using statement will close.
                        using (Process exeProcess = Process.Start(startInfo))
                        {
                            exeProcess.WaitForExit();
                            int exitCode = exeProcess.ExitCode;
                        }
                    }
                    catch 
                    {
                    }
                    sem.Release();
                }
                catch 
                {
                }
            }
        }
    }
}
