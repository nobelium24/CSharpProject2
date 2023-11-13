using System;
using System.Collections.Generic;


namespace ECommerceApp.Models
{
    public class VerifyPaymentModel
    {
        public int? TotalAmount { get; set; }
        public string? Reference { get; set; }
    }



    public class Log
    {
        public int start_time { get; set; }
        public int time_spent { get; set; }
        public int attempts { get; set; }
        public int errors { get; set; }
        public bool success { get; set; }
        public bool mobile { get; set; }
        public List<object>? input { get; set; }
        public List<History>? history { get; set; }
    }

    public class History
    {
        public string? type { get; set; }
        public string? message { get; set; }
        public int time { get; set; }
    }

    public class FeesSplit
    {
        public int paystack { get; set; }
        public int integration { get; set; }
        public int subaccount { get; set; }
        public Params? @params { get; set; }
    }

    public class Params
    {
        public string? bearer { get; set; }
        public string? transaction_charge { get; set; }
        public string? percentage_charge { get; set; }
    }

    public class Authorization
    {
        public string? authorization_code { get; set; }
        public string? bin { get; set; }
        public string? last4 { get; set; }
        public string? exp_month { get; set; }
        public string? exp_year { get; set; }
        public string? channel { get; set; }
        public string? card_type { get; set; }
        public string? bank { get; set; }
        public string? country_code { get; set; }
        public string? brand { get; set; }
        public bool reusable { get; set; }
        public string? signature { get; set; }
        public object? account_name { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public object? first_name { get; set; }
        public object? last_name { get; set; }
        public string? email { get; set; }
        public string? customer_code { get; set; }
        public object? phone { get; set; }
        public object? metadata { get; set; }
        public string? risk_action { get; set; }
    }

    public class Subaccount
    {
        public int id { get; set; }
        public string? subaccount_code { get; set; }
        public string? business_name { get; set; }
        public string? description { get; set; }
        public object? primary_contact_name { get; set; }
        public object? primary_contact_email { get; set; }
        public object? primary_contact_phone { get; set; }
        public object? metadata { get; set; }
        public double percentage_charge { get; set; }
        public string? settlement_bank { get; set; }
        public string? account_number { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string? domain { get; set; }
        public string? status { get; set; }
        public string? reference { get; set; }
        public int amount { get; set; }
        public object? message { get; set; }
        public string? gateway_response { get; set; }
        public DateTime paid_at { get; set; }
        public DateTime created_at { get; set; }
        public string? channel { get; set; }
        public string? currency { get; set; }
        public string? ip_address { get; set; }
        public string? metadata { get; set; }
        public Log? log { get; set; }
        public int fees { get; set; }
        public FeesSplit? fees_split { get; set; }
        public Authorization? authorization { get; set; }
        public Customer? customer { get; set; }
        public object? plan { get; set; }
        public object? order_id { get; set; }
        public DateTime paidAt { get; set; }
        public DateTime createdAt { get; set; }
        public int requested_amount { get; set; }
        public DateTime transaction_date { get; set; }
        public object? plan_object { get; set; }
        public Subaccount? subaccount { get; set; }
    }

    public class Root
    {
        public bool status { get; set; }
        public string? message { get; set; }
        public Data? data { get; set; }
    }

}