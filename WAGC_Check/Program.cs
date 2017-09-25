using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Net.Mail;
 
 
namespace WAGC_Check
{
    class Program
    {
        public static void Main()
        {
 
            System.Diagnostics.Process[] Udtprocess = Process.GetProcessesByName("Udt");
 
            MailMessage objeto_mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = "smtp.carleton.edu";
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //use the windows AD credential of user instead of passing user/pw
            client.UseDefaultCredentials = true;
            // client.Credentials = new System.Net.NetworkCredential("user", "Password");
            objeto_mail.From = new MailAddress("datatel@carleton.edu");
            objeto_mail.To.Add(new MailAddress("colleague-admin@carleton.edu"));
            objeto_mail.To.Add(new MailAddress("nweeg@carleton.edu"));
            objeto_mail.To.Add(new MailAddress("jkramer@carleton.edu"));

            if (Udtprocess.Length > 0)
            { 
                var commandLine = new StringBuilder("Udt");
                bool processFound = false;
 
                using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE name like 'Udt%'"))
                {
                    foreach (var @object in searcher.Get())
                    {
                        commandLine.Append(@object["CommandLine"] + " ");
                        Console.WriteLine("Commandline:{0}", commandLine.ToString());
                        string cmd = commandLine.ToString();
 
                        // check command line string for active Colleague system process
                        if (cmd.Contains("RESTART.DAS.SESSION"))
                        {
                            processFound = true;
                        }
                    }
                    if (processFound == true)
                    {
                        Console.WriteLine("WAGC is running!");
                        //objeto_mail.Subject = "WAGC is running!";
                        //objeto_mail.Body = "WAGC is running!";
                        //client.Send(objeto_mail);
                    }
                    else
                    {
                        Console.WriteLine("WAGC is not running!");
                        objeto_mail.Subject = "WAGC is not running!";
                        objeto_mail.Body = "Garbage Collection is NOT running! Restart from WAGC logged in as ellucian";
                        client.Send(objeto_mail);
 
                    }
                }
            }
        }
    }
}