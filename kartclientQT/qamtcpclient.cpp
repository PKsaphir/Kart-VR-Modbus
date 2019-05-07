/* ----------------------------------------------------------------------------
 |	Nom :			qamtcpclient.cpp
 |	Projet :		Qam Modbus over TCP/IP
 |	Sujet :     	Client TCP
 |  Auteur :		Alain Menu
 |	Version :		1.0
 |	Création :		janvier 2014
 |	Mise à jour :	09/02/2014
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

#include "qamtcpclient.h"
#include <QHostAddress>

/*!
 * Constructeur d'un client TCP avec l'objet serveur à utiliser comme émetteur de
 * requêtes et destinataire des réponses.
 * \param server : serveur "métier" à utiliser.
 * \param parent :: parent Qt.
 */

QamTcpClient::QamTcpClient(QamAbstractServer* server, QObject* parent )
	: QTcpSocket(parent)
	, m_server( server )
{
	connect( this, SIGNAL( connected() ),
			 this, SLOT(   sockConnected() ) ) ;
	connect( this, SIGNAL( disconnected() ),
			 this, SLOT(   sockDisconnected() ) ) ;
	connect( this, SIGNAL( readyRead() ),
			 this, SLOT(   sockRead() ) ) ;
	connect( this, SIGNAL( error(QAbstractSocket::SocketError ) ),
			 this, SLOT(   sockError(QAbstractSocket::SocketError ) ) ) ;

	if ( m_server ) {

		connect( this,		SIGNAL( sockReceive(QByteArray) ),
				 m_server,	SLOT(   response(QByteArray) ) ) ;

		connect( m_server,	SIGNAL( request(QByteArray) ),
				 this,		SLOT(   sockWrite(QByteArray) ) ) ;

		connect( this,		SIGNAL( sockInfo(QString) ),
				 m_server,	SLOT(   networkInfo(QString) ) ) ;
	}
}

/*!
 * Demande de connexion au serveur indiqué par son adresse IP ou son nom réseau
 * et par son port de service.
 * \param hostName : identifiant du serveur ou adresse IP.
 * \param port : port de service.
 */

void QamTcpClient::sockConnect(const QString& hostName, quint16 port )
{
	connectToHost(hostName, port ) ;
}

/*!
 * Demande de fermeture de la connexion.
 */

void QamTcpClient::sockClose()
{
	QString hostAddr = peerAddress().toString() ;
	quint16 hostPort = peerPort() ;

	close() ;
	if ( m_server ) m_server->setServerAvailable( false ) ;
	emit sockInfo( QString("connection %1:%2 closed").arg(hostAddr).arg(hostPort) ) ;
}

/*!
 * Demande d'émission à destination du serveur ; sans effet si la connexion
 * n'est pas active.
 * \param data : trame (suite linéaire d'octets) à émettre.
 */

void QamTcpClient::sockWrite(const QByteArray& data )
{
	if ( this->state() != QAbstractSocket::ConnectedState ) return ;
	write( data ) ;
}

// -------------------------------------------------------------------------

void QamTcpClient::sockConnected()
{
	QString hostAddr = peerAddress().toString() ;
	quint16 hostPort = peerPort() ;

	if ( m_server ) m_server->setServerAvailable( true ) ;
	emit sockInfo( QString("connected to %1:%2").arg(hostAddr).arg(hostPort) ) ;
}

void QamTcpClient::sockDisconnected()
{
	if ( m_server ) m_server->setServerAvailable( false ) ;
}

void QamTcpClient::sockRead()
{
	QByteArray  data = this->readAll() ;
	emit sockReceive( data ) ;
}

void QamTcpClient::sockError(QAbstractSocket::SocketError error )
{
	Q_UNUSED( error ) ;
	emit sockInfo( this->errorString() ) ;
}
