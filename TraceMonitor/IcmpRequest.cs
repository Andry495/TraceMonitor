using System;
using System.Net;
using System.Threading;
using Org.Mentalis.Network;
using TraceMonitor;

namespace ICMPRequestServer
{
	/// <summary>
	/// Summary description for icmpRequest.
	/// </summary>
	public class IcmpRequest
	{
		public IcmpRequest(ToolsCommandRequest myRequest)
		{

			//The request will be a ping
			if (myRequest.myCommandType == CommandType.ping)
			{
				Icmp myIcmp = new Icmp(Dns.Resolve(myRequest.host).AddressList[0]);
                listbox1.items.add("Pinging " + myRequest.host + " [" + Dns.Resolve(myRequest.host).AddressList[0].ToString() + "] with " + myRequest.weightPacket.ToString() + " bytes of data:");
				//Console.WriteLine("Pinging " + myRequest.host + " [" + Dns.Resolve(myRequest.host).AddressList[0].ToString()+ "] with " + myRequest.weightPacket.ToString() + " bytes of data:");
				//Console.WriteLine();

				while(myRequest.nbrEcho > 0) 
				{
					try 
					{
						PingRequestInformation myResult = myIcmp.Ping(myRequest.timeout,myRequest.timeToLiveMax, myRequest.weightPacket);
						if (myResult.duration.Equals(TimeSpan.MaxValue)) Console.WriteLine("Request timed out.");
						else
						{
							Console.WriteLine("Reply from " + Dns.Resolve(myRequest.host).AddressList[0].ToString() + ": bytes=" + myRequest.weightPacket.ToString() + " time=" + Math.Round(myResult.duration.TotalMilliseconds).ToString() + "ms " + "TTL " + myResult.TTL.ToString());
						}

						if (1000 - myResult.duration.TotalMilliseconds > 0)
						{
							Thread.Sleep(myRequest.periodUpdate - (int)myResult.duration.TotalMilliseconds);
						}
					}
					catch 
					{
						Console.WriteLine("Network error.");
					}
					myRequest.nbrEcho--;
				}
			}//end of ping


			//The request will be a tracert
			else if (myRequest.myCommandType == CommandType.tracert)
			{
				Icmp myIcmp = new Icmp(Dns.Resolve(myRequest.host).AddressList[0]);
				Console.WriteLine("Tracing route to " + myRequest.host + " [" + Dns.Resolve(myRequest.host).AddressList[0].ToString()+ "]\r\nover a maximum of " + myRequest.timeToLiveMax.ToString() + " hops:");
				Console.WriteLine();
				bool find = false;
				bool timeOut = false;
				int nbrTTL = 0;
				int i = 1;
				int nbrError = 0;
				string IPToFind = Dns.Resolve(myRequest.host).AddressList[0].ToString();
				while (!find & !timeOut)
				{
					try
					{
						nbrTTL++;

						PingRequestInformation myResult = myIcmp.Ping(myRequest.timeout,nbrTTL);

						//Do it again for the first stop to get a correct value
						//if (nbrTTL == 2) myResult = myIcmp.Ping(myRequest.timeout,nbrTTL);
						

						if (myResult.duration.Equals(TimeSpan.MaxValue))
						{
							throw new Exception("Time out");
						}
						else
						{
							string hostName = "";
							try
							{
								hostName = Dns.GetHostByAddress(myResult.IPEndPoint).HostName.ToString() + " ";
							}
							catch{}

							Console.WriteLine("   ".Substring(0,3-i.ToString().Length) + i.ToString() + "\t" + Convert.ToInt32(Math.Max(1,myResult.duration.TotalMilliseconds)).ToString() + " ms\t" + hostName + "[" + Dns.Resolve(myResult.IPEndPoint).AddressList[0].ToString() + "]");
							nbrError = 0;
							i++;
						}
						if (myResult.IPEndPoint == IPToFind)
						{
							find = true;
						}
						if (nbrTTL >= myRequest.timeToLiveMax)
						{
							timeOut = true;
						}
					}
					catch
					{
						nbrError++;
						if (nbrError > 10) timeOut = true;
					}

					
				}
				Console.WriteLine("\r\nTrace complete.");

			}

		}

		
	}


	

	public class ToolsCommandRequest
	{


		//Default value
		public int periodUpdate = 1000;
		public CommandType myCommandType = CommandType.tracert;
		public string host = "127.0.0.1";
		public int weightPacket = 32;
		public string sessionID;
		public int nbrEcho = 4;
		public int timeout = 1000;
		public int timeToLiveMax = 128;

		public ToolsCommandRequest(string host,string sessionID,CommandType myCommandType, int nbrEcho, int weightPacket,int periodUpdate, int timeout, int timeToLiveMax)
		{
			if (weightPacket < 1)  weightPacket = 1;
			this.periodUpdate = periodUpdate;
			this.myCommandType = myCommandType;
			this.host = host;
			this.weightPacket = weightPacket;
			this.sessionID = sessionID;
			this.nbrEcho = nbrEcho;
			this.timeout = timeout;
			this.timeToLiveMax = timeToLiveMax;
		}

	}


	public enum CommandType
	{
		ping,
		tracert
	}
}
