/**** gen by DataObject.Generator ****/
using System;
namespace HaHaZhuan.OAuthServer.Entities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Castle.ActiveRecord;

	[ActiveRecord("WeiXinRefundOrder")]
	public class WeiXinRefundOrderEntity:ActiveRecordBase
	{
		private string m_OrderNo= string.Empty;
		/// <summary>
		/// 退款订单号
		/// </summary>
		[PrimaryKey(Generator = PrimaryKeyType.Assigned)]
		[JsonProperty("order_no")]
		public string OrderNo
		{
			get { return m_OrderNo; }
			set { m_OrderNo = value; }
		}

		private string m_MerchantId= string.Empty;
		/// <summary>
		/// 商户编号
		/// </summary>
		[Property("MerchantId")]
		[JsonProperty("merchant_id")]
		public string MerchantId
		{
			get { return m_MerchantId; }
			set { m_MerchantId = value; }
		}

		private string m_TransOrderNo= string.Empty;
		/// <summary>
		/// 原交易订单号
		/// </summary>
		[Property("TransOrderNo")]
		[JsonProperty("trans_order_no")]
		public string TransOrderNo
		{
			get { return m_TransOrderNo; }
			set { m_TransOrderNo = value; }
		}

		private string m_OutRefundOrderNo= string.Empty;
		/// <summary>
		/// 商户退款订单号
		/// </summary>
		[Property("OutRefundOrderNo")]
		[JsonProperty("out_refund_order_no")]
		public string OutRefundOrderNo
		{
			get { return m_OutRefundOrderNo; }
			set { m_OutRefundOrderNo = value; }
		}

		private string m_RefundId= string.Empty;
		/// <summary>
		/// 微信退款单号
		/// </summary>
		[Property("RefundId")]
		[JsonProperty("refund_id")]
		public string RefundId
		{
			get { return m_RefundId; }
			set { m_RefundId = value; }
		}

		private decimal m_TotalFee;
		/// <summary>
		/// 原始订单总金额，单位元
		/// </summary>
		[Property("TotalFee")]
		[JsonProperty("total_fee")]
		public decimal TotalFee
		{
			get { return m_TotalFee; }
			set { m_TotalFee = value; }
		}

		private decimal m_Fee;
		/// <summary>
		/// 退款金额，单位元
		/// </summary>
		[Property("Fee")]
		[JsonProperty("fee")]
		public decimal Fee
		{
			get { return m_Fee; }
			set { m_Fee = value; }
		}

		private long m_CreateTime;
		/// <summary>
		/// 订单创建时间的时间戳
		/// </summary>
		[Property("CreateTime")]
		[JsonProperty("create_time")]
		public long CreateTime
		{
			get { return m_CreateTime; }
			set { m_CreateTime = value; }
		}

		private long m_LastUpdateTime;
		/// <summary>
		/// 订单最后更新时间戳
		/// </summary>
		[Property("LastUpdateTime")]
		[JsonProperty("last_update_time")]
		public long LastUpdateTime
		{
			get { return m_LastUpdateTime; }
			set { m_LastUpdateTime = value; }
		}

		private string m_OpUserId= string.Empty;
		/// <summary>
		/// 操作员ID
		/// </summary>
		[Property("OpUserId")]
		[JsonProperty("op_user_id")]
		public string OpUserId
		{
			get { return m_OpUserId; }
			set { m_OpUserId = value; }
		}

		private int m_State;
		/// <summary>
		/// 0 退款单创建  1 转入退款  2 退款成功
		/// </summary>
		[Property("State")]
		[JsonProperty("state")]
		public int State
		{
			get { return m_State; }
			set { m_State = value; }
		}

		private string m_StateDesc= string.Empty;
		/// <summary>
		/// 退款单状态描述
		/// </summary>
		[Property("StateDesc")]
		[JsonProperty("state_desc")]
		public string StateDesc
		{
			get { return m_StateDesc; }
			set { m_StateDesc = value; }
		}

	}
}
