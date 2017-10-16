/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("WeimobInformation")]
	public class WeimobInformationEntity:ActiveRecordBase
	{
		/// <summary>
		/// 
		/// </summary>
		[PrimaryKey(Generator = PrimaryKeyType.Identity)]
		[JsonProperty("id")]
		private long m_Id;
		public long Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Property("AppId")]
		[JsonProperty("app_id")]
		private string m_AppId;
		public string AppId
		{
			get { return m_AppId; }
			set { m_AppId = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Property("AID")]
		[JsonProperty("a_i_d")]
		private string m_AID;
		public string AID
		{
			get { return m_AID; }
			set { m_AID = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Property("ClientId")]
		[JsonProperty("client_id")]
		private string m_ClientId;
		public string ClientId
		{
			get { return m_ClientId; }
			set { m_ClientId = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Property("ClientSecret")]
		[JsonProperty("client_secret")]
		private string m_ClientSecret;
		public string ClientSecret
		{
			get { return m_ClientSecret; }
			set { m_ClientSecret = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		[Property("LastUpdateTime")]
		[JsonProperty("last_update_time")]
		private DateTime m_LastUpdateTime;
		public DateTime LastUpdateTime
		{
			get { return m_LastUpdateTime; }
			set { m_LastUpdateTime = value; }
		}

	}
}
