/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("ShopAppServiceConfig")]
	public class ShopAppServiceConfigEntity:ActiveRecordBase
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

		private long m_ShopAppId;
		/// <summary>
		/// 场地应用ID
		/// </summary>
		[Property("ShopAppId")]
		[JsonProperty("shop_app_id")]
		public long ShopAppId
		{
			get { return m_ShopAppId; }
			set { m_ShopAppId = value; }
		}

		private string m_ConfigCode= string.Empty;
		/// <summary>
		/// 服务配置项机器代码
		/// </summary>
		[Property("ConfigCode")]
		[JsonProperty("config_code")]
		public string ConfigCode
		{
			get { return m_ConfigCode; }
			set { m_ConfigCode = value; }
		}

		private string m_ConfigName= string.Empty;
		/// <summary>
		/// 服务配置项的友好名称
		/// </summary>
		[Property("ConfigName")]
		[JsonProperty("config_name")]
		public string ConfigName
		{
			get { return m_ConfigName; }
			set { m_ConfigName = value; }
		}

		private int m_ValueType;
		/// <summary>
		/// 服务配置项值类型1 = 字符串2 = 数值3 = 网址
		/// </summary>
		[Property("ValueType")]
		[JsonProperty("value_type")]
		public int ValueType
		{
			get { return m_ValueType; }
			set { m_ValueType = value; }
		}

		private string m_Value= string.Empty;
		/// <summary>
		/// 配置项的值，最大长度不超过256
		/// </summary>
		[Property("Value")]
		[JsonProperty("value")]
		public string Value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}

	}
}
