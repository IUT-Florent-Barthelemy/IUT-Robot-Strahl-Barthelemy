/* 
 * File:   Robot.h
 * Author: TP-EO-1
 *
 * Created on 6 f�vrier 2020, 15:26
 */

#ifndef ROBOT_H
#define	ROBOT_H

#define SPEED 0x0040
#define TEXT 0x0080
#define LED 0x0020

#define CODE_LED_ORANGE 1
#define CODE_LED_BLEUE 2
#define CODE_LED_BLANCHE 3


typedef struct robotStateBITS
{
    union
    {
        struct
        {
            unsigned char taskEnCours;
            
            float vitesseGaucheConsigne;
            float vitesseGaucheCommandeCourante;
            float vitesseDroiteConsigne;
            float vitesseDroiteCommandeCourante;
            
            
            double Kp_lin;
            double Ki_lin;
            double kd_lin;
            
            double Kp_angl;
            double Ki_angl;
            double Kd_angl;
            
            double vitesseLineaireConsigne;
            double vitesseAngulaireConsigne;

            
            float QeiDroitPosition;
            float QeiGauchePosition;
            
            double vitesseDroitFromOdometry;
            double vitesseGaucheFromOdometry;
            double vitesseLineaireFromOdometry;
            double vitesseAngulaireFromOdometry;
            
            double xPosFromOdometry_1;
            double xPosFromOdometry;
            double yPosFromOdometry_1;
            double yPosFromOdometry;
            double angleRadianFromOdometry_1;
            double angleRadianFromOdometry;
            double delta_d;
            double delta_g;
            double delta_theta;
            double ds;
        };
    };
}ROBOT_STATE_BITS;
extern volatile ROBOT_STATE_BITS robotState;
#endif /*ROBOT_H*/