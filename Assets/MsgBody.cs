using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MsgBody 
{
	public MsgBody()
	{

	}
	public MsgBody(string str)
	{
		this.BodyData = str;
		ToByteArray();
	}
	public MsgBody(byte[] b)
	{
		this.Source = b;
		SetValues();
	}
	public int PacketLength=0;//封包总大小
	public byte[] PacketLengthByte;

	public Int16 HeaderLength=16;//头部长度
	public byte[] HeaderLengthByte;

	public Int16 ProtocolVersion = 1;//协议版本
	public byte[] ProtocolVersionByte;

	public int Operation=7;//操作码 7表示认证并加入房间
	public byte[] OperationByte;

	public int SequenceId=1;//就1
	public byte[] SequenceIdByte;

	public string BodyData;//包体数据
	public byte[] BodyDataByte;

	public byte End = 0;

	public byte[] Source;
	public void writeInt(byte[] buffer,int start,int lengh,int value)
	{
		int i = 0;
		while(i<lengh)
		{
			buffer[start + i] =(byte)(value / Math.Pow(256, lengh - i - 1));
			i++;
		}
	}
	public byte[] ToByteArray()
	{

		PacketLengthByte = BitConverter.GetBytes(PacketLength);
		HeaderLengthByte = BitConverter.GetBytes(HeaderLength);
		ProtocolVersionByte = BitConverter.GetBytes(ProtocolVersion);
		OperationByte = BitConverter.GetBytes(Operation);
		SequenceIdByte = BitConverter.GetBytes(SequenceId);
		BodyDataByte = Encoding.UTF8.GetBytes(BodyData);
		//Debug.Log(PacketLengthByte[1]);
		byte[] result = new byte[BodyDataByte.Length + 16];

		writeInt(result, 0, 4, PacketLength);
		writeInt(result, 4, 2, HeaderLength);
		writeInt(result, 6, 2, ProtocolVersion);
		writeInt(result, 8, 4, Operation);
		writeInt(result, 12, 4, SequenceId);
		//Array.Copy(PacketLengthByte, 0, result, 3, 4);
		//Array.Copy(HeaderLengthByte, 0, result, 5, 2);
		//Array.Copy(ProtocolVersionByte, 0, result, 7, 2);
		//Array.Copy(OperationByte, 0, result, 11, 4);
		//Array.Copy(SequenceIdByte, 0, result, 15, 4);
		Array.Copy(BodyDataByte, 0, result, 16, BodyDataByte.Length);

		Source = new byte[result.Length];
		Array.Copy(result, 0, Source, 0, result.Length);
		return result;
	}

	public void SetValues()
	{
		try
		{
			PacketLengthByte = WebSocketTest.SubByte(Source, 0, 4);
			HeaderLengthByte = WebSocketTest.SubByte(Source, 4, 6);
			ProtocolVersionByte = WebSocketTest.SubByte(Source, 6, 8);
			OperationByte = WebSocketTest.SubByte(Source, 8, 12);
			SequenceIdByte= WebSocketTest.SubByte(Source, 12, 16);
			BodyDataByte= WebSocketTest.SubByte(Source, 16, BodyDataByte.Length);

			End = WebSocketTest.SubByte(Source, Source.Length - 2, 1)[0];

			PacketLength = BitConverter.ToInt32(PacketLengthByte, 0);
			HeaderLength = BitConverter.ToInt16(HeaderLengthByte, 0);
			ProtocolVersion = BitConverter.ToInt16(ProtocolVersionByte, 0);
			Operation = BitConverter.ToInt32(OperationByte, 0);
			SequenceId = BitConverter.ToInt32(SequenceIdByte, 0);
			BodyData = Encoding.UTF8.GetString(BodyDataByte);
		}
		catch (Exception ex)
		{

			throw;
		}
	}
}
