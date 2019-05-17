#ifndef KARTBTCLIENT_H
#define KARTBTCLIENT_H

#include "qamtcpclient.h"
#include "qammodbusmap.h"

#include <QObject>
#include <QSocketNotifier>
#include <QTimer>
#include <iostream>
using namespace std ;

//#define CONTACTEUR_SEUL 2018    //si decommenter cde seule des contacteurs

class KartBtClient:public QObject
{

    Q_OBJECT
    public :



       //init classe metier avec nom de fichier csv en arg

    explicit  KartBtClient(const QString& configFile, QObject *parent = nullptr);



       //accessseurs

       float get_cons_pente( ) { return cons_pente ; }
       float get_masse_pilote() { return masse_pilote; }


       int get_mode() { return mode_local_distant; }




       void set_cons_pente(float pente) { cons_pente = pente; }

       void set_masse_pilote(float m) { masse_pilote = m ; }


       void set_mode(int mode) { mode_local_distant = mode ;}



       float get_vit_simu() { return vit_simu ; }

       float get_couple_simu() { return couple_simu ; }

       float get_courant_charge_batt() { return courant_charge_batt ; }

       float get_puissance_totale() { return puissance_totale ; }

       float get_tension_batt1() { return tension_batt1 ; }

       float get_tension_batt2() { return tension_batt2 ; }

       float get_tension_batt3() { return tension_batt3 ; }

       float get_tension_batt4() { return tension_batt4 ; }

       float get_charge_totale_batt() { return charge_totale_batt ; }

       float get_courant_batt1() { return courant_batt1 ; }




   public  slots:
    void info(const QString& src, const QString& msg ) ;
    void readConsole();
    float getCourantBatt(); //TEST

   signals:
     void quit() ;   // signal à émettre pour terminer l'application...

   private:
       void qSleep(int ms);
       QamModbusMap*				m_map ;
       QamModbusMap::PrimaryTable	m_table ;

       QamTcpClient*		m_client ;
       QSocketNotifier*    m_notifier ;
       float  couple_simu, cons_pente, masse_pilote, cons_couple, vit_simu, courant_charge_batt, puissance_totale, tension_batt1, tension_batt2, tension_batt3, tension_batt4, charge_totale_batt, courant_batt1;

       int mode_local_distant/*, masse_pilote*/;


       //réception données de MESURES   Read ONLY



       float getVitSimu();    //tr/mn-1

       float getVitKart();   //km/h

       float getCoupleKart();

       float getCoupleSimu();  // le meme que KART ??



       //float getCourantBatt(); // courant de decharge totale (batteries en series ! donc peu importe le numero de batterie
                                        //utile pour simulateur






       //envoi / lecture donnees de CONSIGNE   R/W
       void setMasse_Pilote(QString weight);
       float getMasse_Pilote();
       //int getMasse_Pilote();




       void setConsignePente(QString pente);
       float getConsignePente();

       void setLocal_Distant(QString local_dist);
       int getLocal_Distant();



       float getCourantChargeBatt();

       float getPuissanceTotaleBatt();

       float getTensionBatt1();
       float getTensionBatt2();
       float getTensionBatt3();
       float getTensionBatt4();

       float getChargeTotaleBatt();





   };

#endif // KARTBTCLIENT_H
