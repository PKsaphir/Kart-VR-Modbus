#include "kartbtclient.h"



//version cliente classique
KartBtClient::KartBtClient(const QString& configFile ,QObject* parent )
    : QObject(parent)
{

    // cartographie Modbus
    m_map    = new QamModbusMap( QamModbusMap::ClientMode, this ) ;
    m_map->setVerbose( true ) ;
    //m_map->setServerAvailable(true);
    connect( m_map, SIGNAL(info(QString,QString)), this, SLOT(info(QString,QString)) ) ;


      bool retour = m_map->loadMap( configFile );
      m_table = QamModbusMap::HoldingRegister ;
    // client TCP
    m_client = new QamTcpClient( m_map, this ) ;
    m_client->sockConnect( m_map->host(), m_map->port() ) ;
    cout<< "host distant :" << m_map->host().toStdString()  << "port " <<  m_map->port() << endl;


     cout<< "etat connexion" << m_client->state();
    // intercepteur de commandes clavier

   m_notifier = new QSocketNotifier(fileno(stdin), QSocketNotifier::Read, this ) ;
     connect(m_notifier, SIGNAL(activated(int)), this, SLOT(readConsole()) ) ;

   int thing = 0;


    while(retour == false)
    {
        qSleep(1000);
        if(thing >= 5) {
        cerr << "pb fichier csv non trouve" << endl;
        exit(-1) ;
        }
        thing++;

    }



}


void KartBtClient::readConsole()
{




    QTextStream in(stdin) ;


    QString line = in.readLine() ;   //force evitement retour  CR
    QStringList parse = line.split( QRegExp("\\s+") ) ;




           //lecture vitesse simulateur tr/mn
    if (( parse.at(0) == "1" )){
                //aTTENTION à na pas confondre les methodes d'acces modbus qui lisent dans l'automate des "simple" accesseurs

                    //lecture vitesse  bt->getVitSimu() LECTURE MODBUS en reseau ... et mise dans attribut   de l'objet KartBtClient

                    cout << "Vitesse simulateur (arbre roue) " << qPrintable(QString::number(this->getVitSimu() , 'f', 6 )) << "tr/mn " << endl ;

    }
    if(( parse.at(0) == "2" )){

                    cout << "Couple " << qPrintable( QString::number(this->getCoupleSimu(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "3" )){

                    cout << "Masse " << qPrintable( QString::number(this->getMasse_Pilote(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "4" )){

                    //lecture pente  par bt->getConsignePente()  LECTURE MODBUS en reseau ...et mise dans attribut pente de l'objet KartBtClient

                    cout << "Pente " << qPrintable( QString::number(this->getConsignePente(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "5" )){

                    cout << "Local_distant " << qPrintable( QString::number(this->getLocal_Distant(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "6" )){

                    cout << "Courant_charge_batt " << qPrintable( QString::number(this->getCourantChargeBatt(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "7" )){

                    cout << "Puissance_totale_batt " << qPrintable( QString::number(this->getPuissanceTotaleBatt(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "8" )){

                    cout << "Tension_batt1 " << qPrintable( QString::number(this->getTensionBatt1(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "9" )){

                    cout << "Tension_batt2 " << qPrintable( QString::number(this->getTensionBatt2(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "10" )){

                    cout << "Tension_batt3 " << qPrintable( QString::number(this->getTensionBatt3(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "11" )){

                    cout << "Tension_batt4 " << qPrintable( QString::number(this->getTensionBatt4(),'f',6) ) << "% " << endl ;
    }
    if(( parse.at(0) == "12" )){

                    cout << "Charge_totale_batt " << qPrintable( QString::number(this->getChargeTotaleBatt(),'f',6) ) << "% " << endl ;
    }




    if(( parse.at(0) == "13" ) && ( parse.size() >= 2 )){

                    //ecriture masse pilote


                    this->setMasse_Pilote(parse.at(1));
        cout << "masse pilote  " << qPrintable(  QString::number(this ->getMasse_Pilote() , 'f', 6)) << "kg" << endl;



    }
    else if(( parse.at(0) == "13" ) && ( parse.size() ==1 )){
        cerr << "Entrez 13  et la masse pilote  kg" << endl;


    }

    if(( parse.at(0) == "14" ) && ( parse.size() >= 2 )){    //attention 2 arguments seult sur ligne de commande !!  >=

                    // ecriture consigne pente suppose être en distant (votre IHM...)  Cf csv
                    this->setConsignePente(  parse.at(1));
        cout << "consigne pente  " << qPrintable(  QString::number(this ->getConsignePente() , 'f', 6)) << "%" << endl;

    }
    //erreur
    else if(( parse.at(0) == "14" ) && ( parse.size() ==1 )){
        cerr << "Entrez 14  et la valeur de pente %" << endl;

    }

    if(( parse.at(0) == "15" ) ){ //lecture mode local / distant

                //lecture mode

             this->set_mode(this->getLocal_Distant());
             if(this->get_mode()==1)  cout << "Mode distant " << endl;

            else cout << "Mode local " << endl;

    }





    //courant decharge batterie : celui quand on se sert du kart
    if(( parse.at(0) == "16" ) && ( parse.size() == 1 )){    //attention 1 seul argument seult sur ligne de commande !!  >=

        float courant_decharge = this->getCourantBatt(  );
               courant_decharge /= 1000.0f;  //car facteur 10000 pour 10V dans prog automate


                                                             //PM Maj 9/5/18

        cout << "courant decharge batteries  " << qPrintable(  QString::number( courant_decharge , 'f', 6)) << "A" << endl;

    }
    //erreur
    else if(( parse.at(0) == "16" ) && ( parse.size() >=2 )){
        cerr << "entrez 16  " << endl;

    }


    if ( parse.at(0) == "q" ) {
        emit quit() ;
    }

    cout << "tapez votre choix : ou q pour quitter" << endl;

    cout << "1 : Lecture vitesse simulateur " << endl;
    cout << "2 : Lecture couple " << endl;
    cout << "3 : Lecture masse " << endl;
    cout << "4 : Lecture pente " << endl;
    cout << "5 : Lecture local distant " << endl;
    cout << "6 : Lecture courant charge batt " << endl;
    cout << "7 : Lecture puissance totale batt " << endl;
    cout << "8 : Lecture tension batt1 " << endl;
    cout << "9 : Lecture tension batt2 " << endl;
    cout << "10 : Lecture tension batt3 " << endl;
    cout << "11 : Lecture tension batt4 " << endl;
    cout << "12 : Lecture charge totale batt " << endl;

    cout << "13 : Ecriture masse pilote kg; tapez 13 [0,n]" << endl;
    cout << "14 : Ecriture consigne pente %;  tapez 14 [0,n] " << endl;
    cout << "15 : Ecriture local_distant;  tapez 15 [0,n] " << endl;

    cout << "16 : Lecture courant reel decharge batterie ; tapez 16   " << endl << endl;




}


void KartBtClient::info(const QString& src, const QString& msg )
{
    cout << qPrintable( src ) << ": " << qPrintable( msg ) << endl ;
}
#ifdef Q_OS_WIN
#include <windows.h> // for Sleep
#endif
void KartBtClient::qSleep(int ms)
{
    //QTEST_ASSERT(ms > 0);

#ifdef Q_OS_WIN
    Sleep(uint(ms));
#else
    struct timespec ts = { ms / 1000, (ms % 1000) * 1000 * 1000 };
    nanosleep(&ts, NULL);
#endif
}

// DONNEES MESUREES LECTURE SEULE
float KartBtClient::getVitKart()
{
    return this->m_map->remoteValue(m_table, "vit_kart").toFloat();
}

float KartBtClient::getCoupleSimu()
{
    return   this->m_map->remoteValue(m_table, "couple_simu").toFloat();
}

//vitesse simulateur (moteur  : tr/mn)
float KartBtClient::getVitSimu()
{
    return   this->m_map->remoteValue(m_table, "vit_simu").toFloat();
}

float KartBtClient::getCoupleKart()
{
    return   this->m_map->remoteValue(m_table, "couple_kart").toFloat();
}

//TO DO PM 5/5/18 completer csv  !!

//lit le mode local 1 ; distant : 0
int KartBtClient::getLocal_Distant()
{

      return   this->m_map->remoteValue( m_table,"local_dist").toInt();


}
/** fn  float  KartBtClient::getCourantBatt()
 * brief mesure courant de decharge batterie 1,2,3,4 : batteries en serie !

*/
float  KartBtClient::getCourantBatt()
{
    //return   this->m_map->remoteValue( m_table, "courant_batt1").toFloat(); //courant de decharge batterie 1, le meme que 2,3,4 !
        //utile pour simulateur
    return   this->m_map->remoteValue( m_table, "courant_batt1").toFloat();

}


void KartBtClient::setMasse_Pilote(QString weight)
{

     this -> m_map->setRemoteValue(m_table, "masse_pilote",weight );
    //this -> m_map->setRemoteValue(m_table, "masse_ihm_pilote",weight );//modif pour test 11/10


}

float  KartBtClient::getMasse_Pilote()
{
    return   this->m_map->remoteValue(m_table, "masse_pilote").toFloat();

}


void KartBtClient::setConsignePente(QString pente)
{
    m_table = QamModbusMap::HoldingRegister;

     this->m_map->setRemoteValue(m_table, "cons_pente", pente);

}
float KartBtClient::getConsignePente()
{
    return   this->m_map->remoteValue(m_table, "cons_pente").toFloat();
}


float KartBtClient::getCourantChargeBatt()
{
    return   this->m_map->remoteValue(m_table, "courant_charge_batt").toFloat();
}

float KartBtClient::getPuissanceTotaleBatt()
{
    return   this->m_map->remoteValue(m_table, "puissance_totale").toFloat();
}

float KartBtClient::getTensionBatt1()
{
    return   this->m_map->remoteValue(m_table, "tension_batt1").toFloat();
}
float KartBtClient::getTensionBatt2()
{
    return   this->m_map->remoteValue(m_table, "tension_batt2").toFloat();
}
float KartBtClient::getTensionBatt3()
{
    return   this->m_map->remoteValue(m_table, "tension_batt3").toFloat();
}
float KartBtClient::getTensionBatt4()
{
    return   this->m_map->remoteValue(m_table, "tension_batt4").toFloat();
}

float KartBtClient::getChargeTotaleBatt()
{
    return   this->m_map->remoteValue(m_table, "charge_totale_batt").toFloat();
}




