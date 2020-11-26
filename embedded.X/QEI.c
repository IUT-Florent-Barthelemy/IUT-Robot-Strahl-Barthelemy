#include <xc.h>
#include "Robot.h"
#include "Toolbox.h"
#include "QEI.h"
#include "UART.h"
#include "timer.h"
#include <math.h>

#define DIAM_CODEUR_ROUE 0.0426 //en m
#define POINT_TO_METER  (PI*DIAM_CODEUR_ROUE)/8000

void InitQEI1()
{
    QEI1IOCbits.SWPAB = 1; //QEAX and QEBx are swapped
    QEI1GECL = 0xFFFF;
    QEI1GECH = 0xFFFF;
    QEI1CONbits.QEIEN = 1; //Enable QEI Module
}

void InitQEI2()
{
    QEI2IOCbits.SWPAB = 1;
    QEI2GECL = 0xFFFF;
    QEI2GECH = 0XFFFF;
    QEI2CONbits.QEIEN = 1;
}

void QEIUpdateData()
{
    //save old values
    double QeiDroitPosition_T_1 = robotState.QeiDroitPosition;
    double QeiGauchePosition_T_1 = robotState.QeiGauchePosition;
    
    //refresh position values
    long QEI1RawValue = POS1CNTL;
    QEI1RawValue += ((long)POS1HLD << 16);
    
    long QEI2RawValue = POS2CNTL;
    QEI2RawValue += ((long)POS2HLD << 16);
    
    //convert to mm
    robotState.QeiDroitPosition = (POINT_TO_METER*QEI1RawValue);
    robotState.QeiGauchePosition = (-POINT_TO_METER*QEI2RawValue);
    
    //computing position deltas
    double delta_d = robotState.QeiDroitPosition - QeiDroitPosition_T_1;
    double delta_g = robotState.QeiGauchePosition - QeiGauchePosition_T_1;
    
    //delta_tetha = atan((atan(delta_d - delta_d)/DISTROUES);
    double delta_theta = 2*(delta_d - delta_g)/DISTROUES;
    double ds = (delta_d + delta_g) / 2; //delta_s
    
    //speed computation
    //ATTENTION: multiplier par la freq d'�chantillonage
    
    robotState.vitesseDroitFromOdometry = delta_d*FREQ_ECH_QEI;
    robotState.vitesseGaucheFromOdometry = delta_g*FREQ_ECH_QEI;
    robotState.vitesseLineaireFromOdometry = (robotState.vitesseDroitFromOdometry + robotState.vitesseGaucheFromOdometry)/2;
    robotState.vitesseAngulaireFromOdometry = delta_theta*FREQ_ECH_QEI;
    
    //mise � jour du positionnement terrain � t-1
    robotState.xPosFromOdometry_1 = robotState.xPosFromOdometry;
    robotState.yPosFromOdometry_1 = robotState.yPosFromOdometry;
    robotState.angleRadianFromOdometry_1 = robotState.angleRadianFromOdometry;
    
    //calcul des positions dans le referentiel du terrain
    double theta_rad = robotState.angleRadianFromOdometry_1 + delta_theta;
    robotState.xPosFromOdometry = robotState.xPosFromOdometry_1 + (robotState.vitesseLineaireFromOdometry/FREQ_ECH_QEI)*cos(theta_rad);
    robotState.yPosFromOdometry = robotState.yPosFromOdometry_1 + (robotState.vitesseLineaireFromOdometry/FREQ_ECH_QEI)*sin(theta_rad);
    robotState.angleRadianFromOdometry = theta_rad; 
    
    if(robotState.angleRadianFromOdometry > PI)
        robotState.angleRadianFromOdometry -= 2*PI;;
    if(robotState.angleRadianFromOdometry < -PI)
        robotState.angleRadianFromOdometry += 2*PI;
}

 void SendPositionData()
{
    unsigned char positionPayload[48];
    getBytesFromInt32(positionPayload, 0, timestamp);
    getBytesFromFloat(positionPayload, 4, (float)robotState.xPosFromOdometry);
    getBytesFromFloat(positionPayload, 8, (float)robotState.yPosFromOdometry);
    getBytesFromFloat(positionPayload, 12, (float)robotState.angleRadianFromOdometry);
    getBytesFromFloat(positionPayload, 16, (float)robotState.vitesseLineaireFromOdometry);
    getBytesFromFloat(positionPayload, 20, (float)robotState.vitesseAngulaireFromOdometry);
    //debug
    getBytesFromFloat(positionPayload, 24, (float)robotState.vitesseDroitFromOdometry);
    getBytesFromFloat(positionPayload, 28, (float)robotState.vitesseGaucheFromOdometry);
    getBytesFromFloat(positionPayload, 32, (float)robotState.delta_d);
    getBytesFromFloat(positionPayload, 36, (float)robotState.delta_g);
    getBytesFromFloat(positionPayload, 40, (float)robotState.delta_theta);
    getBytesFromFloat(positionPayload, 44, (float)robotState.ds);

    

    UartEncodeAndSendMessage(CMD_POSITION_DATA, 48, positionPayload);
}