/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("ShopApp")]
	public class ShopAppEntity:ActiveRecordBase
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

		private long m_ShopId;
		/// <summary>
		/// 场地门店ID
		/// </summary>
		[Property("ShopId")]
		[JsonProperty("shop_id")]
		public long ShopId
		{
			get { return m_ShopId; }
			set { m_ShopId = value; }
		}

		private int m_AppKindId;
		/// <summary>
		/// 应用种类ID
		/// </summary>
		[Property("AppKindId")]
		[JsonProperty("app_kind_id")]
		public int AppKindId
		{
			get { return m_AppKindId; }
			set { m_AppKindId = value; }
		}

		private long m_AppServiceProviderId;
		/// <summary>
		/// 应用的服务提供商ID
		/// </summary>
		[Property("AppServiceProviderId")]
		[JsonProperty("app_service_provider_id")]
		public long AppServiceProviderId
		{
			get { return m_AppServiceProviderId; }
			set { m_AppServiceProviderId = value; }
		}

		private string m_AppName= string.Empty;
		/// <summary>
		/// 应用名称
		/// </summary>
		[Property("AppName")]
		[JsonProperty("app_name")]
		public string AppName
		{
			get { return m_AppName; }
			set { m_AppName = value; }
		}

		private DateTime m_CreateTime;
		/// <summary>
		/// 应用创建时间
		/// </summary>
		[Property("CreateTime")]
		[JsonProperty("create_time")]
		public DateTime CreateTime
		{
			get { return m_CreateTime; }
			set { m_CreateTime = value; }
		}

		private DateTime m_LastUpdateTime;
		/// <summary>
		/// 最后更新时间
		/// </summary>
		[Property("LastUpdateTime")]
		[JsonProperty("last_update_time")]
		public DateTime LastUpdateTime
		{
			get { return m_LastUpdateTime; }
			set { m_LastUpdateTime = value; }
		}

		private long m_CreatorId;
		/// <summary>
		/// 创建人ID
		/// </summary>
		[Property("CreatorId")]
		[JsonProperty("creator_id")]
		public long CreatorId
		{
			get { return m_CreatorId; }
			set { m_CreatorId = value; }
		}

		private string m_CreatorName= string.Empty;
		/// <summary>
		/// 创建人姓名
		/// </summary>
		[Property("CreatorName")]
		[JsonProperty("creator_name")]
		public string CreatorName
		{
			get { return m_CreatorName; }
			set { m_CreatorName = value; }
		}

		private int m_State;
		/// <summary>
		/// 应用状态， 0停用  1启用
		/// </summary>
		[Property("State")]
		[JsonProperty("state")]
		public int State
		{
			get { return m_State; }
			set { m_State = value; }
		}

	}
}
