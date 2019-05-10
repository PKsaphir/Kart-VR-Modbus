/* ----------------------------------------------------------------------------
 |	Nom :			qamabstractserver.cpp
 |	Projet :		Qam Modbus over TCP/IP
 |	Sujet :     	Classe abstraite "serveur TCP"
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

#include "qamabstractserver.h"

/*! Constructeur. */

QamAbstractServer::QamAbstractServer(QObject* parent)
	: QObject(parent)
{
}

void QamAbstractServer::setServerAvailable(bool serverAvailable )
{
	Q_UNUSED( serverAvailable ) ;
}

QByteArray QamAbstractServer::responseToRequest(QByteArray& request )
{
	return request ;
}

void QamAbstractServer::networkInfo(const QString& message )
{
	Q_UNUSED( message ) ;
}

void QamAbstractServer::response(QByteArray response )
{
	Q_UNUSED( response ) ;
}
