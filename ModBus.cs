
using UnityEngine;

using ModBus;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// espace de nommage permettant de creer un serveur / ou client Modbus over TCP/IP pour Unity \n
/// La classe ModBusMaster permet de creer un client modbus "threadé"(dérivée de MonoBehavior) \n
/// La classe ModBusSlave permet de creer un serveur modbus "threadé"(dérivée de MonoBehavior) \n
/// \author Copyright A.Facchin <a href = "mailto:alexandre.facchin@ac-creteil.fr"> contact: alexandre Facchin </a> \n
///			          P.Maylaender <a href = "mailto:patrick.maylaender@ac-creteil.fr"> contact: patrick Maylaender </a> \n
/// 
/// IMPORTANT : Pour utiliser la classe serveur ou client il faut respectivement créer un objet de classe ModBusSlave (resp.) ModBusMaster \n
/// exple  pour un client \ref public class NetKartMaster : MonoBehaviour {

///public ModBusData datas ;\n

/// ModBusMaster client;\n

/// The key name vs register R.  Dictionnaire associant nom variable et adresse dans table Modbus \n
 
/// Dictionary<string,UInt32> keyNameVsRegisterRW; // keyName, registre

/// etc...
/// \date 03/18
/// \date 03/19 evolution de la lecture di fichier csv et lecture des variables

/// </summary>
namespace ModBus{

	  
 
	/// <summary>
	/// Mod bus master.
	/// </summary>
	 		
	/// \class ModbusMaster

	/// \author  P.Maylaender
	/// \date 03/18 ; v2 03/19
	/// \brief classe d'exploitation de requetes clientes modbus (mode RTU) sur TCP/IP

	public class ModBusMaster {

		TcpClient client;
		/// <summary>
		/// Le message d'erreur de la couche modbus, accessible par le client uniquement
		/// </summary>
		public string error_message{ get; private set; }
		/// <summary>
		/// The mutex.pour verrou des tables pendant mise à jour
		/// </summary>
		Mutex mutex ;
		FileStream file;  //fichier .csv
		byte[] bytefile = null;   //buffer
		char[] adr_ip; //adr ip serveur 
		string adresse_ip;
		Int32 port ;
		IPAddress remoteAddr;
		IPEndPoint remoteEP  ;
		Byte[] bytesRequete  ; //trame de requete vers le serveur
		Byte[] bytesReponse  ; //trame de reponse du serveur

		/// <summary>
		/// gere les noms des variables modbus.
		/// </summary>
		/// <value>The nom variable modbus.</value>
		public string[] nom_variable_modbus { get; private set; } 
		public string[] adresse_variable_modbus { get; private set; } 
		char [] adresse_memoire; //var temporaire pour creation chaine adresse
		char [] nom_variable;
		/// <summary>
		/// nombre de variables modbus dans csv.
		/// </summary>
		public int nombre_variables_modbus_csv { get; private set; } 

		/// <summary>
		/// gere les adresses des variables table modbus.
		/// </summary>

		public uint[] adresse_variable_table_modbus { get; private set; }

		///cpteur de transaction
		public static int transaction_id { get; private set; } 

		/// <summary>
		/// The datas. Modbus objet agregeant du client
		/// contient en autre le dictionnaire 
		/// </summary>
		public ModBusData datas { get; private set; } 

		//PM 11/3/19
		/// <summary>
		/// Initializes a new instance of the <see cref="ModBus.ModBusMaster"/> class.
		/// </summary>
		/// <param name="datas">objet ModbusData </param>
		/// <param name="file_csv"> nom fichier csv </param>
		public ModBusMaster (ModBusData datas, string file_csv) 
		{
			this.datas = datas;
			mutex = new Mutex();
			//ouverture fichier configuration .csv
			file = File.Open(file_csv,FileMode.Open);

		}   
		/// <summary>
		/// Demarre l'instance client Modbus, attention il faut au besoin adapter le fichier .csv contenant l'adresse du serveur Modbus
		/// </summary>
		public void start(){
			try
			{
				int nbre_point_virgules = 0;
				//alloue buffer emission
				bytesRequete = new Byte[256]; //trame de requete 

				///alloue buffer reception
				bytesReponse = new Byte[256]; //trame de reponse

				///adresse ip du serveur 
				adr_ip = new char[20];
				///port
			 	port = 502;

				nom_variable_modbus = new string[40] ;  //40 variables modbus !! suffisant ? PM  4/3/19
				adresse_variable_modbus = new string[40]; 

				adresse_variable_table_modbus = new uint[40];  // 40 adresses memoires en int !!
				adresse_memoire = new char[10]; //adr memoire en tant que chaine caracteres
				nom_variable = new char[30]; //nom var modbus en tant que chaine caracteres


				Debug.Log("taille .csv" + file.Length);

				// Create a new byte array for the data.
				bytefile = new byte[file.Length];

				// Read the data from the file.
				if(file.Read(bytefile, 0, bytefile.Length) <0)
				{
					Debug.Log("pb lecture fichier .csv");
				}
				// Close the stream.

				file.Seek(0, SeekOrigin.Begin);
				int i = 0;
				while(i < file.Length)
				{
					//if(string.Compare(  Encoding.ASCII.GetString(bytefile +), i,  pattern, 0, 5) == 0 )
					if(bytefile[i] == 'H' && bytefile[i+1] == 'O' && bytefile[i+2] == 'S' && bytefile[i+3] == 'T')
					{
						Debug.Log("adresse ip trouvee");
						break;
					}
					i++;
					//file.Seek(+1, SeekOrigin.Current);   //scrute ds fichier
				}

				//file.Seek(6, SeekOrigin.Begin);//apres le HOST
				i += 5 ; //se place apres le ;
				int j = 0;
				Debug.Log("boucle adr ip ");
				while(i < file.Length &&  bytefile[i] != 0xA)
				{
					adr_ip[j] = (char) bytefile[i++];
					Debug.Log(adr_ip[j]+ " ");
					j++;

				}
				adr_ip[j] = (char ) 0;

				for( i = 0 ; i <= j ; i ++)
					adresse_ip += adr_ip[i]; 
					
				/// recuperation couple variables modbus;adresses
				/// 
			
				file.Seek(0, SeekOrigin.Begin);
			    i = 0;
				int k = 0;   //indice desvariables

				//boucle parcours general fichier csv
			do
			{
				///recherche premiere adresse 400xx  table holding registers
				while(i < file.Length  )
				{
					//if(string.Compare(  Encoding.ASCII.GetString(bytefile +), i,  pattern, 0, 5) == 0 )
						if( bytefile[i] == '4' && bytefile[i+1] == '0' && bytefile[i+2] == '0' )
						{
							 
							{
								nombre_variables_modbus_csv++; //1var de plus
								//Debug.Log("1 zone adresses trouvee");
								nbre_point_virgules = 0;   //RAZ compteur de champs
								break;
							}
						}
				    
					i++;
					 
				}  //remettre si gde boucle bug
				
					if(i >= file.Length) { Debug.Log("fin de fichier csv"); break; }

				j= 0;

				//Debug.Log("boucle adresse memoire des variables ");
				//faire boucle tant que variables  !! PM 4/3/19  TO DO
				while(i < file.Length &&  bytefile[i] != ';')
				{
					adresse_memoire[j] = (char) bytefile[i];
					Debug.Log( adresse_memoire [j]+ "");
						j++; i++;

				}
				adresse_memoire[j] = (char ) 0;
				
				//copie vers une string
				for(int l = 0 ; l <= j ; l ++)
					adresse_variable_modbus[k] += adresse_memoire [l]; 
				
				adresse_variable_table_modbus[k] = uint.Parse(adresse_variable_modbus[k]);
				adresse_variable_table_modbus[k] -= 40001 ; //adresse offset
				Debug.Log("adresse variable " + adresse_variable_table_modbus[k]);


				//recherche debut zone nom variables
				  
				while(i < file.Length  && nbre_point_virgules <= 2)
				{	if(bytefile[i] == ';')
					{
						i++;
						nbre_point_virgules++; //passe au champ suivant
					}
					else i++; //saute le ; 
				}
				
				while(i < file.Length &&  bytefile[i] == ' ') i++; //saute espace debut de nom de variable

				j= 0;
				Debug.Log("boucle nom variables  memoire ");
				//faire boucle tant que variables  !! PM 4/3/19  TO DO
				while(i < file.Length &&  bytefile[i] != ';')
				{
					nom_variable[j] = (char) bytefile[i++];
					Debug.Log( nom_variable [j]+ "");
					j++;
				}
				nom_variable[j] = (char ) 0;

				//copie vers une string
				for(int l = 0 ; l <= j ; l ++)
					nom_variable_modbus[k] += nom_variable [l]; 
				
				Debug.Log("nom variable " + nom_variable_modbus[k]);

				k++; //variable modbus suivante
				//	while(bytefile[i++] != 0xA) ;   //recherche fin de ligne 

				}while(k < 39 && i < file.Length );// 40 variables max  a augmenter si besoin PM 8/3/19 
													//fin boucle traitement lecture fichier csv

				file.Close(); 

				adresse_ip = adresse_ip.Substring(0, adresse_ip.Length - 1);
				Debug.Log("adr ip  serveur "+ adresse_ip);

				remoteAddr = IPAddress.Parse(adresse_ip);   // A remplacer par lecture dans fichier .csv sur unity... PM 28/3/18

					// The first byte is the string length.
					


				//affiche ds conole le fichier 
				Debug.Log(Encoding.ASCII.GetString(bytefile));
				client = new TcpClient();// TcpListener(localAddr, port);

				remoteEP = new IPEndPoint(remoteAddr, port);


				client.SendTimeout = 1000; //1 secondes d'attente

				try{
					Debug.Log("Intenting a connection... ");
					client.Connect (remoteEP);    /* !< connexion au serveur */
					Debug.Log("Server is responding 1 "   );
				}
				catch(SocketException e)
				{
					Debug.LogException( e );
				}
			}
			catch(SocketException e)
			{
				Debug.LogException ( e);
			}

			Debug.Log("Client started..." );
		}
			
	
		 
		/// <summary>
		///  lecture distante d'une seule valeur, de type float (donc 2 registres = 2 mots) sur serveur (automate M340 par exemple) 
		///  Si OK : la valeur est automatiquement mise à jour dans la table(dictionnaire) modbus locale au client 
		///  Si Echec : la valeur initiale du dictionnaire est conservée 
		/// </summary>
		// <returns> int  : 0 : requete OK ; -1 : requete NOK  => place erreur dans le message d'erreur </returns>
		/// <param name="key">UInt32 key : adresse modbus dans le dictionnaire </param>
		 

		public int getRemoteValue(UInt32 key){  

		
		UInt16 addBase = 0xFFFF;
		UInt16 leN = 0;

		//exple de requete send 03 00 12 00 02   adr 12     nombre de mots 2 car float  
		//modbus: FC 3, Address 0012, Number 2
		//modbus: recv 03 04 00 00 42 84 

		//1) construction du MBAP  
		//Transaction id + 1 à chaque requete  voir un static (2 octets)
		//protocol id = 0 (2 octets)
		//longueur PDU   : mettre + 1 p/r taille réelle (d'après les relevés trame wireshark... PM 14/3/18)
		//unit ID = tjs 0 (1 octet)
			mutex.WaitOne ();

			transaction_id++; //1requete de plus
					bytesRequete [0] = 0 ; //PF
			bytesRequete [1] = (byte)transaction_id;
//			Debug.Log ("transaction id" + transaction_id);
			  
					//proto id
			bytesRequete [2] = 0;
			bytesRequete [3] = 0;
					//longueur
			bytesRequete [4] = 0;
			bytesRequete [5] = 6; //tjs 6

			//unit id
			bytesRequete [6] = 255; //tjs 255

		//2) construction du PDU   
		
		//fct 3 !!
		bytesRequete [7] = 0x3;

		//adresse 
		addBase = (UInt16)key;  
	
			bytesRequete [8] = (byte)((addBase & 0xFF00) >> 8);
			bytesRequete [9] = (byte)(addBase & 0xFF) ;
	
		//nombre à lire TJS 2 mots   A modifier 
		bytesRequete [10] = 0; //PF tjs 0
		bytesRequete [11] = 2; //PF tjs 2

			//for (int i = 0; i < 13; i++)
			//	Debug.Log (" trame emise : octet " + i + " --> " + bytesRequete [i]);
			//3) envoi
			mutex.ReleaseMutex();   //fin creation trame => on peut envoyer 
			NetworkStream stream = client.GetStream();

			//Debug.Log ("longueur trame emise " + bytesRequete.Length);
			try{
			stream.Write(this.bytesRequete,0, 12);
			}
			catch(Exception e) {
				Debug.LogException (  e);
				error_message = "echec ecriture sur serveur distant";
				return -1;
			}
				//OK PM 15/3/18
			 

			//TO DO PM 15/3/18 a changer eventuellement en 3 + 2* NB mots à lire
			try{
			stream.Read(this.bytesReponse, 0 , 13);
			}
			catch(Exception e) {
				Debug.LogException (  e);
				error_message = "echec lecture sur serveur distant";
				return -1;
			}
				
			// code erreur Modbus eventuel
			if (((bytesReponse [7] & 0x80) >> 8) == 1) {  //msb positionne => erreur
				error_message = "invalid PDU address or Bad MBAP length";
				return -1;
			}
			else {

				leN = bytesReponse [8 ] ;


				//PM 15/3/18   0 : Table de holding  L'acces a l'enum bloque car ds objet agrege ModbusData
				//force ecriture SI reponse serveur OK , sinon pas de mise à jour ...dans le dictionnaire de la valeur ecrite en distant 
				//=> dico synchronise avec le csv distant du serveur

				//4) recup reponse
				//5) maj table 
				float f;


				if (leN >= 4) {

					//Debug.Log ("valeurs brutes lues");
					for (uint i = 0; i < bytesReponse [8]; i += 4) {// /!\ +=4   //4 octets (32 bits) à chaque fois
						UInt32 futureF = 0;

						//arrangement Little endian semble-t-il PM 14/3/18   
						futureF = (UInt32)(
							(UInt32)(bytesReponse [8 + i + 3] << 24) +
							(UInt32)(bytesReponse [8 + i + 4] << 16) +
							(UInt32)(bytesReponse [8 + i + 1] << 8) +
							(UInt32)(bytesReponse [8 + i + 2])
						);


						//convertit le  uint32 en float avant de le mettre dans le dictionnaire
						f = BitConverter.ToSingle (BitConverter.GetBytes (futureF), 0); 

						//PM 15/3/18   0 : Table de holding   
						//force ecriture ...dans le dictionnaire de la valeur lue en distant => dico synchronise avec le csv distant du serveur
						this.datas.setData (addBase  , 0, f);

					}
				}
				return 1; //ok
			}
		}  //fin methode getRemoteValue


		// requete de ecriture multiple 

		   
		/// <summary>
		/// ecriture distante d'une seule valeur, de type float (donc 2 registres = 2 mots) sur serveur (automate M340 par exemple) \n
		/// Si OK : la valeur est automatiquement mise à jour dans la table(dictionnaire) modbus locale au client \n
		///   Si Echec : la valeur initiale du dictionnaire est conservée \n
		///   L'accès se fait par <nom objet ModbusMaster>.setRemoteValue \n
		/// </summary>
		///
		// <returns> int  : 0 : requete OK ; -1 : requete NOK  => place erreur dans le message d'erreur 
		// </returns>
		/// <param name="key">UInt32 key : adresse modbus dans le dictionnaire </param>
		/// <param name="value">float value : valeur a ecrire </param>

		public int setRemoteValue(UInt32 key, float value){  

 
			UInt16 addBase = 0xFFFF;
			UInt32 value_network_int32;
			NetworkStream stream;


				//convertit les floats en 4 octets chacun de 32bits pour envoyer sur le reseau

				value_network_int32  = BitConverter.ToUInt32 (BitConverter.GetBytes (value),0);
				//Debug.Log (" value en int32 " + value_network_int32[i]);

				//adresse ds dictionnaire
				addBase = (UInt16)key;


			//exple de requete  

			//1) construction du MBAP a faire  
			//Transaction id + 1 à chaque requete  voir un static (2 octets)
			//protocol id = 0 (2 octets)
			//longueur PDU   : mettre + 1 p/r taille réelle (d'après les relevés trame wireshark... PM 14/3/18)
			//unit ID = tjs 0 (1 octet)
			mutex.WaitOne ();

			transaction_id++; //1requete de plus
			bytesRequete [0] = 0; //PF
			bytesRequete [1] = (byte)transaction_id;
			//Debug.Log ("transaction id" + transaction_id);

			//proto id
			bytesRequete [2] = 0;
			bytesRequete [3] = 0;
			//longeur
			bytesRequete [4] = 0;
			bytesRequete [5] = 11; //tjs 11   PDU length  !!   OK PM 16/3/18

			//unit id
			bytesRequete [6] = 255; //tjs 255

			//2) construction du PDU   
			//if (table == TABLE.HOLDINGREGISTER) {
			//	holdingTable [key] = f ;
			//fct 16 !!
			bytesRequete [7] = 0x10;

				
				addBase  = (UInt16)key ;  
				//	Debug.Log ("addresse en 32 " + key);
				//	Debug.Log ("addresse en 16 " + addBase);


				bytesRequete [8  ] = (byte)((addBase & 0xFF00) >> 8);
				bytesRequete [9  ] = (byte)(addBase & 0xFF);
				//	Debug.Log ("adr trame  Modbus" +  bytesRequete [8] + bytesRequete [9]);

				//A tester PM 21/2/19 
				//nombre à ecrire n mots    car 1 float = 4 octets
				bytesRequete [10 ] = 0; //PF tjs 0
				bytesRequete [11 ] = 2; //PF limite à 255 mots !!

				//nombre octets 2* nb de mots    
				bytesRequete [12 ] = 4; //PF nombre d'octets
			 

				// lire les données dans dictionnaire
				// TO DO PM 15/3/18
				//3e octet de la trame de donnée recoit le PF de la data!
				bytesRequete [15 ] = (byte)((value_network_int32 & 0xFF000000) >> 24);  
				//4e octet de la trame de donnée recoit le PmiFort !
				bytesRequete [16 ] = (byte)((value_network_int32 & 0x00FF0000) >> 16);



				//TO SEE PM 15/3/18
				//1er octet de la trame de donnée recoit le Pmifaible !
				bytesRequete [13] = (byte)((value_network_int32 & 0xFF00) >> 8);
				//2e octet de la trame de donnée recoit le Pfaible !
				bytesRequete [14] = (byte)(value_network_int32 & 0x00FF);

		

			mutex.ReleaseMutex();   //fin creation trame => on peut envoyer 

			try{
			stream = client.GetStream();
			

			//for (int i = 0; i < 17; i++)
			//	Debug.Log (" trame emise : octet " + i + " --> " + bytesRequete [i]);
			//Debug.Log ("longueur trame emise " + bytesRequete.Length);
				try{
				stream.Write(this.bytesRequete,0, 17);   // A VERIFIER PM 15/3/18
				}
				catch(Exception e) {
					Debug.LogException (  e);
					error_message = "echec ecriture sur serveur distant";
					return -1;
				}
			}
			catch(Exception e) {
				Debug.LogException (  e);
				error_message = "echec ecriture sur serveur distant";
				return -1;
			}



			//  PM 15/3/18  
			try{
			stream.Read(this.bytesReponse, 0 , 12);   // OK PM 15/3/18
			}
			catch(Exception e) {
				Debug.LogException (  e);
				error_message = "echec ecriture sur serveur distant";
				return -1;
			}

			//	for (int i = 0; i < 12; i++)
			//		Debug.Log (" trame recue : octet " + i + " --> " + bytesReponse [i]);
			 
			//erreur Modbus pure
			if (((bytesReponse [7] & 0x80) >> 8) == 1) {  //msb positionne => erreur
				error_message = "invalid PDU address or Bad MBAP length";
				return -1;
			}			
				else {

				//PM 15/3/18   0 : Table de holding   
				//force ecriture SI reponse serveur OK , sinon pas de mise à jour ...dans le dictionnaire de la valeur ecrite en distant 
				//=> dico synchronise avec le csv distant du serveur

				this.datas.setData (addBase, 0, value);
				return 1;
			}
		}
			
	}    //fin classe client agregee




	/// \class ModbusSlave

	/// \author  A.Facchin
	/// \date 03/18
	/// \brief classe d'exploitation d'un serveur modbus (mode RTU) sur TCP/IP		  


	/// <summary>
	/// Mod bus slave.
	/// </summary>
	public class ModBusSlave {

		Thread thread;
		TcpListener server; 
		public string message;
		Mutex mutex ;

		ModBusData datas ;
		 
		/// <summary>
		/// Initializes a new instance of the <see cref="ModBus.ModBusSlave"/> class.
		/// </summary>
		/// <param name="datas">objet ModbusData</param>
		public ModBusSlave (ModBusData datas) 
		{
			this.datas = datas;
			mutex = new Mutex();

		}   

		public void start(){
			try
			{
				// Set the TcpListener on port 4000.
				//Int32 port = 40000;
				Int32 port = 502;
				IPAddress localAddr = IPAddress.Parse("127.0.0.1");

				// TcpListener server = new TcpListener(port);
				server = new TcpListener(localAddr, port);

				// Start listening for client requests.
				server.Start();

				thread = new Thread (new ThreadStart (this.ThreadAccept));
				thread.Start();

			}
			catch(SocketException e)
			{
				Debug.LogException(  e);
			}

			Debug.Log("Server started..." );
		}

		void ThreadAccept(){// thread Accept
			// Enter the listening loop.
			while(true) 
			{
				Debug.Log("Waiting for a connection... ");

				// Perform a blocking call to accept requests.
				// You could also user server.AcceptSocket() here.
				TcpClient client = server.AcceptTcpClient();    
				// TODO Thread
				message = "client connected";


				// FIXME 1 seule thread semble-t-il...

				Thread localThread = new Thread(() => this.trtClient(client));
				localThread.Start();
				Debug.Log("New Thread..." );


				//				Mutex mutex = new Mutex();
				//				Debug.Log("Mutex create");
				//				mutex.WaitOne ();
				//				Debug.Log("Mutex Get");
				//				mutex.ReleaseMutex();
				//				Debug.Log("Mutex Release");
				//



			}
		}

		void trtClient(TcpClient client){
			Byte[] bytes = new Byte[256];
			Byte[] bytesRep ;

			// mutex


			Debug.Log("Connected! "+client.Client.RemoteEndPoint.ToString());
			message = "Connected! "+client.Client.RemoteEndPoint.ToString()+ "\n";

			// Get a stream object for reading and writing
			NetworkStream stream = client.GetStream();

			int nbRecv;

			// lire en deux fois MBAP FIxe puis le reste avec size 
			// plusieurs trames pour chaques read
			int i = 0 ;
			while ((nbRecv = stream.Read (bytes, 0, 7)) > 0) { // 7 == MBAP LENGHT
				Debug.Log ("taille recv : " + nbRecv);

				int nbMbapRecv = (UInt16)((UInt16)(bytes [4] << 8) + (bytes [5]));
				nbRecv += stream.Read (bytes, 7, nbMbapRecv); 
				//				Debug.Log("Received ");
				Debug.Log ("taille 2 recv : " + nbRecv);

				string asciReq = "";
				for( int j = 0 ; j < nbRecv ; j ++ ){
					asciReq += bytes[j]+".";
				}
				//Debug.Log (asciReq);

				// decode + recode
				mutex.WaitOne ();
				bytesRep = datas.getEncodedResp(bytes, nbRecv);
				mutex.ReleaseMutex();

				// send Resp
				stream.Write(bytesRep,0,datas.getSizeRep());

				string asciReqRep = "";
				for( int j = 0 ; j < datas.getSizeRep() ; j ++ ){
					asciReqRep += bytesRep[j]+".";
				}
				//Debug.Log("Taille émise : " + datas.getSizeRep() ) ;
				//message = "taille question : " + nbRecv + "\n" + asciReq + "\n" + 
				if ((i++ % 30) == 0)
					message = "";
				message += i + ") -- Question : " + nbRecv + "b -- " + asciReq + " -- Trame émise : " + datas.getSizeRep() + "b -- " + asciReqRep + "\n";
				//Debug.Log("Trame émise : " + datas.getSizeRep() + "b -- " + asciReqRep );



			}



			// Shutdown and end connection
			Debug.Log ("Client Closed "+client.Client.RemoteEndPoint.ToString());
			message = "Client Closed " + client.Client.RemoteEndPoint.ToString ();
			client.Close();

		}
	} //fin classe serveur 




	/// <summary>
	/// Mod bus data data modified event handler.
	/// </summary>

	public delegate void ModBusDataDataModifiedEventHandler(object sender, ModBusDataDataModifiedEventArgs e);
	/// <summary>
	/// Mod bus data data modified event arguments.
	/// </summary>
	public class ModBusDataDataModifiedEventArgs : EventArgs
	{
		/// <summary>
		/// datas changed
		/// </summary>
		public UInt32 key { get; private set; }
		public ModBusData.TABLE table { get; private set; }
		public float f { get; private set; }

		public ModBusDataDataModifiedEventArgs(UInt32 key,ModBusData.TABLE table, float f)
		{
			this.key = key;
			this.table = table;
			this.f = f;
		}
	}
	/// <summary>
	/// Mod bus data.
	/// </summary>
	public class ModBusData{

		public enum TABLE { HOLDINGREGISTER , INPUTREGISTER };
		/// <summary>
		/// The holding table: dictionnaire sur le format adresse ; (valeur) float 
		/// Remarque : l'adresse Modbus (exple 40019 => 18 car debut table en 40001 !)
		/// </summary>
		Dictionary<UInt32, float> holdingTable; /// deux registres 16bits (car float sur 4 octets !)
		/// <summary>
		/// The input table.: dictionnaire sur le format adresse ; (valeur) float 
		/// Remarque : l'adresse Modbus (exple 40019 => 18 car debut table en 40001 !)
		/// </summary>
		Dictionary<UInt32, float> inputTable; //idem

		int sizeRep;

		public event ModBusDataDataModifiedEventHandler onDataChanged;
		 

		/// <summary>
		/// Initializes a new instance of the <see cref="ModBus.ModBusData"/> class.
		/// </summary>
		public ModBusData(){
			holdingTable = new Dictionary<UInt32, float> ();
			inputTable = new Dictionary<UInt32, float> ();
		}

		private UInt32 f2uint32(float f){	
			return BitConverter.ToUInt32 (BitConverter.GetBytes( f), 0);
		}
		
		private float uint322f(UInt32 f){
			return BitConverter.ToSingle (BitConverter.GetBytes (f), 0);
		}

		/// <summary>
		/// Adds the def.
		/// </summary>
		/// <param name="key">adresse dans table modbus (dictionnaire) </param>
		/// <param name="f">valeur (float) inscrite dans dictionnaire</param>
		/// <param name="table">type enuméré de table 0: HOLDING ; 1:INPUT</param>
		public void addDef(UInt32 key, float f,TABLE table){
			if (table == TABLE.HOLDINGREGISTER)
				holdingTable.Add (key, f);
			else {
				inputTable.Add (key, f);
			}
		}

		/// <summary>
		/// lit valeur (float) dans la table (dictionnaire) locale
		/// </summary>
		/// <returns>valeur (float) lue dans dictionnaire</returns>
		/// <param name="key">adresse dans table modbus (dictionnaire) </param>
		/// <param name="table">type enuméré de table 0: HOLDING ; 1:INPUT</param>
		public float getData(UInt32 key, TABLE table){
			float ret;
			if (table == TABLE.HOLDINGREGISTER) {
				ret = holdingTable [key];
			} else {
				ret = inputTable [key];
			}
			return ret;
		}

		/// <summary>
		/// ecrit valeur (float) dans la table (dictionnaire) locale
		/// </summary>
		/// <param name="key"> adresse dans table modbus (dictionnaire) </param>
		/// <param name="table"> type enuméré de table 0: HOLDING ; 1:INPUT</param>
		/// <param name="f"></param>
		public void setData(UInt32 key,TABLE table, float f){
			if (table == TABLE.HOLDINGREGISTER) {
				holdingTable [key] = f ;
			} else {
				inputTable [key] = f ;
			}

		}
			
		/// <summary>
		/// Gets the size rep.
		/// </summary>
		/// <returns>The size rep.</returns>
		public int getSizeRep(){
			return sizeRep;
		}

		/// <summary>
		/// Gets the word data.
		/// </summary>
		/// <returns>The word data.</returns>
		/// <param name="addr">Address.</param>
		/// <param name="flagHoldingTable">If set to <c>true</c> flag holding table.</param>
		private UInt16 getWordData(UInt32 addr, bool flagHoldingTable = true){
			float f;
			bool pfort = true;
			if (addr % 2 == 1) {
				addr--;
				pfort = false;
			}

			f = holdingTable[(UInt32)(addr)];


			UInt32 funsafe = f2uint32 (f);

			UInt16 fpMid;
			if(!pfort)
				fpMid = (UInt16)(funsafe >> 16);
			else
				fpMid = (UInt16)(funsafe & 0x0000FFFF);

			return fpMid;

		}
			
		/// <summary>
		/// Gets the encoded resp.
		/// </summary>
		/// <returns>The encoded resp.</returns>
		/// <param name="bytes">Bytes.</param>
		/// <param name="nbRecv">Nb recv.</param>
		public Byte[] getEncodedResp(Byte[] bytes,int nbRecv){
			Byte[] bytesRep = new Byte[256];
			UInt16 number = 0xFFFF;
			UInt16 addBase = 0xFFFF;
			UInt16 leN = 0;
				
			// decode PDU
			if (nbRecv >= 11) {

				// lecture d'un holding register ou d'un inputRegister
				// 0x03 bytes[7] OU 0X04
				if (bytes [7] == 0x03 || bytes [7] == 0x04) {
					//float f;
					// @base bytes[8] -> bytes[9] (uint16)		0 -> 40001
					addBase = (UInt16)((UInt16)(bytes [8] << 8) + (bytes [9]));
					// number bytes[10] -> bytes[11]
					number = (UInt16)((UInt16)(bytes [10] << 8) + (bytes [11]));

					// 03 number*2 data(s)
					for (int i = 0; i < 7; i++) // copie du MBAP
						bytesRep [i] = bytes [i];



					bytesRep [7] = bytes [7];
					bytesRep [8] = (byte)(number * 2);

					// number * 2 + 3 ;
					bytesRep [4] = (byte)((number * 2 + 3)>>8);
					bytesRep [5] = (byte)((number * 2 + 3) & 0x00FF);


					for (uint i = 0; i < number; i += 1) {
						UInt16 fp = getWordData (addBase+i);

				
						// poids fort
						bytesRep [9 + (i) * 2] = (byte)((fp & 0xFF00) >> 8);
						bytesRep [10 + (i) * 2] = (byte)(fp & 0x00FF);

					}

					//						send: 00 04 00 00 00 06 FF  03 00 00 00 02 
					//						recv: 00 04 00 00 00 07 FF  03 04 00 00 41 30
					sizeRep = 9 + (number * 2);
				}// fct 0x03 et 0x04


				//write multiple register
				if (bytes [7] == 0x10) {
					float f;
					// @base bytes[8] -> bytes[9] (uint16)		0 -> 40001
					addBase = (UInt16)((UInt16)(bytes [8] << 8) + (bytes [9]));
					// number bytes[10] -> bytes[11]
					number = (UInt16)((UInt16)(bytes [10] << 8) + (bytes [11]));
					leN = bytes [12];

					sizeRep = 12;

					// 03 number*2 data(s)
					for (int i = 0; i < sizeRep; i++)
						bytesRep [i] = bytes [i];

					// complément MBAP pour db
					bytesRep [4] = (byte)((sizeRep-6) >> 8);
					bytesRep [5] = (byte)((sizeRep-6) & 0x00FF);

					//Debug.Log ("fct : " + bytes [7] + " Registe de depart n° " + addBase + " nb = " + number+", leN "+leN);
					if (leN >= 4) {
						for (uint i = 0; i < leN; i += 4) {// /!\ +=4
							UInt32 futureF = 0;

							futureF = (UInt32)(
								(UInt32)(bytes [13+i + 2 ] << 24) + 
								(UInt32)(bytes [13+i + 3] << 16) + 
								(UInt32)(bytes [13+i + 0] << 8) + 
								(UInt32)(bytes [13+i + 1])
							);

							f = uint322f (futureF);


							setData(addBase+(i/2), TABLE.HOLDINGREGISTER,f);
							if (onDataChanged != null)
								onDataChanged(this, new ModBusDataDataModifiedEventArgs( addBase+(i/2), TABLE.HOLDINGREGISTER, f));
							
						}
					}


				} // fct 0x10

				String asciReq = "";
				for (int j = 0; j < sizeRep; j++) {
					asciReq += bytesRep [j] + "-";
				}

				return bytesRep;
			} else { // trame trop courte
				return null;
			}

		}// getEncodedResp
				
	}// fin classe modbusdata composant
		
}// namespace