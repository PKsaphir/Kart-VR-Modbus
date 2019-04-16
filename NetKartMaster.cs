
using UnityEngine;
using System;

/// <summary>
// todo 1) insertion espace de nommage ModBus
/// </summary>
using ModBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


 
/// <summary>
/// Classe applicative client Modbus pour le Kart
/// \author : P.Maylaender  (projet 2018)
/// 
/// </summary>

public class NetKartMaster : MonoBehaviour {

	/// <summary>
	// todo 2) Déclaration objet datas de type ModbusData
	/// </summary>
	ModBusData datas;


	/// <summary>
	// todo 3) Declarer  un objet nommé client de type ModBusMaster
	/// </summary>
	ModBusMaster client;    

	/// <summary>
	/// The key name vs register R.  Dictionnaire associant nom variable et adresse dans table Modbus
	/// </summary>
	Dictionary<string,UInt32> keyNameVsRegisterRW; // keyName, registre

	//permet acces aux noms variables du fichier csv
	enum NOM_VAR_CSV { local_distant, on_off_run , on_off_enable , vit_simu , couple_simu, masse_pilote, temps_parcours, cons_pente, courant_batt1, puissance_totale_batt, charge_elect_totale, tension_batt1, tension_batt2, tension_batt3,tension_batt4} ;
	 

	void Start () 
	{


		//tables MODBUS

		datas = new ModBusData();

		//      instanciation + demarrage CLIENT modbusmaster 
		// Pensez à ADAPTER le nom et contenu de fichier et enumeration AU dessus !
		client = new  ModBusMaster(datas,"kart_banc_2019.csv");


		///demarrage client
		client.start();


		//VERIF PM 8/3/19

		// instancier le dictionnaire  keyNameVsRegisterRW 
		keyNameVsRegisterRW = new Dictionary<string, uint> (); 


		//  ajout des references : variables du CSV et adresses PAIRES associées   exple masse_pilote en 18
		for(int l = 0; l < client.nombre_variables_modbus_csv; l ++)
			keyNameVsRegisterRW.Add(client.nom_variable_modbus[l],client.adresse_variable_table_modbus [l]);
		
		//pour test debug acces nom de variable
		Debug.Log("adresse 1ere variable modbus" + client.adresse_variable_table_modbus [(int)NOM_VAR_CSV.local_distant]);

		var enumerator = keyNameVsRegisterRW.GetEnumerator();
		while (enumerator.MoveNext())
		{
			///\todo decommenter une fois datas declarée et instancier !!
			datas.addDef (enumerator.Current.Value,0,ModBusData.TABLE.HOLDINGREGISTER);
			Debug.Log ("Add Def : " + enumerator.Current.Key + " " + enumerator.Current.Value);
		}

		// creation de gestionnaire de signal de modif des valeurs dans les dictionnaires
		datas.onDataChanged += new ModBusDataDataModifiedEventHandler(netMaster_OnDataModified);


	}   


   

	/** RAJOUT PM 28/3/18  DEMARRE , ARRET du variateur Parvex lié au Moteur MCC du banc */

	//START and STOP du variateur !! PM 23/3/18
	/// <summary>
	/// Gets the local start stop.dans le dictionnaire local unity
	/// </summary>
	/// <returns>The local start stop.</returns>
	public float getLocalStartStop(){

		//  lecture dans dictionnaire Modbusdata avec getData , 0 : table HOLDING
		return this.datas.getData (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_run]], 0);
	}

	/// <summary>
	/// Gets the remote StartStop.La valeur n'est pas directement retournée, elle est mise dans le dictionnaire local, 
	/// et donc il faut appeler ensuite dans le script "utilisateur" , la methode getLocalStartStop() qui elle ; 
	/// lit dans le dictionnaire et retourne la valeur lue à distance.
	/// </summary>
	/// <returns>0 : OK ; -1 : erreur </returns>
	public int getRemoteStartStop(){

		// appel methode lecture distante de l'objet client (argument adresse du dictionnaire liée au nom variable) 
		if (client.getRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_run]]) == -1) {
			Debug.Log (client.error_message);
			return -1;
		}
			else {
			return 0; //Succes
		}
	}

	//Enable du variateur !! PM 23/3/18
	/// <summary>
	/// Gets the local enable. dans le dictionnaire local
	/// </summary>
	/// <returns> 0 (Enable) non valide ; 1 valide => variateur pret au demarrage</returns>
	public float getLocalEnable(){

		//  lecture dans dictionnaire Modbusdata avec getData , 0 : table HOLDING
		return this.datas.getData (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_enable]], 0);
	}
	/// <summary>
	/// Gets the remote Enable status.La valeur n'est pas directement retournée, elle est mise dans le dictionnaire local, 
	/// et donc il faut appeler ensuite dans le script "utilisateur" , la methode getLocalEnable() qui elle ; 
	/// lit dans le dictionnaire et retourne la valeur lue à distance.
	/// </summary>
	/// <returns>0 : OK ; -1 : erreur </returns>
	public int getRemoteEnable(){

		// appel methode lecture distante de l'objet client (argument adresse du dictionnaire liée au nom variable) 
		if (client.getRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_enable]]) == -1) {
			Debug.Log (client.error_message); //affiche erreur Modbus 
			return -1;
		}
		else {
			return 0; //Succes
		}

	}
	/// <summary>
	/// Sets the remote start stop. Ecrit le Enable et Run en meme temps !!!sur le serveur
	/// </summary>
	/// <returns>0 : OK ; -1 : erreur</returns>
	/// <param name="value32">0.0f <=> devalide ; 1.0f <=> valide </param>
	public int setRemoteStartStop(float value32){

		//force le mode distant car sinon le variateur demarre pas
		//ecrit 1 (pc snir )   dans local_distant
		//if (client.setRemoteValue (keyNameVsRegisterRW ["local_dist"], 1.0f) == -1) {
		if (client.setRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.local_distant]], 1.0f) == -1) {
			Debug.Log (client.error_message);//affiche erreur Modbus 
			return -1;
		}
		else{
			Debug.Log ("Mode distant ok ");
			//ecrit 1 (demarre ) ; 0 (arrete) dans on_off_enable
			//if (client.setRemoteValue (keyNameVsRegisterRW ["on_off_enable"], value32) == -1) {
			if (client.setRemoteValue (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.local_distant]  ], value32) == -1) {
					
				Debug.Log (client.error_message);//affiche erreur Modbus 
				return -1;
			} else {
				Debug.Log ("Enable ok ");
				//ecrit 1 (demarre ) ; 0 (arrete) dans on_off_run
				//if (client.setRemoteValue (keyNameVsRegisterRW ["on_off_run"], value32) == -1) {
				if (client.setRemoteValue (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_run ]], value32) == -1) {
				Debug.Log (client.error_message);//affiche erreur Modbus 
					return -1;
				} else {
					Debug.Log ("Run ok ");
					return 0; //Succes
				}
			}
		}
	}
	/// <summary>
	/// Sets the local start stop. Ecrit le Enable et Run en meme temps !!!dans le dictionnaire local unity
	/// </summary>
	/// <param name="value"> 0.0f <=> devalide ; 1.0f <=> valide </param>
	public void setLocalStartStop( float value){

		// ecriture dans dictionnaire ModbusData avec setData  ; 0 : table HOLDING

			this.datas.setData (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_enable ]], 0 , value);
			this.datas.setData (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_run ]], 0 , value);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float getLocalMassePilote(){

		//  lecture dans dictionnaire Modbusdata avec getData , 0 : table HOLDING
		return this.datas.getData (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.masse_pilote]], 0);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public int getRemoteMassePilote(){

		// appel methode lecture distante de l'objet client (argument adresse du dictionnaire liée au nom variable) 
		if (client.getRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.masse_pilote]]) == -1) {
			Debug.Log (client.error_message);
			return -1;
		}
			else {
			return 0; //Succes
		}
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"> 0.0f <=> devalide ; 1.0f <=> valide </param>
	public void setLocalMassePilote(float value){

		// ecriture dans dictionnaire ModbusData avec setData  ; 0 : table HOLDING

			this.datas.setData (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_enable ]], 0 , value);
			this.datas.setData (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.masse_pilote ]], 0 , value);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	/// <param name="value32"></param>
	public int setRemoteMassePilote(float value32){

		//force le mode distant car sinon le variateur demarre pas
		//ecrit 1 (pc snir )   dans local_distant
		//if (client.setRemoteValue (keyNameVsRegisterRW ["local_dist"], 1.0f) == -1) {
		if (client.setRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.local_distant]], 1.0f) == -1) {
			Debug.Log (client.error_message);//affiche erreur Modbus 
			return -1;
		}
		else{
			Debug.Log ("Mode distant ok ");
			//ecrit 1 (demarre ) ; 0 (arrete) dans on_off_enable
			//if (client.setRemoteValue (keyNameVsRegisterRW ["on_off_enable"], value32) == -1) {
			if (client.setRemoteValue (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.local_distant]  ], value32) == -1) {
					
				Debug.Log (client.error_message);//affiche erreur Modbus 
				return -1;
			} else {
				Debug.Log ("Enable ok ");
				//ecrit 1 (demarre ) ; 0 (arrete) dans on_off_run
				//if (client.setRemoteValue (keyNameVsRegisterRW ["on_off_run"], value32) == -1) {
				if (client.setRemoteValue (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.masse_pilote ]], value32) == -1) {
				Debug.Log (client.error_message);//affiche erreur Modbus 
					return -1;
				} else {
					Debug.Log ("Run ok ");
					return 0; //Succes
				}
			}
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float getLocalCouple(){

		//  lecture dans dictionnaire Modbusdata avec getData , 0 : table HOLDING
		return this.datas.getData (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.couple_simu]], 0);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public int getRemoteCouple(){

		// appel methode lecture distante de l'objet client (argument adresse du dictionnaire liée au nom variable) 
		if (client.getRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.couple_simu]]) == -1) {
			Debug.Log (client.error_message);
			return -1;
		}
			else {
			return 0; //Succes
		}
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"> 0.0f <=> devalide ; 1.0f <=> valide </param>
	public void setLocalCouple(float value){

		// ecriture dans dictionnaire ModbusData avec setData  ; 0 : table HOLDING

			this.datas.setData (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_enable ]], 0 , value);
			this.datas.setData (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.couple_simu ]], 0 , value);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	/// <param name="value32"></param>
	public int setRemoteCouple(float value32){

		//force le mode distant car sinon le variateur demarre pas
		//ecrit 1 (pc snir )   dans local_distant
		//if (client.setRemoteValue (keyNameVsRegisterRW ["local_dist"], 1.0f) == -1) {
		if (client.setRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.local_distant]], 1.0f) == -1) {
			Debug.Log (client.error_message);//affiche erreur Modbus 
			return -1;
		}
		else{
			Debug.Log ("Mode distant ok ");
			//ecrit 1 (demarre ) ; 0 (arrete) dans on_off_enable
			//if (client.setRemoteValue (keyNameVsRegisterRW ["on_off_enable"], value32) == -1) {
			if (client.setRemoteValue (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.local_distant]  ], value32) == -1) {
					
				Debug.Log (client.error_message);//affiche erreur Modbus 
				return -1;
			} else {
				Debug.Log ("Enable ok ");
				//ecrit 1 (demarre ) ; 0 (arrete) dans on_off_run
				//if (client.setRemoteValue (keyNameVsRegisterRW ["on_off_run"], value32) == -1) {
				if (client.setRemoteValue (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.couple_simu ]], value32) == -1) {
				Debug.Log (client.error_message);//affiche erreur Modbus 
					return -1;
				} else {
					Debug.Log ("Run ok ");
					return 0; //Succes
				}
			}
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float getLocalConsignePente(){

		//  lecture dans dictionnaire Modbusdata avec getData , 0 : table HOLDING
		return this.datas.getData (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.cons_pente]], 0);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public int getRemoteConsignePente(){

		// appel methode lecture distante de l'objet client (argument adresse du dictionnaire liée au nom variable) 
		if (client.getRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.cons_pente]]) == -1) {
			Debug.Log (client.error_message);
			return -1;
		}
			else {
			return 0; //Succes
		}
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"> 0.0f <=> devalide ; 1.0f <=> valide </param>
	public void setLocalConsignePente(float value){

		// ecriture dans dictionnaire ModbusData avec setData  ; 0 : table HOLDING

			this.datas.setData (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.on_off_enable ]], 0 , value);
			this.datas.setData (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.cons_pente ]], 0 , value);
	}
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	/// <param name="value32"></param>
	public int setRemoteConsignePente(float value32){

		//force le mode distant car sinon le variateur demarre pas
		//ecrit 1 (pc snir )   dans local_distant
		//if (client.setRemoteValue (keyNameVsRegisterRW ["local_dist"], 1.0f) == -1) {
		if (client.setRemoteValue (keyNameVsRegisterRW [ client.nom_variable_modbus[(int)NOM_VAR_CSV.local_distant]], 1.0f) == -1) {
			Debug.Log (client.error_message);//affiche erreur Modbus 
			return -1;
		}
		else{
			Debug.Log ("Mode distant ok ");
			//ecrit 1 (demarre ) ; 0 (arrete) dans on_off_enable
			//if (client.setRemoteValue (keyNameVsRegisterRW ["on_off_enable"], value32) == -1) {
			if (client.setRemoteValue (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.local_distant]  ], value32) == -1) {
					
				Debug.Log (client.error_message);//affiche erreur Modbus 
				return -1;
			} else {
				Debug.Log ("Enable ok ");
				//ecrit 1 (demarre ) ; 0 (arrete) dans on_off_run
				//if (client.setRemoteValue (keyNameVsRegisterRW ["on_off_run"], value32) == -1) {
				if (client.setRemoteValue (keyNameVsRegisterRW [client.nom_variable_modbus[(int)NOM_VAR_CSV.cons_pente ]], value32) == -1) {
				Debug.Log (client.error_message);//affiche erreur Modbus 
					return -1;
				} else {
					Debug.Log ("Run ok ");
					return 0; //Succes
				}
			}
		}
	}


		
	//pour mettre à jour les val par signal
	void netMaster_OnDataModified(object sender, ModBusDataDataModifiedEventArgs ea){
		float f = ea.f ;
		UInt32 key = ea.key ;
		ModBusData.TABLE table = ea.table ;
		//Debug.Log ("Datas Modified " + f + " " + key + " " + table );
	}
}
