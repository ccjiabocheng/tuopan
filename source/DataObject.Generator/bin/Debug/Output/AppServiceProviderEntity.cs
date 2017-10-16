/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("AppServiceProvider")]
	public class AppServiceProviderEntity:ActiveRecordBase
	{
		private long m_Id;
		/// <summary>
		/// 
		/// </summary>
		[PrimaryKey(Generator = PrimaryKeyType.Identity)]
		[JsonProperty("id")]
		public long Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		private string m_ProviderName= string.Empty;
		/// <summary>
		/// 
		/// </summary>
		[Property("ProviderName")]
		[JsonProperty("provider_name")]
		public string ProviderName
		{
			get { return m_ProviderName; }
			set { m_ProviderName = value; }
		}

		private string m_ProviderCode= string.Empty;
		/// <summary>
		/// 
		/// </summary>
		[Property("ProviderCode")]
		[JsonProperty("provider_code")]
		public string ProviderCode
		{
			get { return m_ProviderCode; }
			set { m_ProviderCode = value; }
		}

	}
}
