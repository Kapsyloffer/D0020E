using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;

public class SwordCapture : MonoBehaviour
{
	Thread receiveThread; //1
	UdpClient client; //2
	int port = 5065; //3
	string x;
	string y;
	string w;
	string h;
	static int x1;
	static int y1;
	static int w1;
	static int h1;
	float x2;
	float y2;
	float w2;
	float h2;



	//public gameObject cube;
	public Vector3 pos;
	Vector3 size;
	//client = new UdpClient(port); //1

	// Start is called before the first frame update
	void Start()
	{
		print("UDP Initialized");

		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();


	}


	// 4. Receive Data
	private void ReceiveData()
	{
		client = new UdpClient(port); //1
		while (true) //2
		{
			try
			{
				//print(">> " + "hello");
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //3
				byte[] data = client.Receive(ref anyIP); //4

				string text = Encoding.UTF8.GetString(data); //5
				//print(">> " + text);

				int count = 0;
				foreach (char c in text)
				{
					if (c.Equals(':'))
					{
						count += 1;
					}
					else if (count == 0)
					{
						x = x + c;
					}
					else if (count == 1)
					{
						y = y + c;
					}
					else if (count == 2)
					{
						w = w + c;
					}
					else if (count == 3)
					{
						h = h + c;
					}
				}

				//print("x = " + x);

				int.TryParse(x, out x1);
				int.TryParse(y, out y1);
				int.TryParse(w, out w1);
				int.TryParse(h, out h1);

				

				x2 = x1 * 0.021818f - 8.0f;
				y2 = y1 * 0.021818f - 4.0f;
				w2 = w1 / 10f + x2;
				h2 = h1 / 20f + y2;


				x = "";
				y = "";
				w = "";
				y = "";


			}
			catch (Exception e)
			{
				print(e.ToString()); //7
			}
		}
	}


	// Update is called once per frame
	void Update()
	{





		//pos = sword.transform.position;
		//pos.x = x2;
		//pos.y = y2;
		//transform.position = pos;

		//LetterController.getPos(x1, y1);


		//size = transform.localScale;
		//size.x = w2;
		//size.y = h2;
		//transform.localScale = size;

		//pos = transform.position;
		//pos.x += x1;
		//pos.y += y1;
		//transform.position = pos;
		//GetComponent<BoxCollider2D>().size = new Vector2(1f, 0.5f);
		//cube.transform.position = new Vector3(xPos - 6.0f, -3, 0);
	}

	public static int getX()
    {
		return SwordCapture.x1;

	}

	public static int getY()
    {
		return SwordCapture.y1;
	}

	public static int getW()
	{
		return SwordCapture.w1;
	}

	public static int getH()
	{
		return SwordCapture.h1;
	}



}
