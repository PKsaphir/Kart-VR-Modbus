/****************************************************************************
** Meta object code from reading C++ file 'qammodbusmap.h'
**
** Created by: The Qt Meta Object Compiler version 67 (Qt 5.12.0)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "qammodbusmap.h"
#include <QtCore/qbytearray.h>
#include <QtCore/qmetatype.h>
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'qammodbusmap.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 67
#error "This file was generated using the moc from 5.12.0. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
QT_WARNING_PUSH
QT_WARNING_DISABLE_DEPRECATED
struct qt_meta_stringdata_QamModbusMap_t {
    QByteArrayData data[16];
    char stringdata0[156];
};
#define QT_MOC_LITERAL(idx, ofs, len) \
    Q_STATIC_BYTE_ARRAY_DATA_HEADER_INITIALIZER_WITH_OFFSET(len, \
    qptrdiff(offsetof(qt_meta_stringdata_QamModbusMap_t, stringdata0) + ofs \
        - idx * sizeof(QByteArrayData)) \
    )
static const qt_meta_stringdata_QamModbusMap_t qt_meta_stringdata_QamModbusMap = {
    {
QT_MOC_LITERAL(0, 0, 12), // "QamModbusMap"
QT_MOC_LITERAL(1, 13, 12), // "responseDone"
QT_MOC_LITERAL(2, 26, 0), // ""
QT_MOC_LITERAL(3, 27, 12), // "valueChanged"
QT_MOC_LITERAL(4, 40, 5), // "table"
QT_MOC_LITERAL(5, 46, 4), // "name"
QT_MOC_LITERAL(6, 51, 4), // "info"
QT_MOC_LITERAL(7, 56, 6), // "source"
QT_MOC_LITERAL(8, 63, 7), // "message"
QT_MOC_LITERAL(9, 71, 18), // "responseFromServer"
QT_MOC_LITERAL(10, 90, 8), // "response"
QT_MOC_LITERAL(11, 99, 8), // "setValue"
QT_MOC_LITERAL(12, 108, 12), // "PrimaryTable"
QT_MOC_LITERAL(13, 121, 5), // "value"
QT_MOC_LITERAL(14, 127, 13), // "setLocalValue"
QT_MOC_LITERAL(15, 141, 14) // "setRemoteValue"

    },
    "QamModbusMap\0responseDone\0\0valueChanged\0"
    "table\0name\0info\0source\0message\0"
    "responseFromServer\0response\0setValue\0"
    "PrimaryTable\0value\0setLocalValue\0"
    "setRemoteValue"
};
#undef QT_MOC_LITERAL

static const uint qt_meta_data_QamModbusMap[] = {

 // content:
       8,       // revision
       0,       // classname
       0,    0, // classinfo
       7,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       3,       // signalCount

 // signals: name, argc, parameters, tag, flags
       1,    0,   49,    2, 0x06 /* Public */,
       3,    2,   50,    2, 0x06 /* Public */,
       6,    2,   55,    2, 0x06 /* Public */,

 // slots: name, argc, parameters, tag, flags
       9,    1,   60,    2, 0x0a /* Public */,
      11,    3,   63,    2, 0x0a /* Public */,
      14,    3,   70,    2, 0x0a /* Public */,
      15,    3,   77,    2, 0x0a /* Public */,

 // signals: parameters
    QMetaType::Void,
    QMetaType::Void, QMetaType::Int, QMetaType::QString,    4,    5,
    QMetaType::Void, QMetaType::QString, QMetaType::QString,    7,    8,

 // slots: parameters
    QMetaType::Void, QMetaType::QByteArray,   10,
    QMetaType::Void, 0x80000000 | 12, QMetaType::QString, QMetaType::QString,    4,    5,   13,
    QMetaType::Void, 0x80000000 | 12, QMetaType::QString, QMetaType::QString,    4,    5,   13,
    QMetaType::Void, 0x80000000 | 12, QMetaType::QString, QMetaType::QString,    4,    5,   13,

       0        // eod
};

void QamModbusMap::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        QamModbusMap *_t = static_cast<QamModbusMap *>(_o);
        Q_UNUSED(_t)
        switch (_id) {
        case 0: _t->responseDone(); break;
        case 1: _t->valueChanged((*reinterpret_cast< int(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2]))); break;
        case 2: _t->info((*reinterpret_cast< const QString(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2]))); break;
        case 3: _t->responseFromServer((*reinterpret_cast< QByteArray(*)>(_a[1]))); break;
        case 4: _t->setValue((*reinterpret_cast< PrimaryTable(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2])),(*reinterpret_cast< const QString(*)>(_a[3]))); break;
        case 5: _t->setLocalValue((*reinterpret_cast< PrimaryTable(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2])),(*reinterpret_cast< const QString(*)>(_a[3]))); break;
        case 6: _t->setRemoteValue((*reinterpret_cast< PrimaryTable(*)>(_a[1])),(*reinterpret_cast< const QString(*)>(_a[2])),(*reinterpret_cast< const QString(*)>(_a[3]))); break;
        default: ;
        }
    } else if (_c == QMetaObject::IndexOfMethod) {
        int *result = reinterpret_cast<int *>(_a[0]);
        {
            using _t = void (QamModbusMap::*)();
            if (*reinterpret_cast<_t *>(_a[1]) == static_cast<_t>(&QamModbusMap::responseDone)) {
                *result = 0;
                return;
            }
        }
        {
            using _t = void (QamModbusMap::*)(int , const QString & );
            if (*reinterpret_cast<_t *>(_a[1]) == static_cast<_t>(&QamModbusMap::valueChanged)) {
                *result = 1;
                return;
            }
        }
        {
            using _t = void (QamModbusMap::*)(const QString & , const QString & );
            if (*reinterpret_cast<_t *>(_a[1]) == static_cast<_t>(&QamModbusMap::info)) {
                *result = 2;
                return;
            }
        }
    }
}

QT_INIT_METAOBJECT const QMetaObject QamModbusMap::staticMetaObject = { {
    &QamAbstractServer::staticMetaObject,
    qt_meta_stringdata_QamModbusMap.data,
    qt_meta_data_QamModbusMap,
    qt_static_metacall,
    nullptr,
    nullptr
} };


const QMetaObject *QamModbusMap::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *QamModbusMap::qt_metacast(const char *_clname)
{
    if (!_clname) return nullptr;
    if (!strcmp(_clname, qt_meta_stringdata_QamModbusMap.stringdata0))
        return static_cast<void*>(this);
    return QamAbstractServer::qt_metacast(_clname);
}

int QamModbusMap::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QamAbstractServer::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 7)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 7;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 7)
            *reinterpret_cast<int*>(_a[0]) = -1;
        _id -= 7;
    }
    return _id;
}

// SIGNAL 0
void QamModbusMap::responseDone()
{
    QMetaObject::activate(this, &staticMetaObject, 0, nullptr);
}

// SIGNAL 1
void QamModbusMap::valueChanged(int _t1, const QString & _t2)
{
    void *_a[] = { nullptr, const_cast<void*>(reinterpret_cast<const void*>(&_t1)), const_cast<void*>(reinterpret_cast<const void*>(&_t2)) };
    QMetaObject::activate(this, &staticMetaObject, 1, _a);
}

// SIGNAL 2
void QamModbusMap::info(const QString & _t1, const QString & _t2)
{
    void *_a[] = { nullptr, const_cast<void*>(reinterpret_cast<const void*>(&_t1)), const_cast<void*>(reinterpret_cast<const void*>(&_t2)) };
    QMetaObject::activate(this, &staticMetaObject, 2, _a);
}
QT_WARNING_POP
QT_END_MOC_NAMESPACE
