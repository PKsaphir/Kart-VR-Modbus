/* ----------------------------------------------------------------------------
 |	Nom :			qamabstractserver.h
 |	Projet :		Qam Modbus over TCP/IP
 |	Sujet :     	Classe abstraite "serveur TCP"
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

#ifndef QAMABSTRACTSERVER_H
#define QAMABSTRACTSERVER_H

/*!
  @file
  @brief Classe modèle "client/serveur métier TCP"
 */

/*!
 @class QamAbstractServer
 @brief Classe modèle "client/serveur métier TCP".

La classe QamAbstractServer sert de modèle de développement
pour l'interlocuteur des classes QamTcpServer et QamTcpConnection lors
de l'implémentation d'un serveur. Elle doit aussi être utilisée comme
modèle pour QamTcpClient lors de la conception d'une application maître.
 */

#include <QObject>
#include <QByteArray>

class QamAbstractServer : public QObject
{
	Q_OBJECT

  public:
	explicit QamAbstractServer(QObject* parent = 0 ) ;

	/*! Mode client : indique une connexion active ou non. */
	virtual void setServerAvailable(bool serverAvailable ) ;

	/*! Méthode de réponse à une requête client. */
	virtual QByteArray responseToRequest(QByteArray& request ) /*= 0*/ ;

  signals:
	/*! Signal de demande d'émission d'une requête. */
	void request(QByteArray request ) ;

  public slots:
	/*! Connecteur de remontée de messages d'erreurs ou d'information. */
	virtual void networkInfo(const QString& message ) /*= 0*/ ;
	/*! Connecteur de traitement d'une trame entrante. */
	virtual void response(QByteArray response ) ;
} ;

#endif // QAMABSTRACTSERVER_H
