/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("Account")]
	public class AccountEntity:ActiveRecordBase
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

		private string m_LoginName= string.Empty;
		/// <summary>
		/// 
		/// </summary>
		[Property("LoginName")]
		[JsonProperty("login_name")]
		public string LoginName
		{
			get { return m_LoginName; }
			set { m_LoginName = value; }
		}

		private string m_LoginPwd= string.Empty;
		/// <summary>
		/// 
		/// </summary>
		[Property("LoginPwd")]
		[JsonProperty("login_pwd")]
		public string LoginPwd
		{
			get { return m_LoginPwd; }
			set { m_LoginPwd = value; }
		}

		private string m_AliasName= string.Empty;
		/// <summary>
		/// 昵称
		/// </summary>
		[Property("AliasName")]
		[JsonProperty("alias_name")]
		public string AliasName
		{
			get { return m_AliasName; }
			set { m_AliasName = value; }
		}

		private DateTime m_CreateTime;
		/// <summary>
		/// 
		/// </summary>
		[Property("CreateTime")]
		[JsonProperty("create_time")]
		public DateTime CreateTime
		{
			get { return m_CreateTime; }
			set { m_CreateTime = value; }
		}

	}
}
