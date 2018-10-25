// Link de youtube https://www.youtube.com/watch?v=uIanSvWou1M

#pragma warning(disable:4996)
#include <iostream>
#include <WS2tcpip.h>


// Include the Winsock library (lib) file
#pragma comment (lib, "ws2_32.lib")

// Saves us from typing std::cout << etc. etc. etc.
using namespace std; 

// Main entry point into the server
void main()
{
	////////////////////////////////////////////////////////////
	// INITIALIZE WINSOCK
	////////////////////////////////////////////////////////////

	// Structure to store the WinSock version. This is filled in
	// on the call to WSAStartup()
	WSADATA data;

	// To start WinSock, the required version must be passed to
	// WSAStartup(). This server is going to use WinSock version
	// 2 so I create a word that will store 2 and 2 in hex i.e.
	// 0x0202
	WORD version = MAKEWORD(2, 2);

	// Start WinSock
	int wsOk = WSAStartup(version, &data);
	if (wsOk != 0)
	{
		// Not ok! Get out quickly
		cout << "Can't start Winsock! " << wsOk;
		return;
	}

	////////////////////////////////////////////////////////////
	// SOCKET CREATION AND BINDING
	////////////////////////////////////////////////////////////

	// Create a socket, notice that it is a user datagram socket (UDP)
	SOCKET in = socket(AF_INET, SOCK_DGRAM, 0);

	// Create a server hint structure for the server
	sockaddr_in serverHint;
	serverHint.sin_addr.S_un.S_addr = ADDR_ANY;
	serverHint.sin_family = AF_INET; // Address format is IPv4
	serverHint.sin_port = htons(54000); // Convert from little to big endian

	// Try and bind the socket to the IP and port
	if (bind(in, (sockaddr*)&serverHint, sizeof(serverHint)) == SOCKET_ERROR)
	{
		cout << "Can't bind socket! " << WSAGetLastError() << endl;
		return;
	}

	char str[INET_ADDRSTRLEN];
	inet_ntop(AF_INET, &(serverHint.sin_addr), str, INET_ADDRSTRLEN);

	std::cout << str << '\t' << serverHint.sin_port << '\n';


	////////////////////////////////////////////////////////////
	// MAIN LOOP SETUP AND ENTRY
	////////////////////////////////////////////////////////////

	sockaddr_in client; // Use to hold the client information (port / ip address)
	int clientLength = sizeof(client); // The size of the client information

	char buf[1024];

	// Enter a loop
	while (true)
	{
		ZeroMemory(&client, clientLength); // Clear the client structure
		ZeroMemory(buf, 1024); // Clear the receive buffer

		// Wait for message
		int bytesIn = recvfrom(in, buf, 1024, 0, (sockaddr*)&client, &clientLength);
		if (bytesIn == SOCKET_ERROR)
		{
			cout << "Error receiving from client " << WSAGetLastError() << endl;
			continue;
		}

		// Display message and client info
		char clientIp[256]; // Create enough space to convert the address byte array
		ZeroMemory(clientIp, 256); // to string of characters

		// Convert from byte array to chars
		inet_ntop(AF_INET, &client.sin_addr, clientIp, 256);

		//Both coordenates x,y to unserialize.
		/*
		int x = 0;
		int y = 0;
		for (unsigned n = 0; n < 4; n++)
			x = (x << 8) + buf[n];

		for (unsigned n = 4; n < 8; n++)
			y = (y << 8) + buf[n];

		*/
		int x = 0;
		x = 0xFF & buf[3];
		x |= (0xFF & buf[2]) << 8;
		x |= (0xFF & buf[1]) << 16;
		x |= (0xFF & buf[0]) << 24;

		int y = 0;
		y = 0xFF & buf[7];
		y |= (0xFF & buf[6]) << 8;
		y |= (0xFF & buf[5]) << 16;
		y |= (0xFF & buf[4]) << 24;

		
		/*
		int valx = 0;
		buf[3] = (valx & 0xff000000) << 24;
		buf[2] = (valx & 0x00ff0000) << 16;
		buf[1] = (valx & 0x0000ff00) << 8;
		buf[0] = (valx & 0x000000ff);
		*/
		//int* x = (int*)buf;
		

		// Display the message / who sent it
		cout << "Message recv from " << clientIp << " : La x es: " << x <<
			" y la y es: " << y << endl;
	}

	// Close socket
	closesocket(in);

	// Shutdown winsock
	WSACleanup();
}
