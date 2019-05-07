/* ----------------------------------------------------------------------------
 |	Nom :			qamtcpclient.h
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

#ifndef QAMTCPCLIENT_H
#define QAMTCPCLIENT_H

/*!
  @file
  @brief Client TCP
 */

/*!
 @class QamTcpClient
 @brief Client TCP.

 La classe QamTcpClient permet la mise en oeuvre d'un client TCP/IP.
 La logique applicative d'envoi de requêtes et de traitement des réponses peut
 être assurée par un objet répondant au modèle de développement spécifié par la
 classe QamAbstractServer. Dans ce cas, le client TCP met en oeuvre les
 connexions signaux-slots nécessaires à partir des membres spécifiés par la
 classe modèle QamAbstractServer :
 - signal request() : demande d'émisssion de données ;
 - slot response() : connecteur de réception de données ;
 - slot networkInfo() : connecteur destinataire des messages de suivi ou d'erreur.

 QamTcpClient peut aussi être utilisée sans objet dérivé de QamAbstractServer.
 L'application cliente doit alors intercepter les signaux sockReceive() et
 sockInfo() pour la réception de données et la remontée d'informations. L'émission
 est réalisée au moyen de sockWrite().

 Dans les deux situations, les méthodes sockConnect() et sockClose() permettent
 respectivement le démarrage et l'arrêt de la connexion.
 */

#include <QTcpSocket>
#include "qamabstractserver.h"

class QamTcpClient : public QTcpSocket
{
	Q_OBJECT

  public:
    explicit QamTcpClient(QamAbstractServer* server = nullptr, QObject* parent = nullptr ) ;

	void sockConnect(const QString& hostName, quint16 port ) ;

  public slots :
	void sockClose() ;
	void sockWrite(const QByteArray& data ) ;

  signals :
	/*! Signal émis lors de la réception de données sur la socket TCP. */
	void sockReceive(const QByteArray& ) ;
	/*! Signal de remontée d'informations liées à l'état de la connexion. */
	void sockInfo(const QString& ) ;

  private slots :
	void sockRead() ;
	void sockError(QAbstractSocket::SocketError error ) ;
	void sockConnected() ;
	void sockDisconnected() ;

  private:
	QamAbstractServer*	m_server ;
} ;

#endif // QAMTCPCLIENT_H
