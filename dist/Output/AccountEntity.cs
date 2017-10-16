/**** gen by DataObject.Generator ****/
using System;
namespace _3ELS.Data.DataObjects
{
    using Castle.ActiveRecord;
    using Enums;
    using Attributes;

	[ActiveRecord("Account")]
	public class AccountEntity:ActiveRecordBase
	{
		/// <summary>
		/// 
		/// </summary>
		[PrimaryKey(Generator = PrimaryKeyType.Identity)]
		[JsonProperty("id")]
		public long Id { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[Property("LoginName")]
		[JsonProperty("login_name")]
		public string LoginName { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[Property("LoginPwd")]
		[JsonProperty("login_pwd")]
		public string LoginPwd { set; get; }

		/// <summary>
		/// 帐号状态，0禁用 1启用
		/// </summary>
		[Property("State")]
		[JsonProperty("state")]
		public int State { set; get; }

		/// <summary>
		/// 注册来源
		/// </summary>
		[Property("Source")]
		[JsonProperty("source")]
		public string Source { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[Property("CreateTime")]
		[JsonProperty("create_time")]
		public DateTime CreateTime { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[Property("LastUpdateTime")]
		[JsonProperty("last_update_time")]
		public DateTime LastUpdateTime { set; get; }

	}
}
