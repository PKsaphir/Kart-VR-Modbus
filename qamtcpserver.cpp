/* ----------------------------------------------------------------------------
 |	Nom :			qamtcpserver.cpp
 |	Projet :		Qam Modbus over TCP/IP
 |	Sujet :     	Serveur TCP multithread
 |  Auteur :		Alain Menu
 |	Version :		1.0
 |	Création :		janvier 2014
 |	Mise à jour :	08/02/2014
 |	Fabrication :	Qt4 / Qt5 OpenSource (Desktop)
 + ------------------------------------------------------------------------- */
/*
 |	Copyright (c) 2014 by Alain Menu <alain.menu@ac-creteil.fr>
 |
 |  This file is part of "Qam Modbus over TCP/IP project"
 |
 |  This program is free software ;  you can  redistribute it and/or  modify it
 |  under the terms of the  GNU General Public License as published by the Free
 |  Software Foundation ; either version 2 of the License, or  (at your option)
 |  any later version.
 |
 |  This program is distributed in the hope that it will be useful, but WITHOUT
 |  ANY WARRANTY ; without even the  implied  warranty  of  MERCHANTABILITY  or
 |  FITNESS FOR  A PARTICULAR PURPOSE. See the  GNU General Public License  for
 |  more details < http://www.gnu.org/licenses/gpl.txt >.
 + ------------------------------------------------------------------------- */

#include "qamtcpserver.h"

/*!
 * Constructeur.
 * \param server : serveur "métier" à utiliser.
 * \param parent : parent Qt.
 */

QamTcpServer::QamTcpServer(QamAbstractServer* server, QObject* parent )
	: QTcpServer(parent)
	, m_port( 4000 )
	, m_dataServer( server )
{
}

/*!
 * Démarrage et mise en écoute du serveur.
 * \param listenPort : port de service.
 */

void QamTcpServer::start(int listenPort )
{
	m_port = listenPort ;
	if ( !this->listen(QHostAddress::Any, m_port ) ) {
		m_dataServer->networkInfo( "Could not start server: " + this->errorString() ) ;
	}
	else {
		m_dataServer->networkInfo( QString("Listening to port %1...").arg(m_port) ) ;
	}
}

/*!
 * Réponse à une demande de connexion par création d'un nouveau thread dédié de classe
 * QamTcpConnection à qui est transmis l'adresse du serveur "métier" à utiliser
 * comme interlocuteur.
 * \param socketDescriptor : descripteur de socket associé à la connexion.
 */

void QamTcpServer::incomingConnection(qintptr socketDescriptor )
{
	QamTcpConnection* tcpConnection = new QamTcpConnection(socketDescriptor, m_dataServer, this ) ;

	connect( tcpConnection, SIGNAL(finished()),
			 tcpConnection, SLOT(deleteLater()) ) ;

	tcpConnection->start() ;
}
