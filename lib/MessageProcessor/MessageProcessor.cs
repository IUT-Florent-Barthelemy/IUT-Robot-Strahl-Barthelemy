using System;
using System.Text;
using EvArgsLibrary;
using Robot;

/// <summary>
/// this class is used to process the previously decoded message and output events
/// depending on the message for a simpler, higer level use.
/// </summary>

namespace MessageProcessor
{
    public class msgProcessor
    { 
        //contains the processor state machine
        public void ProcessMessage(object Sender, DataDecodedArgs e)
        {
            ushort function = e.DecodedFunction;
            byte[] irDistance = new byte[5];

            robot.MotorWays wayGauche;
            robot.MotorWays wayDroit;

            sbyte speedGauche;
            sbyte speedDroit;

            if (e.CheckSumErrorOccured)
                OnCheckSumErrorOccured();
            else 
            { 
                switch(function)
                {
                    case 0x0080: //is TextMessage
                        OnTextMessageProcessed(Encoding.UTF8.GetString(e.DecodedPayload));
                        break;

                    case 0x0030: //is IrMessage
                        for (int i = 0; i < e.DecodedPayload.Length; i++)
                            irDistance[i] = e.DecodedPayload[i];

                        OnIrMessageProcessed(irDistance);
                        break;

                    case 0x0040: //is SpeedMessage
                        speedGauche = (sbyte)e.DecodedPayload[0];
                        speedDroit = (sbyte)e.DecodedPayload[1];

                        if (speedGauche >= 0) //if 0, saying foward...
                            wayGauche = robot.MotorWays.Avance;
                        else                                   //first bit is a 0, going reverse
                            wayGauche = robot.MotorWays.Recule;

                        if(speedDroit >= 0)   //first bit is a 1, going foward
                            wayDroit = robot.MotorWays.Avance;
                        else                                   //first bit is a 0, going reverse
                            wayDroit = robot.MotorWays.Recule;

                        OnSpeedMessageProcessed(speedGauche, speedDroit,
                                                wayGauche, wayDroit);

                        break;

                    case 0x0061:
                        ulong timestamp = 0;


                        timestamp =  (ulong)(e.DecodedPayload[0] << 24);
                        timestamp += (ulong)(e.DecodedPayload[1] << 16);
                        timestamp += (ulong)(e.DecodedPayload[2] << 8);
                        timestamp += (ulong)(e.DecodedPayload[3] << 0);
                        float xPosFromodo = BitConverter.ToSingle(e.DecodedPayload, 4);
                        float yPosFromodo = BitConverter.ToSingle(e.DecodedPayload, 8);
                        float angleRadOdo = BitConverter.ToSingle(e.DecodedPayload, 12);
                        float vitesseLneaireOdo = BitConverter.ToSingle(e.DecodedPayload, 16);
                        float vitesseAngulaireOdo = BitConverter.ToSingle(e.DecodedPayload, 20);
                        //
                        float vitesseDroit = BitConverter.ToSingle(e.DecodedPayload, 24);
                        float vitesseGauche = BitConverter.ToSingle(e.DecodedPayload, 28);
                        float deltaD = BitConverter.ToSingle(e.DecodedPayload, 32);
                        float deltaG = BitConverter.ToSingle(e.DecodedPayload, 36);
                        float deltaTheta = BitConverter.ToSingle(e.DecodedPayload, 40);
                        float deltaS = BitConverter.ToSingle(e.DecodedPayload, 44);

                        onPositionDataReceived(timestamp, xPosFromodo, yPosFromodo, angleRadOdo, vitesseLneaireOdo,
                                               vitesseAngulaireOdo, vitesseDroit, vitesseGauche, deltaD, deltaG, deltaTheta, deltaS);
                        break;
                }
            }

        }


        public event EventHandler<PositionDataArgs> onPositionDataProcessedEvent;
        public event EventHandler<TextDataProcessedArgs> OnTextMessageProcessedEvent;
        public event EventHandler<IrDataProcessedArgs> OnIrMessageProcessedEvent;
        public event EventHandler<SpeedDataProcessedArgs> OnSpeedMessageProcessedEvent;
        public event EventHandler<CheckSumErrorOccuredArgs> OnCheckSumErrorOccuredEvent;

        public virtual void onPositionDataReceived(ulong timestamp, float xposFromOdometry, float yposFromOdometry, float angleRadFromOdometry,
                                                   float vitesseLineaireFromOdometry, float vitesseAngulaireFromOdometry, float vitesseDroit, float vitesseGauche, float deltaD, float deltaG, float deltaTheta, float deltaS)
        {
            var handler = onPositionDataProcessedEvent;
            if(handler != null)
            {
                handler(this, new PositionDataArgs
                {
                    Timestamp = timestamp,
                    XposFromOdometry = xposFromOdometry,
                    YposFromOdometry = yposFromOdometry,
                    AngleRadFromOdometry = angleRadFromOdometry,
                    VitesseLineaireFromOdometry = vitesseLineaireFromOdometry,
                    VitesseAngulaireFromOdometry = vitesseAngulaireFromOdometry,
                    VitesseDroit = vitesseDroit,
                    VitesseGauche = vitesseGauche,
                    DeltaD = deltaD,
                    DeltaG = deltaG,
                    DeltaS = deltaS,
                    DeltaTheta = deltaTheta
                });
            }
        }

        public virtual void OnCheckSumErrorOccured()
        {
            var handler = OnCheckSumErrorOccuredEvent;
            if(handler != null)
            {
                handler(this, new CheckSumErrorOccuredArgs{});
            }
        }
        public virtual void OnIrMessageProcessed(byte[] distance)
        {
            var handler = OnIrMessageProcessedEvent;
            if(handler != null)
            {
                handler(this, new IrDataProcessedArgs
                {
                    Distance = distance
                });
            }
        }
        public virtual void OnTextMessageProcessed(string text)
        {
            var handler = OnTextMessageProcessedEvent;
            if(handler != null)
            {
                handler(this, new TextDataProcessedArgs
                {
                    ProcessedText = text
                });
            }
        }
        public virtual void OnSpeedMessageProcessed(sbyte speedGauche, sbyte speedDroit, robot.MotorWays wayGauche, robot.MotorWays wayDroit)
        {
            var handler = OnSpeedMessageProcessedEvent;
            if(handler != null)
            {
                handler(this, new SpeedDataProcessedArgs
                {
                    SpeedGauche = speedGauche,
                    SpeedDroit = speedDroit,
                    WayGauche = wayGauche,
                    WayDroit = wayDroit
                });
            }
        }
    }
}
