/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("AppServiceConfig")]
	public class AppServiceConfigEntity:ActiveRecordBase
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

		private long m_ProviderId;
		/// <summary>
		/// 所属应用服务提供商ID
		/// </summary>
		[Property("ProviderId")]
		[JsonProperty("provider_id")]
		public long ProviderId
		{
			get { return m_ProviderId; }
			set { m_ProviderId = value; }
		}

		private string m_ProviderCode= string.Empty;
		/// <summary>
		/// 服务提供商的编码
		/// </summary>
		[Property("ProviderCode")]
		[JsonProperty("provider_code")]
		public string ProviderCode
		{
			get { return m_ProviderCode; }
			set { m_ProviderCode = value; }
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
		/// 服务配置项值类型  1 = 字符串  2 = 数值  3 = 网址
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

		private string m_DefaultValue= string.Empty;
		/// <summary>
		/// 配置项默认值
		/// </summary>
		[Property("DefaultValue")]
		[JsonProperty("default_value")]
		public string DefaultValue
		{
			get { return m_DefaultValue; }
			set { m_DefaultValue = value; }
		}

		private byte m_Required;
		/// <summary>
		/// 是否为必须项,0否 1是 ，默认1
		/// </summary>
		[Property("Required")]
		[JsonProperty("required")]
		public byte Required
		{
			get { return m_Required; }
			set { m_Required = value; }
		}

		private byte m_Visible;
		/// <summary>
		/// 配置项是否可见，0 不可见 1可见。 默认1
		/// </summary>
		[Property("Visible")]
		[JsonProperty("visible")]
		public byte Visible
		{
			get { return m_Visible; }
			set { m_Visible = value; }
		}

	}
}
