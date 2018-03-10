using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RPC
{
	internal class Caller
	{
		public string method { get; set; }
		public ParamsUsedToReplace paramsUsedToReplace_b5a073d1194edc73c75c0c1c0e20e3f74150b6e1 { get; set; }
	}

	internal class ParamsUsedToReplace
	{
		public string uri { get; set; }
		public string languageId { get; set; }
		public int version { get; set; }
		public string text { get; set; }
	}

	/// <summary>
	/// 提供了获取待POST的JSON数据的方法
	/// </summary>
	public static class Wrapper
	{
		/// <summary>
		/// 封装了序列化JSON的操作
		/// </summary>
		/// <param name="method">要调用的RPC方法名，请参考http://127.0.0.1:3000/help</param>
		/// <param name="uri">待操作的文件uri</param>
		/// <param name="languageId">文本的语言格式，如“json”等</param>
		/// <param name="version">请参考http://127.0.0.1:3000/help</param>
		/// <param name="text">TBD，目前版本不要使用这个参数，请保持其默认值</param>
		/// <returns>返回RPC操作所需的JSON数据</returns>
		public static string GetCallerJSON(string method, string uri, string languageId, int version, string text = "")
		{
			Caller caller = new Caller();
			caller.method = method;
			caller.paramsUsedToReplace_b5a073d1194edc73c75c0c1c0e20e3f74150b6e1 = new ParamsUsedToReplace();
			caller.paramsUsedToReplace_b5a073d1194edc73c75c0c1c0e20e3f74150b6e1.uri = uri;
			caller.paramsUsedToReplace_b5a073d1194edc73c75c0c1c0e20e3f74150b6e1.languageId = languageId;
			caller.paramsUsedToReplace_b5a073d1194edc73c75c0c1c0e20e3f74150b6e1.version = version;
			caller.paramsUsedToReplace_b5a073d1194edc73c75c0c1c0e20e3f74150b6e1.text = string.Empty;
			StringBuilder dest = new StringBuilder(JsonConvert.SerializeObject(caller));
			dest.Insert(0, "rpc=");
			
			/* 解释一下这里的操作：											 * 
			 * 因为RPC框架中有一项属性名为params，但这与C#中关键字重名		 *
			 * 所以不能直接将C#对象的属性设置为params，迂回一下				 *
			 * 将该属性设置为一个极小概率与文本信息重复的特殊名字，替换之	 */
			Regex regex = new Regex("paramsUsedToReplace_b5a073d1194edc73c75c0c1c0e20e3f74150b6e1");
			return regex.Replace(dest.ToString(), "params");
		}
	}
	internal class SuccessObject
	{
		public string jsonrpc { get; set; }
		public string result { get; set; }
	}
	internal class FailInfo
	{
		public string code { get; set; }
		public string message { get; set; }
	}
	internal class ErrorObject
	{
		public string jsonrpc { get; set; }
		public FailInfo error { get; set; }
	}
}

namespace TestJSONRPC
{
	class Program
	{
		/// <summary>
		/// 入口点，这里演示了同步的方法进行POST，异步同理，略
		/// </summary>
		static void Main(string[] args)
		{
			string filepath = Path.Combine(Environment.CurrentDirectory, "users.json");
			// 获取文件URI
			UriBuilder ub = new UriBuilder("file", "loopback", -1, filepath);
			// 获取待POST的JSON数据
			string json = RPC.Wrapper.GetCallerJSON("FormatDocument", ub.Uri.ToString(), "json", 2);
			
			// 创建HTTP请求，并设置header信息
			HttpWebRequest httpWebRequest = WebRequest.Create("http://127.0.0.1:3000") as HttpWebRequest;
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			byte[] payload = Encoding.GetEncoding("UTF-8").GetBytes(json);
			httpWebRequest.ContentLength = payload.Length;
			// 写至流中
			Stream writer = httpWebRequest.GetRequestStream();
			writer.Write(payload, 0, payload.Length);
			writer.Close();
			
			// 获取对应的回应
			HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
			Stream reader = httpWebResponse.GetResponseStream();
			// 以流的方式按行读入
			StreamReader streamReader = new StreamReader(reader, Encoding.GetEncoding("UTF-8"));
			StringBuilder stringBuilder = new StringBuilder();
			string templine = string.Empty;
			while ((templine = streamReader.ReadLine()) != null)
			{
				stringBuilder.AppendLine(templine);
			}
			string result = stringBuilder.ToString();
			Console.WriteLine(result);

			// 反序列化为对象以利用数据
			if (result[18] == 'r')
			{
				RPC.SuccessObject obj = JsonConvert.DeserializeObject<RPC.SuccessObject>(result) as RPC.SuccessObject;
				// obj.result即为返回的文本（这个例子是格式化）
			} 
			else
			{
				RPC.ErrorObject obj = JsonConvert.DeserializeObject<RPC.ErrorObject>(result) as RPC.ErrorObject;
				// obj.error.code为错误编码
				// obj.error.message为错误信息
			}
		}
	}
}
