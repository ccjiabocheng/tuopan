/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("BusinessOrder")]
	public class BusinessOrderEntity:ActiveRecordBase
	{
		private string m_OrderNo= string.Empty;
		/// <summary>
		/// 系统订单号
		/// </summary>
		[PrimaryKey(Generator = PrimaryKeyType.Assigned)]
		[JsonProperty("order_no")]
		public string OrderNo
		{
			get { return m_OrderNo; }
			set { m_OrderNo = value; }
		}

		private string m_AppOrderNo= string.Empty;
		/// <summary>
		/// 应用订单号
		/// </summary>
		[Property("AppOrderNo")]
		[JsonProperty("app_order_no")]
		public string AppOrderNo
		{
			get { return m_AppOrderNo; }
			set { m_AppOrderNo = value; }
		}

		private string m_ServiceOrderNo= string.Empty;
		/// <summary>
		/// 服务方订单号
		/// </summary>
		[Property("ServiceOrderNo")]
		[JsonProperty("service_order_no")]
		public string ServiceOrderNo
		{
			get { return m_ServiceOrderNo; }
			set { m_ServiceOrderNo = value; }
		}

		private long m_ShopId;
		/// <summary>
		/// 
		/// </summary>
		[Property("ShopId")]
		[JsonProperty("shop_id")]
		public long ShopId
		{
			get { return m_ShopId; }
			set { m_ShopId = value; }
		}

		private string m_Creator= string.Empty;
		/// <summary>
		/// 订单创建人
		/// </summary>
		[Property("Creator")]
		[JsonProperty("creator")]
		public string Creator
		{
			get { return m_Creator; }
			set { m_Creator = value; }
		}

		private DateTime m_CreateTime;
		/// <summary>
		/// 创建时间
		/// </summary>
		[Property("CreateTime")]
		[JsonProperty("create_time")]
		public DateTime CreateTime
		{
			get { return m_CreateTime; }
			set { m_CreateTime = value; }
		}

		private DateTime m_BusinessTime;
		/// <summary>
		/// 业务完成时间
		/// </summary>
		[Property("BusinessTime")]
		[JsonProperty("business_time")]
		public DateTime BusinessTime
		{
			get { return m_BusinessTime; }
			set { m_BusinessTime = value; }
		}

		private string m_Productld= string.Empty;
		/// <summary>
		/// 产品ID
		/// </summary>
		[Property("Productld")]
		[JsonProperty("productld")]
		public string Productld
		{
			get { return m_Productld; }
			set { m_Productld = value; }
		}

		private string m_Body= string.Empty;
		/// <summary>
		/// 订单关联商品的描述信息
		/// </summary>
		[Property("Body")]
		[JsonProperty("body")]
		public string Body
		{
			get { return m_Body; }
			set { m_Body = value; }
		}

		private string m_Attach= string.Empty;
		/// <summary>
		/// 订单附属信息
		/// </summary>
		[Property("Attach")]
		[JsonProperty("attach")]
		public string Attach
		{
			get { return m_Attach; }
			set { m_Attach = value; }
		}

		private int m_Coins;
		/// <summary>
		/// 币值
		/// </summary>
		[Property("Coins")]
		[JsonProperty("coins")]
		public int Coins
		{
			get { return m_Coins; }
			set { m_Coins = value; }
		}

		private int m_ACoins;
		/// <summary>
		/// A币值
		/// </summary>
		[Property("ACoins")]
		[JsonProperty("a_coins")]
		public int ACoins
		{
			get { return m_ACoins; }
			set { m_ACoins = value; }
		}

		private decimal m_Cash;
		/// <summary>
		/// 现金，单位元
		/// </summary>
		[Property("Cash")]
		[JsonProperty("cash")]
		public decimal Cash
		{
			get { return m_Cash; }
			set { m_Cash = value; }
		}

		private int m_Lotteries;
		/// <summary>
		/// 票值
		/// </summary>
		[Property("Lotteries")]
		[JsonProperty("lotteries")]
		public int Lotteries
		{
			get { return m_Lotteries; }
			set { m_Lotteries = value; }
		}

		private int m_Points;
		/// <summary>
		/// 积分
		/// </summary>
		[Property("Points")]
		[JsonProperty("points")]
		public int Points
		{
			get { return m_Points; }
			set { m_Points = value; }
		}

		private decimal m_OtherCurrency;
		/// <summary>
		/// 其他币类型
		/// </summary>
		[Property("OtherCurrency")]
		[JsonProperty("other_currency")]
		public decimal OtherCurrency
		{
			get { return m_OtherCurrency; }
			set { m_OtherCurrency = value; }
		}

		private int m_State;
		/// <summary>
		/// 订单状态，0 = 预下单成功 1= 已支付 2= 已发货   200=已退款  255=订单关闭
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
