#include <QCoreApplication>


#include "kartbtclient.h"

int main(int argc, char *argv[])
{
     QCoreApplication app(argc, argv ) ;




        QStringList	args = QCoreApplication::arguments() ;

        if ( args.size() < 2 ) {
            cerr << "usage: " << qPrintable( args.at(0) ) << "<configFile>" << endl ;
            return -1 ;
        }
        KartBtClient* bt = new KartBtClient(args.at(1),&app);

        bt->readConsole();
        bt->get_masse_pilote();



        QObject::connect(bt, SIGNAL(quit()), &app, SLOT(quit()) ) ;

        /*float test1 = bt->get_masse_pilote();
        qDebug("%f", test1);
        test1 = bt->get_local_masse();
        qDebug("%f", test1);*/

        return app.exec() ;
}
