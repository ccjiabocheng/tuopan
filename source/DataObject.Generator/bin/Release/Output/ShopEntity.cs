/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("Shop")]
	public class ShopEntity:ActiveRecordBase
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

		private long m_MchId;
		/// <summary>
		/// 商户ID
		/// </summary>
		[Property("MchId")]
		[JsonProperty("mch_id")]
		public long MchId
		{
			get { return m_MchId; }
			set { m_MchId = value; }
		}

		private string m_MchNo= string.Empty;
		/// <summary>
		/// 所属商户编号
		/// </summary>
		[Property("MchNo")]
		[JsonProperty("mch_no")]
		public string MchNo
		{
			get { return m_MchNo; }
			set { m_MchNo = value; }
		}

		private string m_ShopNo= string.Empty;
		/// <summary>
		/// 场地编号
		/// </summary>
		[Property("ShopNo")]
		[JsonProperty("shop_no")]
		public string ShopNo
		{
			get { return m_ShopNo; }
			set { m_ShopNo = value; }
		}

		private string m_ShopName= string.Empty;
		/// <summary>
		/// 场地名称
		/// </summary>
		[Property("ShopName")]
		[JsonProperty("shop_name")]
		public string ShopName
		{
			get { return m_ShopName; }
			set { m_ShopName = value; }
		}

		private string m_Address= string.Empty;
		/// <summary>
		/// 场地地址
		/// </summary>
		[Property("Address")]
		[JsonProperty("address")]
		public string Address
		{
			get { return m_Address; }
			set { m_Address = value; }
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

		private long m_CreatorId;
		/// <summary>
		/// 
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
		/// 
		/// </summary>
		[Property("CreatorName")]
		[JsonProperty("creator_name")]
		public string CreatorName
		{
			get { return m_CreatorName; }
			set { m_CreatorName = value; }
		}

	}
}
