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
        return app.exec() ;
}
