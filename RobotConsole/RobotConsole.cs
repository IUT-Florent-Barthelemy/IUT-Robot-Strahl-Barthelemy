using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using EvArgsLibrary;
using GUI;

using ExtendedSerialPort;
using MessageDecoder;



namespace RobotConsole
{
    class RobotConsole
    {
        static ExtendedSerialPort.ReliableSerialPort port = new ReliableSerialPort("COM8", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        static MessageDecoder.msgDecoder decoder = new msgDecoder();
        static MessageProcessor.msgProcessor processor = new MessageProcessor.msgProcessor();
        static interfaceRobot UI;

        static bool usingUI = true;

        static void Main(string[] args)
        {
            port.DataReceived += decoder.DecodeMessage;
            decoder.OnDataDecodedEvent += processor.ProcessMessage;
            processor.onPositionDataProcessedEvent += Processor_onPositionDataProcessedEvent;
            

            //port.Open();

            if (usingUI)
                StartRobotInterface();

            Thread.CurrentThread.Join();
         }

            private static void Processor_onPositionDataProcessedEvent(object sender, EvArgsLibrary.PositionDataArgs e)
        {
            Console.WriteLine("X: " + e.XposFromOdometry);    
            Console.WriteLine("Y: " + e.YposFromOdometry);    
            Console.WriteLine("VitesseLin: " + e.VitesseLineaireFromOdometry);    
            Console.WriteLine("VitesseAng: " + e.VitesseAngulaireFromOdometry);    
            Console.WriteLine("AngleRad: " + e.AngleRadFromOdometry);
            Console.WriteLine("VitesseGauche: " + e.VitesseGauche);
            Console.WriteLine("VitesseDroit: " + e.VitesseDroit);
        }

        static Thread ui_thread;
        static void StartRobotInterface()
        {
            ui_thread = new Thread(() =>
            {
                UI = new interfaceRobot();
                processor.onPositionDataProcessedEvent += UI.UpdateLocation;
                UI.ShowDialog();
            });
            ui_thread.SetApartmentState(ApartmentState.STA);
            ui_thread.Start();
        }
    }
}

