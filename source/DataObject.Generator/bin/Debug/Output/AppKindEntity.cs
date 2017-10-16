/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("AppKind")]
	public class AppKindEntity:ActiveRecordBase
	{
		private long m_Id;
		/// <summary>
		/// 
		/// </summary>
		[Property("Id")]
		[JsonProperty("id")]
		public long Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		private string m_KindName= string.Empty;
		/// <summary>
		/// 应用类型名称
		/// </summary>
		[Property("KindName")]
		[JsonProperty("kind_name")]
		public string KindName
		{
			get { return m_KindName; }
			set { m_KindName = value; }
		}

	}
}
