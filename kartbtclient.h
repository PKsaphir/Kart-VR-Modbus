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

    explicit  KartBtClient(const QString& configFile, QObject *parent = 0);



       //accessseurs

       float get_cons_pente( ) { return cons_pente ; }
       float get_masse_pilote() { return masse_pilote; }


       int get_mode() { return mode_local_distant; }




       void set_cons_pente(float pente) { cons_pente = pente; }

       void set_masse_pilote(float m) { masse_pilote = m ; }


       void set_mode(int mode) { mode_local_distant = mode ;}




   public  slots:
    void info(const QString& src, const QString& msg ) ;
    void readConsole();


   signals:
     void quit() ;   // signal à émettre pour terminer l'application...

   private:
       void qSleep(int ms);
       QamModbusMap*				m_map ;
       QamModbusMap::PrimaryTable	m_table ;

       QamTcpClient*		m_client ;
       QSocketNotifier*    m_notifier ;
       float  couple_simu, cons_pente, masse_pilote, cons_couple;

       int mode_local_distant/*, masse_pilote*/;


       //réception données de MESURES   Read ONLY



       float getVitSimu();    //tr/mn-1

       float getVitKart();   //km/h

       float getCoupleKart();

       float getCoupleSimu();  // le meme que KART ??



       float getCourantBatt(); // courant de decharge totale (batteries en series ! donc peu importe le numero de batterie
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
