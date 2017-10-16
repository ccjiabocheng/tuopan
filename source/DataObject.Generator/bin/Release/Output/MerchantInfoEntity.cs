/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("MerchantInfo")]
	public class MerchantInfoEntity:ActiveRecordBase
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

		private string m_MchNo= string.Empty;
		/// <summary>
		/// 商户编号
		/// </summary>
		[Property("MchNo")]
		[JsonProperty("mch_no")]
		public string MchNo
		{
			get { return m_MchNo; }
			set { m_MchNo = value; }
		}

		private string m_MchName= string.Empty;
		/// <summary>
		/// 商户名称
		/// </summary>
		[Property("MchName")]
		[JsonProperty("mch_name")]
		public string MchName
		{
			get { return m_MchName; }
			set { m_MchName = value; }
		}

		private string m_Address= string.Empty;
		/// <summary>
		/// 
		/// </summary>
		[Property("Address")]
		[JsonProperty("address")]
		public string Address
		{
			get { return m_Address; }
			set { m_Address = value; }
		}

		private string m_BusinessLicenseNo= string.Empty;
		/// <summary>
		/// 工商许可证编号/三证合一编号
		/// </summary>
		[Property("BusinessLicenseNo")]
		[JsonProperty("business_license_no")]
		public string BusinessLicenseNo
		{
			get { return m_BusinessLicenseNo; }
			set { m_BusinessLicenseNo = value; }
		}

		private string m_Contact= string.Empty;
		/// <summary>
		/// 联系人姓名
		/// </summary>
		[Property("Contact")]
		[JsonProperty("contact")]
		public string Contact
		{
			get { return m_Contact; }
			set { m_Contact = value; }
		}

		private string m_ContactTel= string.Empty;
		/// <summary>
		/// 联系人电话
		/// </summary>
		[Property("ContactTel")]
		[JsonProperty("contact_tel")]
		public string ContactTel
		{
			get { return m_ContactTel; }
			set { m_ContactTel = value; }
		}

		private string m_ContactMail= string.Empty;
		/// <summary>
		/// 联系人电邮
		/// </summary>
		[Property("ContactMail")]
		[JsonProperty("contact_mail")]
		public string ContactMail
		{
			get { return m_ContactMail; }
			set { m_ContactMail = value; }
		}

		private string m_ExpiredDate= string.Empty;
		/// <summary>
		/// 权益过期时间
		/// </summary>
		[Property("ExpiredDate")]
		[JsonProperty("expired_date")]
		public string ExpiredDate
		{
			get { return m_ExpiredDate; }
			set { m_ExpiredDate = value; }
		}

		private string m_Remarks= string.Empty;
		/// <summary>
		/// 备注
		/// </summary>
		[Property("Remarks")]
		[JsonProperty("remarks")]
		public string Remarks
		{
			get { return m_Remarks; }
			set { m_Remarks = value; }
		}

	}
}
