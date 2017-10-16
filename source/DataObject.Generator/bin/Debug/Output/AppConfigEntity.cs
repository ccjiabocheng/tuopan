/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("AppConfig")]
	public class AppConfigEntity:ActiveRecordBase
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

		private long m_AppKindId;
		/// <summary>
		/// 应用类型Id
		/// </summary>
		[Property("AppKindId")]
		[JsonProperty("app_kind_id")]
		public long AppKindId
		{
			get { return m_AppKindId; }
			set { m_AppKindId = value; }
		}

		private string m_ConfigCode= string.Empty;
		/// <summary>
		/// 应用配置项机器代码
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
		/// 应用配置项的友好名称
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
		/// 应用配置项值类型1 = 字符串2 = 数值3 = 网址
		/// </summary>
		[Property("ValueType")]
		[JsonProperty("value_type")]
		public int ValueType
		{
			get { return m_ValueType; }
			set { m_ValueType = value; }
		}

		private int m_ValueMaxLength;
		/// <summary>
		/// 配置项的值最大长度
		/// </summary>
		[Property("ValueMaxLength")]
		[JsonProperty("value_max_length")]
		public int ValueMaxLength
		{
			get { return m_ValueMaxLength; }
			set { m_ValueMaxLength = value; }
		}

		private int m_Required;
		/// <summary>
		/// 是否为必须项
		/// </summary>
		[Property("Required")]
		[JsonProperty("required")]
		public int Required
		{
			get { return m_Required; }
			set { m_Required = value; }
		}

	}
}
