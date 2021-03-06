
QT += core widgets network
QT -= gui

CONFIG += c++11 console
CONFIG += app_bundle
TEMPLATE = app
TARGET = kartbtclient


# The following define makes your compiler emit warnings if you use
# any feature of Qt which as been marked deprecated (the exact warnings
# depend on your compiler). Please consult the documentation of the
# deprecated API in order to know how to port your code away from it.
DEFINES += QT_DEPRECATED_WARNINGS

# You can also make your code fail to compile if you use deprecated APIs.
# In order to do so, uncomment the following line.
# You can also select to disable deprecated APIs only up to a certain version of Qt.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

SOURCES += \
        main.cpp \
    kartbtclient.cpp \
    qammodbusdata.cpp \
    qammodbusmap.cpp \
    qammodbusmapviewer.cpp \
    qamtcpclient.cpp \
    qamabstractserver.cpp \
    qamtcpserver.cpp \
    qamtcpconnection.cpp

HEADERS += \
    kartbtclient.h \
    qammodbusdata.h \
    qamtcpclient.h \
    qammodbusmap.h \
    qammodbusmapviewer.h \
    qamabstractserver.h \
    qamtcpserver.h \
    qamtcpconnection.h
