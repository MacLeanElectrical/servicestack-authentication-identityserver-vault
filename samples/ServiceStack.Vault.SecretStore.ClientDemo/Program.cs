﻿namespace ServiceStack.Vault.SecretStore.ClientDemo
{
    using System;
    using System.Diagnostics;
    using Text;

    class Program
    {
        static void Main(string[] args)
        {
            // Now start up service stack client
            new AppHost("http://localhost:5001/").Init().Start("http://*:5001/");
            "ServiceStack Self Host with Razor listening at http://localhost:5001 ".Print();
            Process.Start("http://localhost:5001/");

            Console.ReadLine();
        }
    }
}