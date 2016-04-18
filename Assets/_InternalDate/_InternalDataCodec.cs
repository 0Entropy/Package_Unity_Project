using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum TextureSuffix{JPG, PNG}

public class _InternalDataCodec{

	string sdCardPath = "/mnt/sdcard/Android/data";

//	public static void MoveFile(string from, string to){
//		try{
//			File.Move(from, to);
//		}
//		catch(SerializationException e){
//			throw;
//		}
//	}

	public static byte[] ObjectToByteArray(GameObject obj){
		try
		{
			BinaryFormatter bf = new BinaryFormatter ();
			using(MemoryStream ms = new MemoryStream()){
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
			
		}
		catch(SerializationException e)
		{
			Debug.Log (e.Message);
			throw;
		}
	}

	public static void SaveObject(string path, GameObject obj){
		SaveBytes(path, ObjectToByteArray(obj));
	}
	

	#region String

	public static void SaveString(String file, string localPath){
		try
		{
			using (StreamWriter outfile = new StreamWriter(localPath))
			{
				outfile.Write(file);
			}
			
		}
		catch(SerializationException e)
		{
			Debug.Log (e.Message);
			throw;
		}
	}

	public static String LoadString(string localPath){
		if (File.Exists (localPath)) {
			
			try
			{
				using (StreamReader sr = File.OpenText(localPath))
				{
					return sr.ReadToEnd();
				}
				
			}
			catch(SerializationException e)
			{
				Debug.Log (e.Message);
				//				throw;
				return null;
			}
			
		}else{
			Debug.LogError("Load : That "+ localPath +" Do Not Exists !");
			return null;
		}
	}

	#endregion

	#region Serializable Data
	public static void SaveSerializableData<T>(T data, string localPath) where T : class{

		try
		{

			using(FileStream file = File.OpenWrite (localPath)){
				BinaryFormatter bf = new BinaryFormatter ();
				bf.Serialize (file, data);
			}

		}
		catch(SerializationException e)
		{
			Debug.Log (e.Message);
			throw;
		}
	}

	public static T LoadSerializableData<T>(string localPath) where T : class{

		if (File.Exists (localPath)) {

			try
			{
				using(FileStream file = File.OpenRead (localPath)){
					BinaryFormatter bf = new BinaryFormatter ();
					T data = (T)bf.Deserialize (file);
					return data;
				}

			}
			catch(SerializationException e)
			{
				Debug.Log (e.Message);
//				throw;
				return null;
			}

		}else{
			//THIS APP MUST BE PLAYED AT THE FIRST TIME!!!
			Debug.LogError("Load : That "+ localPath +" Do Not Exists !");
			return null;
		}
	}
	#endregion

	#region Texture
	public static Texture2D LoadTexture (string path)
	{
		Texture2D tex = new Texture2D (1, 1, TextureFormat.ARGB32, false);
		Byte[] bytes = LoadBytes(path);
		tex.LoadImage (bytes);
		tex.Apply ();
		
		return tex;
	}



	public static void SaveTexture (string path, Texture2D tex, bool isPNGFormat)
	{
		byte[] bytes = isPNGFormat ? tex.EncodeToPNG () : tex.EncodeToJPG();
		SaveBytes(path, bytes);
	}
	#endregion

	#region Bytes
	public static void SaveBytes(string path, Byte[] bytes){
		try {
			using (FileStream file = File.OpenWrite(path)) {
//				Debug.Log ("Bytes length ---->" + bytes.Length);
				file.Write (bytes, 0, bytes.Length);
			}
		} catch (SerializationException e) {
			Debug.Log (e.Message);
			throw;
		}
	}

	public static byte[] LoadBytes(string path){
		try {
			using (FileStream file = File.OpenRead(path)) {
				byte[] bytes = new byte[file.Length];
				
				int numBytesToRead = (int)file.Length;

				int numBytesReaded = 0;
				//如果文件过大，需要 while loop 。。。
				while (numBytesToRead > 0) {
					int n = file.Read (bytes, numBytesReaded, numBytesToRead);
					if (n == 0)
						break;

					//Debug.Log ("file buffer size ---> " + n);
					
					numBytesReaded += n;
					numBytesToRead -= n;
					
				}
				
				numBytesToRead = bytes.Length;
				
				return bytes;
			}
		} catch (SerializationException e) {
			Debug.Log (e.Message);
			throw;
		}
	}

	public static void TransferBytes(string fromPath, string toPath){
		SaveBytes(toPath, LoadBytes(fromPath));
	}
	#endregion

	#region Delete Data
	public static void DeleteLocalData(string localPath){
		if(File.Exists(localPath)){
			try {
				if ((File.GetAttributes(localPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					File.SetAttributes(localPath, File.GetAttributes(localPath) ^ FileAttributes.ReadOnly);
				}

				File.Delete(localPath);
								
			} 
			catch (FileLoadException e)
			{
				Debug.Log (e.Message);
				throw;
			}
		}
	}
	#endregion

//	void Start(){
//
////		LoadJosnTest();
////		StartCoroutine("LoadJosnTest", "http://blog.paultondeur.com/files/2010/UnityExternalJSONXML/books.json");
//	}

}

//public class WWWPipeline{
//
//	public List<WWWProxy> proxies;// = new List<WWWProxy>();
//
//	public WWWPipeline(){
//		proxies = new List<WWWProxy>();
//	}
//
//	public IEnumerator WaitForFinished(){
////		yield return null;
//		foreach(WWWProxy proxy in proxies){
//			yield return proxy.WaitForFinished();
//		}
//	}
//}

