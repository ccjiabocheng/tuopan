/**** gen by DataObject.Generator ****/
using System;
namespace _3ELS.Data.DataObjects
{
    using Castle.ActiveRecord;
    using Enums;
    using Attributes;

	[ActiveRecord("WeimobInformation")]
	public class WeimobInformationEntity:ActiveRecordBase
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
		[Property("AppId")]
		[JsonProperty("app_id")]
		public string AppId { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[Property("AID")]
		[JsonProperty("a_i_d")]
		public string AID { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[Property("ClientId")]
		[JsonProperty("client_id")]
		public string ClientId { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[Property("ClientSecret")]
		[JsonProperty("client_secret")]
		public string ClientSecret { set; get; }

		/// <summary>
		/// 
		/// </summary>
		[Property("LastUpdateTime")]
		[JsonProperty("last_update_time")]
		public DateTime LastUpdateTime { set; get; }

	}
}
